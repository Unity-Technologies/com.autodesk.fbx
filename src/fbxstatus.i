// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxStatus;
%rename("%s") FbxStatus::FbxStatus;
%rename("%s") FbxStatus::~FbxStatus;
%rename("%s") FbxStatus::EStatusCode;
%rename("%s") FbxStatus::Clear;
%rename("%s") FbxStatus::Error;
%rename("%s") FbxStatus::GetCode;
%rename("%s") FbxStatus::GetErrorString;

%define_equality_from_operator(FbxStatus);
%ignore FbxStatus::operator == (const EStatusCode) const;
%extend FbxStatus { %proxycode %{
  public override int GetHashCode() { return (int)GetCode(); }
  public static bool operator == (EStatusCode a, FbxStatus b) { return a == b.GetCode(); }
  public static bool operator != (EStatusCode a, FbxStatus b) { return a != b.GetCode(); }
  public static bool operator == (FbxStatus a, EStatusCode b) { return a.GetCode() == b; }
  public static bool operator != (FbxStatus a, EStatusCode b) { return a.GetCode() != b; }
%} }

%define_tostring(FbxStatus, GetCode().ToString() + ": " + GetErrorString());

/*
 * SetCode takes a format string and a vararg. That can crash.  Make C# pass in
 * just a string (which you can already format in C# easily)
 */
%rename("%s") FbxStatus::SetCode(const EStatusCode);
%ignore FbxStatus::SetCode(const EStatusCode rhs, const char* pErrorMsg, ...);
%rename("SetCode") FbxStatus::SetCodeNoFormat;
%extend FbxStatus {
  void SetCodeNoFormat(const EStatusCode rhs, const char* pErrorMsg) {
    $self->SetCode(rhs, "%s", pErrorMsg);
  }
}



%include "fbxsdk/core/base/fbxstatus.h"
