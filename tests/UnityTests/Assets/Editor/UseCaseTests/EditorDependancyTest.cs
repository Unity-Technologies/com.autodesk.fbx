// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Text;
using System;

namespace UnityEngine.Formats.FbxSdk.UseCaseTests{

    public class EditorDependancyTest {

        private string m_exeFileName = null;
        protected string exeFileName {
            get {
#if UNITY_EDITOR
                if(m_exeFileName == null){
                    m_exeFileName = Path.Combine (UnityEngine.Application.dataPath, "UnityDependancyTest.exe");
                }
                return m_exeFileName;
#else
                throw new NotImplementedException();
#endif
            }
        }

        [SetUp]
        public void Init()
        {
            if (!File.Exists (this.exeFileName)) {
                Assert.Ignore ("Missing exe at " + this.exeFileName);
            }
        }

        [Test]
        public void TestCSharpConsoleApp()
        {
            Process consoleApp = new Process ();
            consoleApp.StartInfo.FileName = this.exeFileName;
            consoleApp.StartInfo.RedirectStandardError = true;
            consoleApp.StartInfo.UseShellExecute = false;
            consoleApp.StartInfo.CreateNoWindow = true;
            consoleApp.StartInfo.ErrorDialog = false;

            // point the path to the directories where the native library can be found
            consoleApp.StartInfo.EnvironmentVariables ["PATH"] = 
#if UNITY_EDITOR_WIN            
                Path.Combine (UnityEngine.Application.dataPath, "FbxSdk/Plugins/x64/Windows;") +
#elif UNITY_EDITOR_OSX
                Path.Combine (UnityEngine.Application.dataPath, "FbxSdk/Plugins/x64/MacOS;") +
#elif UNITY_EDITOR_LINUX
                Path.Combine (UnityEngine.Application.dataPath, "FbxSdk/Plugins/x64/Linux;") +
#endif
                consoleApp.StartInfo.EnvironmentVariables ["PATH"];
            
            consoleApp.Start ();

            StringBuilder error = new StringBuilder ();

            error.Append (consoleApp.StandardError.ReadToEnd ());
            while (!consoleApp.HasExited) {
                error.Append (consoleApp.StandardError.ReadToEnd ());
            }
            string errorString = error.ToString ();

            Assert.IsNotEmpty (errorString);
            Assert.IsTrue (errorString.Contains (
                "Unhandled Exception: System.TypeInitializationException: " +
                "The type initializer for 'UnityEngine.Formats.FbxSdk.GlobalsPINVOKE' threw an exception"));
            Assert.IsTrue (errorString.Contains ("InitFbxAllocators"));
        }
    }
}