using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxAMatrixTest
    {
        [Test]
        public void BasicTests ()
        {
            // make sure the constructors compile and don't crash
            new FbxAMatrix();
            new FbxAMatrix(new FbxAMatrix());
            new FbxAMatrix(new FbxVector4(), new FbxVector4(), new FbxVector4(1,1,1));

            // TODO: more operations
        }

        [Test]
        public void TestUsing ()
        {
            /* make sure that the using form compiles and doesn't crash */
            using (new FbxAMatrix()) { }

            // Make sure we can explicitly dispose as well.
            new FbxAMatrix().Dispose();
        }

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxAMatrix), this.GetType()); }
#endif
    }
}
