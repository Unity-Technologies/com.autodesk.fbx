// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using FbxSdk;

namespace FbxSdk.Examples
{
    namespace Editor {

        //
        // The exporter01 example illustrates how to:
        //
        //        1) create and initialize an exporter
        //        2) create a scene
        //        3) export a scene to a .FBX file (ASCII mode)
        //

        public class exporter01 : System.IDisposable
        {
            public void Dispose () { }

            /// <summary>
            /// Export all the objects in the set.
            /// Return the number of objects in the set that we exported.
            /// </summary>
            public int ExportAll (IEnumerable<UnityEngine.Object> exportSet)
            {
                // Create fbx manager
                var manager = FbxManager.Create ();

                // Configure fbx IO settings.
                manager.SetIOSettings (FbxIOSettings.Create (manager, Globals.IOSROOT));

                // Create the exporter 
                var exporter = FbxExporter.Create (manager, MakeObjectName ("Exporter"));

                // Initialize the exporter.
                bool status = exporter.Initialize (LastFilePath, -1, manager.GetIOSettings ());
                // Check that initialization of the exporter was successful
                if (!status)
                    return 0;
                
                // Create a scene
                var scene = FbxScene.Create (manager, MakeObjectName ("Scene"));

            	// create scene info
            	FbxDocumentInfo sceneInfo = FbxDocumentInfo.Create (manager, MakeObjectName ("SceneInfo"));

            	// set some scene info values
            	sceneInfo.mTitle = " Example 01: empty scene";
            	sceneInfo.mSubject = "Example of an empty scene with document information settings";
            	sceneInfo.mAuthor = "Unit Technologies";
            	sceneInfo.mRevision = "1.0";
            	sceneInfo.mKeywords = "example empty scene";
            	sceneInfo.mComment = "Set some scene settings. Note that the scene thumnail has not been set.";

            	scene.SetSceneInfo (sceneInfo);

                // Export the scene to the file.
                status = exporter.Export (scene);

                // cleanup
                scene.Destroy ();
                exporter.Destroy ();
                manager.Destroy ();

                return status==true ? 1 : 0; 
            }

            // 
            // Create a simple user interface (menu items)
            //
            /// <summary>
            /// create menu item in the File menu
            /// </summary>
            [MenuItem ("File/Export/Export (Scene only) to FBX", false)]
            public static void OnMenuItem ()
            {
                OnExport();
            }

            /// <summary>
            // Validate the menu item defined by the function above.
            /// </summary>
            [MenuItem ("File/Export/Export (Scene only) to FBX", true)]
            public static bool OnValidateMenuItem ()
            {
                // Return true
                return true;
            }

            //
            // manage the selection of a filename
            //
            static string m_LastFilePath = "";
            static string LastFilePath { get { return m_LastFilePath; } set { m_LastFilePath = value; } }
            static string Basename { get { return GetActiveSceneName (); } }
            static string Extension { get { return "fbx"; } }

            private static string GetActiveSceneName()
            {
                var scene = SceneManager.GetActiveScene();

                return string.IsNullOrEmpty(scene.name) ? "Untitled" : scene.name;    
            }

            private static string MakeFileName(string basename = "test", string extension = "fbx")
            {
                return basename + "." + extension;
            }

            // use the SaveFile panel to allow user to enter a file name
            private static void OnExport()
            {
                // Now that we know we have stuff to export, get the user-desired path.
                var directory = string.IsNullOrEmpty (LastFilePath) ? Application.dataPath : System.IO.Path.GetDirectoryName (LastFilePath);
                var filename = string.IsNullOrEmpty (LastFilePath) ? MakeFileName(basename: Basename, extension: Extension) : System.IO.Path.GetFileName (LastFilePath);
                var title = string.Format ("Export FBX ({0})", Basename);

                var filePath = EditorUtility.SaveFilePanel (title, directory, filename, "");
                if (string.IsNullOrEmpty (filePath)) {
                    return;
                }
                LastFilePath = filePath;

                using (exporter01 exporter = new exporter01()) {
                    
    				// ensure output directory exists
    				EnsureDirectory (filePath);

                    if (exporter.ExportAll(Selection.objects) > 0)
                    {
                        string message = string.Format ("Successfully exported scene: {0}", filePath);
                        UnityEngine.Debug.Log (message);
                    }
                }
            }

            private static void EnsureDirectory(string path)
            {
                //check to make sure the path exists, and if it doesn't then
                //create all the missing directories.
                FileInfo fileInfo = new FileInfo (path);

                if (!fileInfo.Exists) {
                    Directory.CreateDirectory (fileInfo.Directory.FullName);
                }
            }

             private static string MakeObjectName (string name)
            {
                return "_fbxexporter01_" + name;
            }
        }
    }
}