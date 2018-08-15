// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/*
 * Helper for our setup to handle callbacks in
 * - FbxExporter::SetProgressCallback
 * - FbxImporter::SetProgressCallback
 * See fbxprogress.i
 */
class FbxSharpProgressCallback
{
  public:
  FbxSharpProgressCallback() { }
  virtual ~FbxSharpProgressCallback() { }
  virtual bool Progress(float percentage, const char * status) = 0;

  static bool CallProgress(void *callback, float percentage, const char *status) {
    return ((FbxSharpProgressCallback*)callback)->Progress(percentage, status);
  }
};
