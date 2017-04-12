using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxAMatrixTest
    {
        [Test]
        public void BasicTests ()
        {
            FbxAMatrix mx;

            // make sure the constructors compile and don't crash
            mx = new FbxAMatrix();
            mx = new FbxAMatrix(new FbxAMatrix());
            mx = new FbxAMatrix(new FbxVector4(), new FbxVector4(), new FbxVector4(1,1,1));

            // TODO: more operations
        }

        [Test]
        public void TestUsing ()
        {
            /* make sure that the using form compiles and doesn't crash */
            using (new FbxAMatrix()) { }
        }
    }
}
