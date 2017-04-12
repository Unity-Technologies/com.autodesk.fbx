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
        protected string filePath       { get { return "."; } }
        protected string fileNamePrefix { get { return "_safe_to_delete__empty_export_test_"; } }
        protected string fileNameExt    { get { return ".fbx"; } }

        private static Dictionary<string, string> m_dataValues = new Dictionary<string, string> ()
        {
            { "title",      "Empty scene" },
            { "subject",    "Example of an empty scene with document information settings" },
            { "author",     "Unit Technologies" },
            { "revision",   "1.0" },
            { "keywords",   "example empty scene" },
            { "comment",    "basic scene settings. Note that the scene thumnail is not set." },
        };

        protected Dictionary<string, string> dataValues { get { return m_dataValues; } }

        private string MakeFileName(string baseName = null, string prefixName = null, string extName = null)
        {
            if (baseName==null)
                baseName = Path.GetRandomFileName();
            
            if (prefixName==null)
                prefixName = this.fileNamePrefix;
            
            if (extName==null)
                extName = this.fileNameExt;
                
            return prefixName + baseName + extName;
        }

        private string GetRandomFileNamePath(string pathName = null, string prefixName = null, string extName = null)
        {
            string temp;

            if (pathName==null)
                pathName = this.filePath;

            if (prefixName==null)
                prefixName = this.fileNamePrefix;
                
            if (extName==null)
                extName = this.fileNameExt;
            
            // repeat until you find a file that does not already exist
            do {
                temp = Path.Combine (pathName, MakeFileName(prefixName: prefixName, extName: extName));
                
            } while(File.Exists (temp));
            
            return temp;
        }

        private FbxManager m_fbxManager;

        protected FbxManager FbxManager { get { return m_fbxManager; } }

        [SetUp]
        public virtual void Init ()
        {
            foreach (string file in Directory.GetFiles (this.filePath, MakeFileName("*"))) {
                File.Delete (file);
            }

            // create fbx manager.
            m_fbxManager = FbxManager.Create ();

            // configure IO settings.
            m_fbxManager.SetIOSettings (FbxIOSettings.Create (m_fbxManager, Globals.IOSROOT));
        }

        [TearDown]
        public virtual void Term ()
        {
            try {
                m_fbxManager.Destroy ();
            } 
            catch (System.ArgumentNullException) {
            }
        }

        private FbxScene CreateScene (FbxManager manager)
        {
            FbxScene scene = FbxScene.Create (manager, "myScene");

            // create scene info
            FbxDocumentInfo sceneInfo = FbxDocumentInfo.Create (manager, "mySceneInfo");

            sceneInfo.mTitle = dataValues ["title"];
            sceneInfo.mSubject = dataValues ["subject"];
            sceneInfo.mAuthor = dataValues ["author"];
            sceneInfo.mRevision = dataValues ["revision"];
            sceneInfo.mKeywords = dataValues ["keywords"];
            sceneInfo.mComment = dataValues ["comment"];

            scene.SetSceneInfo (sceneInfo);

            // TODO: port SetSceneThumbnail

            return scene;
        }

        private void CheckScene (FbxScene scene, Dictionary<string, string> values)
        {
            FbxDocumentInfo sceneInfo = scene.GetSceneInfo ();

            Assert.AreEqual (sceneInfo.mTitle, values ["title"]);
            Assert.AreEqual (sceneInfo.mSubject, values ["subject"]);
            Assert.AreEqual (sceneInfo.mAuthor, values ["author"]);
            Assert.AreEqual (sceneInfo.mRevision, values ["revision"]);
            Assert.AreEqual (sceneInfo.mKeywords, values ["keywords"]);
            Assert.AreEqual (sceneInfo.mComment, values ["comment"]);
        }

        private void ExportScene (string fileName)
        {
            // Export the scene
            using (FbxExporter exporter = FbxExporter.Create (FbxManager, "myExporter")) {

                // Initialize the exporter.
                bool status = exporter.Initialize (fileName, -1, FbxManager.GetIOSettings ());

                // Check that export status is True
                Assert.IsTrue (status);

                // Create a new scene so it can be populated by the imported file.
                FbxScene scene = CreateScene (FbxManager);

                CheckScene (scene, this.dataValues);

                // Export the scene to the file.
                exporter.Export (scene);

                // Check if file exists
                Assert.IsTrue (File.Exists (fileName));
            }
        }

        private void ImportScene (string fileName)
        {
            // Import the scene to make sure file is valid
            using (FbxImporter importer = FbxImporter.Create (FbxManager, "myImporter")) {

                // Initialize the importer.
                bool status = importer.Initialize (fileName, -1, FbxManager.GetIOSettings ());

                Assert.IsTrue (status);

                // Create a new scene so it can be populated by the imported file.
                FbxScene scene = FbxScene.Create (FbxManager, "myScene");

                // Import the contents of the file into the scene.
                importer.Import (scene);

				// check that the scene is valid
				CheckScene (scene, this.dataValues);
            }
        }

        [Test]
        public void ExportSceneTest ()
        {
            var fileName = GetRandomFileNamePath ();

            this.ExportScene (fileName);

            File.Delete (fileName);
        }

        [Test]
        public void RoundTripTest ()
        {
            var fileName = GetRandomFileNamePath ();
            
            this.ExportScene (fileName);
            this.ImportScene (fileName);

            File.Delete (fileName);
        }
    }
}
