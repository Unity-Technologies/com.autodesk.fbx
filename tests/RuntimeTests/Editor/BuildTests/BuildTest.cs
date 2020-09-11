using System.Collections;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEngine.TestTools;

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
#elif UNITY_EDITOR_OSX
        private const string k_fbxsdkNativePluginExt = ".bundle";
        private const string k_buildPluginPath = "{0}.app/Contents";
        private const BuildTarget k_buildTarget = BuildTarget.StandaloneOSX;
#else // UNITY_EDITOR_LINUX
        private const string k_fbxsdkNativePluginExt = ".so";
        private const string k_buildPluginPath = "{0}_Data";
        private const BuildTarget k_buildTarget = BuildTarget.StandaloneLinux64;
#endif

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
                    "Managed",
                    k_autodeskFbxDll
                );
            Assert.That(File.Exists(autodeskDllFullPath), Is.True);
            var fileInfo = new FileInfo(autodeskDllFullPath);
            Debug.Log("Autodesk DLL file size: " + fileInfo.Length);
        }
    }
}