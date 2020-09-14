using System.Collections;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEngine.TestTools;
using System.Diagnostics;

namespace Autodesk.Fbx.BuildTests
{
    public class BuildTest
    {
        private const string k_fbxsdkNativePlugin = "UnityFbxSdkNative";
        private const string k_autodeskFbxDll = "Autodesk.Fbx.dll";

#if UNITY_EDITOR_WIN
        private const string k_fbxsdkNativePluginExt = ".dll";
        private const string k_buildPluginPath = "{0}_Data";
        private const BuildTarget k_buildTarget = BuildTarget.StandaloneWindows64;
        private const string k_autodeskDllInstallPath = "Managed";
#elif UNITY_EDITOR_OSX
        private const string k_fbxsdkNativePluginExt = ".bundle";
        private const string k_buildPluginPath = "{0}.app/Contents";
        private const BuildTarget k_buildTarget = BuildTarget.StandaloneOSX;
        private const string k_autodeskDllInstallPath = "Resources/Data/Managed";
#else // UNITY_EDITOR_LINUX
        private const string k_fbxsdkNativePluginExt = ".so";
        private const string k_buildPluginPath = "{0}_Data";
        private const BuildTarget k_buildTarget = BuildTarget.StandaloneLinux64;
        private const string k_autodeskDllInstallPath = "Managed";
#endif
        
        private const string k_buildTestScene = "Packages/com.autodesk.fbx/Tests/Runtime/BuildTestsAssets/BuildTestScene.unity";

        private const string k_createdFbx = "emptySceneFromRuntimeBuild.fbx";

        private const string k_runningBuildSymbol = "FBX_RUNNING_BUILD_TEST";

        private const string k_buildName = "test.exe";

        private string BuildFolder { get { return Path.Combine(Path.GetDirectoryName(Application.dataPath), "_safe_to_delete_build"); } }

        public static IEnumerable RuntimeFbxSdkTestData
        {
            get
            {
                yield return new TestCaseData(new string[] { k_runningBuildSymbol }, false).SetName("FbxSdkNotIncludedAtRuntime").Returns(null);
                yield return new TestCaseData(new string[] { k_runningBuildSymbol, "FBXSDK_RUNTIME" }, true).SetName("FbxSdkIncludedAtRuntime").Returns(null);
            }
        }

        [SetUp]
        public void Init()
        {
            // Create build folder
            Directory.CreateDirectory(BuildFolder);
        }

        [TearDown]
        public void Term()
        {
            // reset the scripting define symbols
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            // remove the running build symbol and everything after it
            var result = symbols.Split(new string[] { k_runningBuildSymbol, ";" + k_runningBuildSymbol }, System.StringSplitOptions.None);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, result[0]);

            // delete build folder
            if (Directory.Exists(BuildFolder))
            {
                Directory.Delete(BuildFolder, recursive: true);
            }
        }

        private BuildReport BuildPlayer()
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.locationPathName = Path.Combine(BuildFolder, k_buildName);
            options.target = k_buildTarget;
            options.targetGroup = BuildTargetGroup.Standalone;
            options.scenes = new string[] { k_buildTestScene };

            var report = BuildPipeline.BuildPlayer(options);

            Assert.That(report.summary.result, Is.EqualTo(BuildResult.Succeeded));

            return report;
        }

        private void AddDefineSymbols(string[] toAdd)
        {
            if (toAdd.Length <= 0)
            {
                return;
            }

            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!string.IsNullOrEmpty(symbols))
            {
                symbols += ";";
            }

            symbols += toAdd[0];
            for (int i = 1; i < toAdd.Length; i++)
            {
                symbols += ";" + toAdd[i];
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
        }

        [UnityTest]
        [TestCaseSource("RuntimeFbxSdkTestData")]
        public IEnumerator TestFbxSdkAtRuntime(string[] defineSymbols, bool dllExists)
        {
            AddDefineSymbols(defineSymbols);

            // start and stop playmode to force a domain reload
            Assert.False(Application.isPlaying);
            yield return new EnterPlayMode();
            Assert.True(Application.isPlaying);
            yield return new ExitPlayMode();
            Assert.False(Application.isPlaying);

            var report = BuildPlayer();

            // check whether the plugins were copied
            var buildPathWithoutExt = Path.ChangeExtension(report.summary.outputPath, null);
            var buildPluginFullPath = Path.Combine(
                    string.Format(k_buildPluginPath, buildPathWithoutExt),
                    "Plugins",
                    k_fbxsdkNativePlugin + k_fbxsdkNativePluginExt
                );

            NUnit.Framework.Constraints.Constraint constraint = Is.False;
            if (dllExists)
            {
                constraint = Is.True;
            }

#if UNITY_EDITOR_OSX
            Assert.That(Directory.Exists(buildPluginFullPath), constraint);
#else
            Assert.That(File.Exists(buildPluginFullPath), constraint);
#endif

            // check the size of Autodesk.Fbx.dll
            var autodeskDllFullPath = Path.Combine(
                    string.Format(k_buildPluginPath, buildPathWithoutExt),
                    k_autodeskDllInstallPath,
                    k_autodeskFbxDll
                );
            Assert.That(File.Exists(autodeskDllFullPath), Is.True);
            var fileInfo = new FileInfo(autodeskDllFullPath);

            // If the FBX SDK is copied over at runtime, the DLL filesize will
            // be ~350,000. If it isn't copied over it will be ~3500.
            // Putting the expected size as 10000 to allow for some buffer room.
            var expectedDllFileSize = 10000;
            if (dllExists)
            {
                Assert.That(fileInfo.Length, Is.GreaterThan(expectedDllFileSize));
            }
            else
            {
                Assert.That(fileInfo.Length, Is.LessThan(expectedDllFileSize));
            }

            var buildPath = report.summary.outputPath;
            Process p = new Process();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.FileName = buildPath;
            p.StartInfo.UseShellExecute = false;
            p.Start();

            p.WaitForExit();

            // Check that the FBX was created
            var buildPluginFbxPath = Path.Combine(
                    string.Format(k_buildPluginPath, buildPathWithoutExt),
                    k_createdFbx
                );
            Assert.That(File.Exists(buildPluginFbxPath), constraint);
        }
    }
}