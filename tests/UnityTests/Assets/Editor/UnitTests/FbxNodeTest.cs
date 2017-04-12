using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{

    public class FbxNodeTest : Base<FbxNode>
    {
        [Test]
        public void TestNodeBasics ()
        {
            // TODO: UNI-13959 comparisons by name should just be ==
            bool ok;
            FbxNode found;

            // Call every function once in a non-corner-case way
            var root = CreateObject("root");

            Assert.AreEqual(0, root.GetChildCount()); // non-recursive
            Assert.AreEqual(0, root.GetChildCount(true)); // recursive

            var child = CreateObject("child");
            ok = root.AddChild(child);
            Assert.IsTrue(ok);
            Assert.AreEqual(0, child.GetChildCount()); // non-recursive
            Assert.AreEqual(0, child.GetChildCount(true)); // recursive
            Assert.AreEqual(1, root.GetChildCount()); // non-recursive
            Assert.AreEqual(1, root.GetChildCount(true)); // recursive
            found = child.GetParent();
            Assert.AreEqual(root.GetName(), found.GetName());
            found = root.GetChild(0);
            Assert.AreEqual(child.GetName(), found.GetName());

            var grandchild = CreateObject("grandchild");
            ok = child.AddChild(grandchild);
            Assert.IsTrue(ok);
            Assert.AreEqual(0, grandchild.GetChildCount()); // non-recursive
            Assert.AreEqual(0, grandchild.GetChildCount(true)); // recursive
            Assert.AreEqual(1, child.GetChildCount()); // non-recursive
            Assert.AreEqual(1, child.GetChildCount(true)); // recursive
            Assert.AreEqual(1, root.GetChildCount()); // non-recursive
            Assert.AreEqual(2, root.GetChildCount(true)); // recursive
            found = root.GetChild(0);
            Assert.AreEqual(child.GetName(), found.GetName());
            found = child.GetChild(0);
            Assert.AreEqual(grandchild.GetName(), found.GetName());

            // Just creating a node doesn't parent. But they get destroyed together.
            var grandchildOwned = CreateObject(grandchild, "grandchild-owned");
            Assert.AreEqual(0, grandchild.GetChildCount()); // non-recursive

            found = root.FindChild("child"); // non-recursive
            Assert.AreEqual(child.GetName(), found.GetName());
            found = root.FindChild("grandchild"); // non-recursive
            Assert.IsNull(found);
            found = root.FindChild("grandchild", pRecursive: true);
            Assert.AreEqual(grandchild.GetName(), found.GetName());
            found = root.FindChild("grandchild-owned", pRecursive: true);
            Assert.IsNull(found);

            // Destroying the grandchild destroys the object it owns, and unparents it from child.
            grandchild.Destroy();
            Assert.That(() => { grandchildOwned.GetName(); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
            Assert.AreEqual(0, child.GetChildCount());

            // Destroying the child non-recursively (after adding a new
            // grandchild) doesn't destroy the grandchild.
            grandchild = CreateObject("grandchild2");
            child.AddChild(grandchild);
            child.Destroy();
            Assert.AreEqual("grandchild2", grandchild.GetName()); // actually compare by name => check it doesn't throw

            // That reparents the grandchild to the root.
            Assert.AreEqual(root.GetName(), grandchild.GetParent().GetName());

            // Recursively destroying the root destroys the grandchild.
            root.Destroy(pRecursive: true);
            Assert.That(() => { grandchildOwned.GetName(); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }
    }
}
