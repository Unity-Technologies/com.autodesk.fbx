using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxImporterTest : Base
    {
        protected override FbxObject CreateObject()
        {
            return FbxImporter.Create (FbxManager, "MyImporter");
        }

        [Test]
        public void TestImport1 ()
        {
            using (FbxImporter newImporter = FbxImporter.Create (FbxManager, "MyImporter"))
            {
                Assert.IsFalse (newImporter.Import (null));
            }
        }

        [Test]
        public void TestImport2 ()
        {
            using (FbxImporter newImporter = FbxImporter.Create (FbxManager, "MyImporter"))
            {
                Assert.IsFalse (newImporter.Import (null, false));

                // don't ask
                Assert.IsTrue (newImporter.Import (null, true));
            }
        }
    }
}
