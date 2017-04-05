using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;

public class FbxIOSettingsTest {

	[Test]
	public void TestCreate()
	{
		FbxManager manager = FbxManager.Create ();
		FbxIOSettings ioSettings = FbxIOSettings.Create (manager, "");

		Assert.IsNotNull (ioSettings);
		Assert.IsInstanceOf<FbxObject> (ioSettings);

		manager.Destroy ();
	}
}
