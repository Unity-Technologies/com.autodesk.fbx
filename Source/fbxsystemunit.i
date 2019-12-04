// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxSystemUnit;
%ignore FbxSystemUnit::FbxSystemUnit();
%rename("%s") FbxSystemUnit::~FbxSystemUnit();
%rename("%s") FbxSystemUnit::mm;
%rename("%s") FbxSystemUnit::cm;
%rename("%s") FbxSystemUnit::dm;
%rename("%s") FbxSystemUnit::m;
%rename("%s") FbxSystemUnit::km;
%rename("%s") FbxSystemUnit::Inch;
%rename("%s") FbxSystemUnit::Foot;
%rename("%s") FbxSystemUnit::Yard;
%rename("%s") FbxSystemUnit::GetScaleFactor;
%rename("%s") FbxSystemUnit::GetMultiplier;
%rename("%s") FbxSystemUnit::GetScaleFactorAsString;
%rename("%s") FbxSystemUnit::GetHashCode;
%rename("%s") FbxSystemUnit::GetConversionFactorTo;
%rename("%s") FbxSystemUnit::GetConversionFactorFrom;
%rename("%s") FbxSystemUnit::ConvertScene(FbxScene* pScene, const ConversionOptions& pOptions=DefaultConversionOptions) const;
%rename("%s") FbxSystemUnit::ConversionOptions;
%rename("%s") FbxSystemUnit::ConversionOptions::ConversionOptions;
%rename("%s") FbxSystemUnit::ConversionOptions::~ConversionOptions;
%rename("%s") FbxSystemUnit::ConversionOptions::mConvertRrsNodes;
#endif

/* Define equality and hash code. */
%define_equality_from_operator(FbxSystemUnit);
%extend FbxSystemUnit {
  %csmethodmodifiers GetHashCode "public override";
  int GetHashCode() const {
    union {
      uint64_t uScale;
      double dScale;
    };
    union {
      uint64_t uMultiplier;
      double dMultiplier;
    };
    dScale = $self->GetScaleFactor();
    dMultiplier = $self->GetMultiplier();
    /* Combine the two doubles into 32 bits by rotating by 8 bits at a time and
     * XORing 32 bits of the doubles at a time. */
    uint32_t u = (uint32_t) (uScale >> 32);
    u = (u << 8) | (u >> 24);
    u ^= (uint32_t) uScale;
    u = (u << 8) | (u >> 24);
    u ^= (uint32_t) (uMultiplier >> 32);
    u = (u << 8) | (u >> 24);
    u ^= (uint32_t) uMultiplier;
    return (int)u;
  }
}

/* Define ToString */
%extend FbxSystemUnit {
  %proxycode %{
  public override string ToString() {
    var unitName = GetScaleFactorAsString();
    if (unitName == "custom unit") {
      unitName += string.Format(" ({0} cm)", GetScaleFactor());
    }
    var multiplier = GetMultiplier();
    if (multiplier != 1.0) {
      unitName += string.Format(" multiplier {0}", multiplier);
    }
    return unitName;
  }
%} }

%include "fbxsdk/core/fbxsystemunit.h"
