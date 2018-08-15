// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s", %$isclass) FbxBindingTableBase;
%rename("%s") FbxBindingTableBase::AddNewEntry;

%rename("%s", %$isclass) FbxBindingTable;
%rename("%s") FbxBindingTable::Create;
%rename("%s") FbxBindingTable::DescAbsoluteURL;
%rename("%s") FbxBindingTable::DescRelativeURL;
%rename("%s") FbxBindingTable::DescTAG;
%rename("%s") FbxBindingTable::AddNewEntry;

%rename("%s", %$isclass) FbxBindingTableEntry;
%rename("%s") FbxBindingTableEntry::GetName;
%rename("%s") FbxBindingTableEntry::GetHierarchicalName;

%rename("%s", %$isclass) FbxEntryView;
%rename("%s") FbxEntryView::IsValid;
%rename("%s") FbxEntryView::EntryType;

%rename("%s", %$isclass) FbxPropertyEntryView;
%rename("%s") FbxPropertyEntryView::FbxPropertyEntryView;
%rename("%s") FbxPropertyEntryView::~FbxPropertyEntryView;
%rename("%s") FbxPropertyEntryView::SetProperty;
%rename("%s") FbxPropertyEntryView::GetProperty;

%rename("%s", %$isclass) FbxSemanticEntryView;
%rename("%s") FbxSemanticEntryView::FbxSemanticEntryView;
%rename("%s") FbxSemanticEntryView::~FbxSemanticEntryView;
%rename("%s") FbxSemanticEntryView::SetSemantic;
%rename("%s") FbxSemanticEntryView::GetSemantic;
%rename("%s") FbxSemanticEntryView::GetIndex;

#endif

/* You can't create a tablebase -- it's an abstract class */
%ignore FbxBindingTableBase::Create;

/* Properties can't be assigned to */
%immutable FbxBindingTable::DescAbsoluteURL;
%immutable FbxBindingTable::DescRelativeURL;
%immutable FbxBindingTable::DescTAG;

/* You're not supposed to create these directly, just with AddNewEntry. */
%rename("$ignore", %$isconstructor) FbxBindingTableEntry;

%include "fbxsdk/scene/shading/fbxbindingtablebase.h"
%include "fbxsdk/scene/shading/fbxbindingtable.h"
%include "fbxsdk/scene/shading/fbxbindingtableentry.h"
%include "fbxsdk/scene/shading/fbxentryview.h"
%include "fbxsdk/scene/shading/fbxpropertyentryview.h"
%include "fbxsdk/scene/shading/fbxsemanticentryview.h"
