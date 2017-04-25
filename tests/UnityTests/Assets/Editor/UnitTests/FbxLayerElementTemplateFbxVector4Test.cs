// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using FbxSdk;

namespace UnitTests
{
    /*
     * Convenience class for testing functions of FbxLayerElementTemplateFbxVector4 in its derived classes.
     * 
     * FbxLayerElementTemplateFbxVector4 derives from FbxLayerElement, so we also derive from FbxLayerElementTestBase.
     * FbxLayerElementTemplateFbxVector4 has no public constructors or Create function, therefore
     * this class is abstract and must be inherited from for the tests to run.
     */
    public abstract class FbxLayerElementTemplateFbxVector4Test<T> : FbxLayerElementTestBase<T>
        where T: FbxSdk.FbxLayerElementTemplateFbxVector4
    {

    }

    public class FbxLayerElementNormalTest : FbxLayerElementTemplateFbxVector4Test<FbxLayerElementNormal>
    {}

    public class FbxLayerElementBinormalTest : FbxLayerElementTemplateFbxVector4Test<FbxLayerElementBinormal>
    {}

    public class FbxLayerElementTangentTest : FbxLayerElementTemplateFbxVector4Test<FbxLayerElementTangent>
    {}
}