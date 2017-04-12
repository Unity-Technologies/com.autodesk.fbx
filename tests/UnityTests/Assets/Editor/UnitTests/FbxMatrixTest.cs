using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxMatrixTest
    {
        [Test]
        public void BasicTests ()
        {
            FbxMatrix mx;

            // make sure the constructors compile and don't crash
            mx = new FbxMatrix();
            mx = new FbxMatrix(new FbxMatrix());
            mx = new FbxMatrix(new FbxAMatrix());
            mx = new FbxMatrix(new FbxVector4(), new FbxVector4(), new FbxVector4(1,1,1));
            mx = new FbxMatrix(0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15);

            /* Check the values we typed in match up. */
            for(int y = 0; y < 4; ++y) {
                for(int x = 0; x < 4; ++x) {
                    Assert.AreEqual(x + 4 * y, mx.Get(y, x));
                }
            }

            /* Check that set and get work (silly transpose operation). */
            FbxMatrix mx2 = new FbxMatrix();
            for(int y = 0; y < 4; ++y) {
                for(int x = 0; x < 4; ++x) {
                    mx2.Set(y, x, y + 4 * x);
                    Assert.AreEqual(mx.Get(x, y), mx2.Get(y, x));
                }
            }
        }

        [Test]
        public void TestUsing ()
        {
            /* make sure that the using form compiles and doesn't crash */
            using (new FbxMatrix()) { }
        }
    }
}
