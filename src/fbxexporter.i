// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxExporter;
%rename("%s") FbxExporter::Export(FbxDocument *pDocument);
%rename("%s") FbxExporter::SetFileExportVersion(FbxString pVersion);
#endif

/* TODO: implement this! */
%ignore FbxExporter::SetProgressCallback;

%apply bool & OUTPUT { bool & pExportResult };

/* Described as obsolete but not deprecated. */
%ignore FbxExporter::SetResamplingRate;
%ignore FbxExporter::SetDefaultRenderResolution;

/* GetCurrentWritableVersions returns a null-terminated list of strings. That
 * takes some handling. */
%ignore FbxExporter::GetCurrentWritableVersions;
%rename ("%s") FbxExporter::_getCurrentWritableVersionsLength;
%rename ("%s") FbxExporter::_getCurrentWritableVersionByIndex;
%extend FbxExporter {
  %csmethodmodifiers _getCurrentWritableVersionsLength "private";
  %csmethodmodifiers _getCurrentWritableVersionByIndex "private";
  int _getCurrentWritableVersionsLength () {
    auto versions = $self->GetCurrentWritableVersions();
    int i = 0;
    for( ; versions[i] != nullptr; ++i) { }
    return i;
  }
  const char *_getCurrentWritableVersionByIndex(int i) {
    return $self->GetCurrentWritableVersions()[i];
  }
  %proxycode %{
  public string[] GetCurrentWritableVersions() {
    var versions = new string[_getCurrentWritableVersionsLength()];
    for(int i = 0, n = versions.Length; i < n; ++i) {
      versions[i] = _getCurrentWritableVersionByIndex(i);
    }
    return versions;
  }
  %}
}

#ifndef SWIG_GENERATING_TYPEDEFS
// TODO: should we be more specific, test each function in turn for whether it can
// actually take null?
%apply FbxIOSettings * MAYBENULL { FbxIOSettings * };
%apply FbxDocument * MAYBENULL { FbxDocument *pDocument };
#endif

%include "fbxsdk/fileio/fbxexporter.h"
