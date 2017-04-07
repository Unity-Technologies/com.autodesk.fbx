using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;
using System;

public abstract class FbxSdkTestBase {

    private FbxManager m_fbxManager;

    protected FbxManager FbxManager {
        get {
            return m_fbxManager;
        }
    }

    [SetUp]
    public virtual void InitTest()
    {
        m_fbxManager = FbxManager.Create ();
    }

    [TearDown]
    public virtual void DestroyTest()
    {
        try{
            // TODO: crashes instead of going to the catch block if fbx manager already destroyed
            m_fbxManager.Destroy ();
        }
        catch(System.ArgumentNullException){}
    }
}
