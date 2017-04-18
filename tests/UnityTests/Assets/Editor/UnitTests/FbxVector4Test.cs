using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxVector4Test
    {
        [Test]
        public void BasicTests ()
        {
            FbxVector4 v;

            // make sure the constructors compile and don't crash
            v = new FbxVector4();
            v = new FbxVector4(new FbxVector4());
            v = new FbxVector4(1, 2, 3, 4);
            v = new FbxVector4(1, 2, 3);
        }

        [Test]
        public void TestUsing ()
        {
            /* make sure that the using form compiles and doesn't crash */
            using (new FbxVector4()) { }

            // Make sure we can explicitly dispose as well.
            new FbxVector4().Dispose();
        }

        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxVector4), this.GetType()); }
    }
}
