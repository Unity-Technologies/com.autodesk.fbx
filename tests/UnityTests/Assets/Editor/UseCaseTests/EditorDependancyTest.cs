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

namespace Unity.FbxSdk.UseCaseTests{

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
            consoleApp.Start ();

            StringBuilder error = new StringBuilder ();
            while (!consoleApp.HasExited) {
                error.Append (consoleApp.StandardError.ReadToEnd ());
            }
            
            Assert.IsNotEmpty (error.ToString ());
            Assert.IsTrue (error.ToString ().Contains (
                "Unhandled Exception: System.TypeInitializationException: " +
                "The type initializer for 'Unity.FbxSdk.GlobalsPINVOKE' threw an exception"));
        }
    }
}