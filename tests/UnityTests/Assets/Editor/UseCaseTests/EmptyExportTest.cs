// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
 
using NUnit.Framework;
using FbxSdk;
using System.IO;
using System.Collections.Generic;
            
namespace UseCaseTests
{

    public class EmptyExportTest
    {
        protected string fileNamePrefix { get { return "safe_to_delete_"; } }

        private static Dictionary<string, string> m_dataValues = null;

        protected Dictionary<string, string> dataValues {
            get {
                if (m_dataValues == null) {
                    m_dataValues = new Dictionary<string, string> ()
                    {
                        { "title",      "Empty scene" },
                        { "subject",    "Example of an empty scene with document information settings" },
                        { "author",     "Unit Technologies" },
                        { "revision",   "1.0" },
                        { "keywords",   "example empty scene" },
                        { "comment",    "basic scene settings. Note that the scene thumnail is not set." },
                    };
                }
                return m_dataValues;
            }
        }

        // Declare the path and filename for the scene to be exported.
        // In this case, the file will be in the same directory as the executable.
        private string m_fileName = null;

        protected string fileName {
        	get {
                if (m_fileName==null) {
                    m_fileName = this.fileNamePrefix + Path.GetRandomFileName ();
                }
        		return m_fileName;
        	}
        }

        [SetUp]
        public virtual void Init ()
        {
            foreach (string file in Directory.GetFiles (".", this.fileNamePrefix + "*.fbx")) {
                File.Delete (file);
            }
        }

        [TearDown]
        public virtual void Term ()
        {
            // Delete the file once the test is complete
            File.Delete (fileName);
        }

        private FbxScene CreateScene(FbxManager manager)
        {
            FbxScene scene = FbxScene.Create (manager, "myScene");

            // create scene info
            FbxDocumentInfo sceneInfo = FbxDocumentInfo.Create (manager, "mySceneInfo");

            sceneInfo.mTitle      = dataValues["title"];
            sceneInfo.mSubject    = dataValues["subject"];
            sceneInfo.mAuthor     = dataValues["author"];
            sceneInfo.mRevision   = dataValues["revision"];
            sceneInfo.mKeywords   = dataValues["keywords"];
            sceneInfo.mComment    = dataValues["comment"];

            scene.SetSceneInfo (sceneInfo);

            // TODO: port SetSceneThumbnail

            return scene;
        }

        private void CheckSceneInfo(FbxScene scene, Dictionary<string,string> values)
        {
            FbxDocumentInfo sceneInfo = scene.GetSceneInfo ();

            Assert.Equals (sceneInfo.mTitle,    values["title"]);
            Assert.Equals (sceneInfo.mSubject,  values["subject"]);
            Assert.Equals (sceneInfo.mAuthor,   values["author"]);
            Assert.Equals (sceneInfo.mRevision, values["revision"]);
            Assert.Equals (sceneInfo.mKeywords, values["keywords"]);
            Assert.Equals (sceneInfo.mComment,  values["comment"]);
        }

        [Test]
        public void EditorTest ()
        {
            // Create the FBX SDK manager
            using (FbxManager manager = FbxManager.Create ()) {
                
                // Create an IOSettings object.
                FbxIOSettings iosettings = FbxIOSettings.Create (manager, Globals.IOSROOT);
                manager.SetIOSettings (iosettings);

                // Export the scene
                using (FbxExporter exporter = FbxExporter.Create (manager, "myExporter")) {

                    // Initialize the exporter.
                    bool status = exporter.Initialize (this.fileName, -1, manager.GetIOSettings ());

                    // Check that export status is True
                    Assert.IsTrue (status);

                    // Create a new scene so it can be populated by the imported file.
                    FbxScene scene = CreateScene (manager);

                    CheckSceneInfo (scene, this.dataValues);

                    // Export the scene to the file.
                    exporter.Export (scene);

                    // Check if file exists
                    Assert.IsTrue (File.Exists (fileName));

                    exporter.Destroy ();
                }

                // Import the scene to make sure file is valid
                using (FbxImporter importer = FbxImporter.Create (manager, "myImporter")) {

                    // Initialize the importer.
                    bool status = importer.Initialize (this.fileName, -1, manager.GetIOSettings ());

                    Assert.IsTrue (status);

                    // Create a new scene so it can be populated by the imported file.
                    FbxScene scene = FbxScene.Create (manager, "myScene");

                    // Import the contents of the file into the scene.
                    importer.Import (scene);

                    // check that the scene is valid
                    CheckSceneInfo (scene, this.dataValues);

                    importer.Destroy ();
                }

                manager.Destroy ();
            }
        }
    }
}