// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxImporterTest : FbxIOBaseTest<FbxImporter>
    {
        [Test]
        public override void TestBasics ()
        {
            base.TestBasics();

            using (FbxImporter newImporter = CreateObject("MyImporter"))
            {
                // import a null document.
                Assert.IsFalse (newImporter.Import (null));

                // set a callback function
                newImporter.SetProgressCallback(null);
                newImporter.SetProgressCallback((float a, string b) => true);
                newImporter.SetProgressCallback(null);
            }

            // Export an empty scene to a temp file, then import.
            var filename = GetRandomFile();
            try {
                using(var exporter = FbxExporter.Create(Manager, "exporter")) {
                    using (var scene = FbxScene.Create(Manager, "exported scene")) {
                        Assert.IsTrue(exporter.Initialize(filename));
                        Assert.IsTrue(exporter.Export(scene));
                    }
                }
                var scene_in = FbxScene.Create(Manager, "imported scene");
                using(var importer = FbxImporter.Create(Manager, "import")) {
                    Assert.IsTrue(importer.Initialize(filename));
                    Assert.IsTrue(importer.Import(scene_in));
                }
                // we actually don't care about the scene itself!
            } finally {
                System.IO.File.Delete(filename);
            }
        }

        string GetRandomFile()
        {
            var tmp = System.IO.Path.GetTempPath();
            for(int i = 0; i < 20; ++i) {
                var path = System.IO.Path.Combine(tmp, System.IO.Path.GetRandomFileName()) + ".fbx";
                if (!System.IO.File.Exists(path)) {
                    return path;
                }
            }
            throw new System.IO.IOException("can't find an unused random temp filename");
        }
    }
}
