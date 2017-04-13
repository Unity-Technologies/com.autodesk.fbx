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
        // The FbxExporter01 example illustrates how to:
        //
        //        1) create and initialize an exporter
        //        2) create scene
        //        3) export a scene in a .FBX file (ASCII mode)
        //

        public class FbxExporter01 : System.IDisposable
        {
            private FbxManager m_fbxManager;
            private FbxManager FbxManager   { get { return m_fbxManager; } }

            private FbxExporter m_fbxExporter;
            private FbxExporter FbxExporter { get { return m_fbxExporter; } }

            private FbxScene m_fbxScene;
            private FbxScene FbxScene       { get { return m_fbxScene; } }

            public void Dispose ()
            {
                // FbxManager is disposable so when this class is disposed it  
                // will also be disposed so we have nothing to do anything here.
            }

            private bool BeginExport ()
            {
                // create fbx manager
                m_fbxManager = FbxManager.Create ();

                // configure fbx IO settings.
                m_fbxManager.SetIOSettings (FbxIOSettings.Create (m_fbxManager, Globals.IOSROOT));

                // create exporter for the scene
                m_fbxExporter = FbxExporter.Create (FbxManager, MakeObjectName ("Exporter"));

                // ensure output directory exists
                EnsureDirectory (LastFilePath);

                // Initialize the exporter.
                bool status = m_fbxExporter.Initialize (LastFilePath, -1, FbxManager.GetIOSettings ());
                // Check that initialization of the exporter was successful
                if (!status)
                    return false;
                
                // Create a new scene
                m_fbxScene = CreateScene (FbxManager);

                return true;
            }

            private bool EndExport ()
            {
                // Export the scene to the file.
                return FbxExporter.Export (FbxScene);
            }

            private FbxScene CreateScene (FbxManager manager)
            {
                // create the scene
                FbxScene scene = FbxScene.Create (manager, MakeObjectName ("Scene"));

                // create scene info
                FbxDocumentInfo sceneInfo = FbxDocumentInfo.Create (manager, MakeObjectName ("SceneInfo"));

                // set some values on the scene
                sceneInfo.mTitle = " Example 01: empty scene";
                sceneInfo.mSubject = "Example of an empty scene with document information settings";
                sceneInfo.mAuthor = "Unit Technologies";
                sceneInfo.mRevision = "1.0";
                sceneInfo.mKeywords = "example empty scene";
                sceneInfo.mComment = "Set some scene settings. Note that the scene thumnail has not been set.";

                scene.SetSceneInfo (sceneInfo);

                return scene;
            }

            /// <summary>
            /// Export all the objects in the set.
            /// Return the number of objects in the set that we exported.
            /// </summary>
            public int ExportAll(IEnumerable<UnityEngine.Object> exportSet)
            {
                if (!BeginExport ())
                    return 0;
                
                return EndExport()==true ? 1 : 0; 
            }

            // 
            // Create simple user interface (menu items) to access exporter
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
            // manage selecting a filename
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

            // use the SaveFile dialog to allow user to enter a file name
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

                using (FbxExporter01 exporter = new FbxExporter01()) {
                    if (exporter.ExportAll(Selection.objects) > 0)
                    {
                        string message = string.Format ("Successfully exported scene: {0}", filename);
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
                return "_example_export_empty_scene_" + name;
            }
        }
    }
}