// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxBindingTableBase;
%rename("%s") FbxBindingTableBase::Create;
%rename("%s") FbxBindingTableBase::AddNewEntry;

%rename("%s") FbxBindingTable;
%rename("%s") FbxBindingTable::Create;
%rename("%s") FbxBindingTable::DescAbsoluteURL;
%rename("%s") FbxBindingTable::DescTAG;
%rename("%s") FbxBindingTable::AddNewEntry;

%rename("%s") FbxBindingTableEntry;
#endif

/* You're not supposed to create these directly, just with AddNewEntry. */
%ignore FbxBindingTableEntry::FbxBindingTableEntry();
%ignore FbxBindingTableEntry::FbxBindingTableEntry(const FbxBindingTableEntry&);

%include "fbxsdk/scene/shading/fbxbindingtablebase.h"
%include "fbxsdk/scene/shading/fbxbindingtable.h"
%include "fbxsdk/scene/shading/fbxbindingtableentry.h"
