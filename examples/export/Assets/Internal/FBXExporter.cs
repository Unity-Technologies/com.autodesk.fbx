using System.IO;
using UnityEngine;
using FbxSdk;

namespace FbxSdk.Examples
{
    public class FBXExporter : AbstractExporter
    {
        private FbxManager m_fbxManager;
        private FbxManager FbxManager { get { return m_fbxManager; } }

        FbxExporter m_fbxExporter;
        private FbxExporter FbxExporter { get { return m_fbxExporter; } }

        FbxScene m_fbxScene;
        private FbxScene FbxScene { get { return m_fbxScene; } }

        static readonly ExportMethod s_method = ExportMethod.ConstructMethod<FBXExporter> ("FBX", "fbx", "application/fbx", ExportSettings.AxesSettings.RightHandedZUp);

        static public ExportMethod StaticExportMethod {
            get {
                return s_method;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FBXExporter"/> class.
        /// Normally you should do this in a using block, for example:
        /// <code> using (var exporter = new FbxSdkCSharp.FBXExporter(filename)) {
        ///   exporter.Export(a); exporter.Export(b); 
        /// }</code>
        /// 
        /// If you don't call Dispose(), the file will be corrupted.
        /// </summary>
        public FBXExporter (ExportSettings settings) : base (settings)
                {
            //check to make sure the path exists, and if it doesn't then
            //create all the missing directories.
            FileInfo fileInfo = new FileInfo (settings.DestinationPath);

            if (!fileInfo.Exists) {
                Directory.CreateDirectory (fileInfo.Directory.FullName);
            }
        }

        public override void BeginExport ()
        {
            // create fbx manager
            m_fbxManager = FbxManager.Create ();

            // configure fbx IO settings.
            m_fbxManager.SetIOSettings (FbxIOSettings.Create (m_fbxManager, Globals.IOSROOT));

            // create exporter for the scene
            m_fbxExporter = FbxExporter.Create (FbxManager, MakeObjectName("Exporter"));

            // Initialize the exporter.
            bool status = m_fbxExporter.Initialize (Settings.DestinationPath, -1, FbxManager.GetIOSettings ());

            // Check that export status is True
            if (status)
            {
                // Create a new scene
                m_fbxScene = CreateScene (FbxManager);
            }
        }

        public override void EndExport ()
        {
            // Export the scene to the file.
            FbxExporter.Export (FbxScene);
        }

        public override void Dispose ()
        {
            m_fbxManager.Destroy ();
        }

        protected override void ExportComponents (GameObject gameObject)
        {
            string msg = string.Format (gameObject.name);
            UnityEngine.Debug.Log (msg);
        }

        private static string MakeObjectName(string name)
        {
            return "examples_export_" + name;
        }
        private FbxScene CreateScene (FbxManager manager)
        {
            FbxScene scene = FbxScene.Create (manager, MakeObjectName("Scene"));

            // create scene info
            FbxDocumentInfo sceneInfo = FbxDocumentInfo.Create (manager, MakeObjectName("SceneInfo"));

            sceneInfo.mTitle = "bob";
            sceneInfo.mSubject = "bob";
            sceneInfo.mAuthor = "bob";
            sceneInfo.mRevision = "bob";
            sceneInfo.mKeywords = "bob";
            sceneInfo.mComment = "bob";

            scene.SetSceneInfo (sceneInfo);

            // TODO: port SetSceneThumbnail

            return scene;
        }


    }
}