using UnityEngine.Formats.FbxSdk;

namespace EditorDependancyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FbxManager manager = FbxManager.Create();
            manager.Destroy();
        }
    }
}
