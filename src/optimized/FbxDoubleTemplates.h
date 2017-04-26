// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/**
 * These structs are designed to be bitwise compatible with the C# structs in
 * the corresponding .cs file.
 *
 * They need to be assignable from an FbxDouble, convert implicitly to one, and
 * they must be POD -- trivial ctor/dtor, nothing virtual (otherwise we get a
 * compiler warning).
 */

struct FbxSharpDouble2 {
    double x;
    double y;
    inline void operator = (const FbxDouble2& fbx) { 
        x = fbx.mData[0]; 
        y = fbx.mData[1]; 
    }
    inline operator FbxDouble2 () const { return FbxDouble2(x, y); }
};

struct FbxSharpDouble3 {
    double x;
    double y;
    double z;
    inline void operator = (const FbxDouble3& fbx) { 
        x = fbx.mData[0]; 
        y = fbx.mData[1]; 
        z = fbx.mData[2]; 
    }
    inline operator FbxDouble3 () const { return FbxDouble3(x, y, z); }
};

struct FbxSharpDouble4 {
    double x;
    double y;
    double z;
    double w;
    inline void operator = (const FbxDouble4& fbx) { 
        x = fbx.mData[0]; 
        y = fbx.mData[1]; 
        z = fbx.mData[2]; 
        w = fbx.mData[3];
    }
    inline operator FbxDouble4 () const { return FbxDouble4(x, y, z, w); }
    inline operator FbxVector4 () const { return FbxVector4(x, y, z, w); }
};
