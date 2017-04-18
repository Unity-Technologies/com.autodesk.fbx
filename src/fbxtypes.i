%ignore FbxMin;
%ignore FbxMax;

%define %rename_vector_operators(THETYPE)
%ignore THETYPE::operator[]; // TODO: support operator[]
%ignore THETYPE::operator=;
%rename("Equals") THETYPE::operator==;
%ignore THETYPE::operator!=;
%ignore THETYPE::Buffer;
%ignore THETYPE::mData;
%ignore operator THETYPE<T>&;
%enddef

%rename_vector_operators(FbxVectorTemplate2);
%rename_vector_operators(FbxVectorTemplate3);
%rename_vector_operators(FbxVectorTemplate4);

/* We aren't ignoring everything, so we need to explicitly ignore all these
 * constants. */
%ignore FBXSDK_CHAR_MIN;
%ignore FBXSDK_CHAR_MAX;
%ignore FBXSDK_UCHAR_MIN;
%ignore FBXSDK_UCHAR_MAX;
%ignore FBXSDK_SHORT_MIN;
%ignore FBXSDK_SHORT_MAX;
%ignore FBXSDK_USHORT_MIN;
%ignore FBXSDK_USHORT_MAX;
%ignore FBXSDK_INT_MIN;
%ignore FBXSDK_INT_MAX;
%ignore FBXSDK_UINT_MIN;
%ignore FBXSDK_UINT_MAX;
%ignore FBXSDK_LONG_MIN;
%ignore FBXSDK_LONG_MAX;
%ignore FBXSDK_ULONG_MIN;
%ignore FBXSDK_ULONG_MAX;
%ignore FBXSDK_LONGLONG_MIN;
%ignore FBXSDK_LONGLONG_MAX;
%ignore FBXSDK_ULONGLONG_MIN;
%ignore FBXSDK_ULONGLONG_MAX;
%ignore FBXSDK_FLOAT_MIN;
%ignore FBXSDK_FLOAT_MAX;
%ignore FBXSDK_FLOAT_EPSILON;
%ignore FBXSDK_DOUBLE_MIN;
%ignore FBXSDK_DOUBLE_MAX;
%ignore FBXSDK_DOUBLE_EPSILON;
%ignore FBXSDK_TOLERANCE;
%ignore FBXSDK_SYSTEM_IS_LP64;
%ignore FBXSDK_REF_MIN;
%ignore FBXSDK_REF_MAX;
%ignore FBXSDK_ATOMIC_MIN;
%ignore FBXSDK_ATOMIC_MAX;


%include "fbxsdk/core/arch/fbxtypes.h"

// Templates must *follow* the include, unlike everything else in swig.
%template("FbxDouble2") FbxVectorTemplate2<double>;
%template("FbxDouble3") FbxVectorTemplate3<double>;
%template("FbxDouble4") FbxVectorTemplate4<double>;
%template("FbxDouble4x4") FbxVectorTemplate4<FbxDouble4>;
