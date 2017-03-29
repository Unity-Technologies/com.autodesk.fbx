#include <fbxsdk.h>

#include <stdio.h>
#include <algorithm>
#include <unordered_map>

namespace Allocator {
    std::unordered_map<void*, size_t> gAllocations;

    void *mymalloc(size_t size) {
        void *p = ::malloc(size);
        gAllocations[p]++;
        printf("Allocated %llx => %lu bytes\n", (uint64_t)p, size);
        return p;
    }
    void myfree(void *ptr) {
        gAllocations[ptr]--;
        printf("Freed     %llx\n", (uint64_t)ptr);
        return ::free(ptr);
    }
    void *mycalloc(size_t count, size_t size) {
        void *p = ::calloc(count, size);
        gAllocations[p]++;
        printf("Allocated %llx => %llu zeroed\n", (uint64_t)p, (uint64_t)(size * count));
        return p;
    }
    void *myrealloc(void *ptr, size_t size) {
        void *newPtr = ::realloc(ptr, size);
        if (ptr != newPtr) {
            if (ptr) gAllocations[ptr]--;
            gAllocations[newPtr]++;
        }
        printf("Realloc   %llx from %llx, now size %lld\n", (uint64_t)newPtr, (uint64_t)ptr, (uint64_t)size);
        return newPtr;
    }
    void report()
    {
        bool printed = false;
        for(const auto& kvp : gAllocations) {
            if (kvp.second != 0) {
                printf("[%llx] net allocations: %lld\n", (uint64_t)kvp.first, (int64_t)kvp.second);
                printed = true;
            }
        }
        if (!printed) {
            printf("All allocations were freed properly.");
        }
    }
}

///////////////
// All the below is copied from docs "your first fbx sdk program"
// Except the setting up of malloc.
using namespace fbxsdk;

/* Tab character ("\t") counter */
int numTabs = 0; 

/**
 * Print the required number of tabs.
 */
void PrintTabs() {
    for(int i = 0; i < numTabs; i++)
        printf("\t");
}

/**
 * Return a string-based representation based on the attribute type.
 */
FbxString GetAttributeTypeName(FbxNodeAttribute::EType type) { 
    switch(type) { 
        case FbxNodeAttribute::eUnknown: return "unidentified"; 
        case FbxNodeAttribute::eNull: return "null"; 
        case FbxNodeAttribute::eMarker: return "marker"; 
        case FbxNodeAttribute::eSkeleton: return "skeleton"; 
        case FbxNodeAttribute::eMesh: return "mesh"; 
        case FbxNodeAttribute::eNurbs: return "nurbs"; 
        case FbxNodeAttribute::ePatch: return "patch"; 
        case FbxNodeAttribute::eCamera: return "camera"; 
        case FbxNodeAttribute::eCameraStereo: return "stereo"; 
        case FbxNodeAttribute::eCameraSwitcher: return "camera switcher"; 
        case FbxNodeAttribute::eLight: return "light"; 
        case FbxNodeAttribute::eOpticalReference: return "optical reference"; 
        case FbxNodeAttribute::eOpticalMarker: return "marker"; 
        case FbxNodeAttribute::eNurbsCurve: return "nurbs curve"; 
        case FbxNodeAttribute::eTrimNurbsSurface: return "trim nurbs surface"; 
        case FbxNodeAttribute::eBoundary: return "boundary"; 
        case FbxNodeAttribute::eNurbsSurface: return "nurbs surface"; 
        case FbxNodeAttribute::eShape: return "shape"; 
        case FbxNodeAttribute::eLODGroup: return "lodgroup"; 
        case FbxNodeAttribute::eSubDiv: return "subdiv"; 
        default: return "unknown"; 
    } 
}

/**
 * Print an attribute.
 */
void PrintAttribute(FbxNodeAttribute* pAttribute) {
    if(!pAttribute) return;
 
    FbxString typeName = GetAttributeTypeName(pAttribute->GetAttributeType());
    FbxString attrName = pAttribute->GetName();
    PrintTabs();
    // Note: to retrieve the character array of a FbxString, use its Buffer() method.
    printf("<attribute type='%s' name='%s'/>\n", typeName.Buffer(), attrName.Buffer());
}

/**
 * Print a node, its attributes, and all its children recursively.
 */
void PrintNode(FbxNode* pNode) {
    PrintTabs();
    const char* nodeName = pNode->GetName();
    FbxDouble3 translation = pNode->LclTranslation.Get(); 
    FbxDouble3 rotation = pNode->LclRotation.Get(); 
    FbxDouble3 scaling = pNode->LclScaling.Get();

    // Print the contents of the node.
    printf("<node name='%s' translation='(%f, %f, %f)' rotation='(%f, %f, %f)' scaling='(%f, %f, %f)'>\n", 
        nodeName, 
        translation[0], translation[1], translation[2],
        rotation[0], rotation[1], rotation[2],
        scaling[0], scaling[1], scaling[2]
        );
    numTabs++;

    // Print the node's attributes.
    for(int i = 0; i < pNode->GetNodeAttributeCount(); i++)
        PrintAttribute(pNode->GetNodeAttributeByIndex(i));

    // Recursively print the children.
    for(int j = 0; j < pNode->GetChildCount(); j++)
        PrintNode(pNode->GetChild(j));

    numTabs--;
    PrintTabs();
    printf("</node>\n");
}

/**
 * Main function - loads the hard-coded fbx file,
 * and prints its contents in an xml format to stdout.
 */
int main(int argc, char** argv) {
    if (argc > 1) {
        puts("Setting up custom allocators");
        fbxsdk::FbxSetMallocHandler(Allocator::mymalloc);
        fbxsdk::FbxSetFreeHandler(Allocator::myfree);
        fbxsdk::FbxSetCallocHandler(Allocator::mycalloc);
        fbxsdk::FbxSetReallocHandler(Allocator::myrealloc);
    }

    // Change the following filename to a suitable filename value.
    const char* lFilename = "file.fbx";
    
    puts("// Initialize the SDK manager. This object handles all our memory management.");
    FbxManager* lSdkManager = FbxManager::Create();
    
    puts("// Create the IO settings object.");
    FbxIOSettings *ios = FbxIOSettings::Create(lSdkManager, IOSROOT);
    lSdkManager->SetIOSettings(ios);

    puts("// Create an importer using the SDK manager.");
    FbxImporter* lImporter = FbxImporter::Create(lSdkManager,"");
    
    // Use the first argument as the filename for the importer.
    if(!lImporter->Initialize(lFilename, -1, lSdkManager->GetIOSettings())) { 
        printf("Call to FbxImporter::Initialize() failed.\n"); 
        printf("Error returned: %s\n\n", lImporter->GetStatus().GetErrorString()); 
        exit(-1); 
    }
    
    puts("// Create a new scene so that it can be populated by the imported file.");
    FbxScene* lScene = FbxScene::Create(lSdkManager,"myScene");

    puts("// Import the contents of the file into the scene.");
    lImporter->Import(lScene);

    puts("// The file is imported; so get rid of the importer.");
    lImporter->Destroy();
    
    puts("// Print the nodes of the scene and their attributes recursively.");
    // Note that we are not printing the root node because it should
    // not contain any attributes.
    FbxNode* lRootNode = lScene->GetRootNode();
    if(lRootNode) {
        for(int i = 0; i < lRootNode->GetChildCount(); i++)
            PrintNode(lRootNode->GetChild(i));
    }

    //////// new code... //////
    // create a node and destroy it immediately, to test whether that shows up as alloc/free
    {
        puts("// Get the root node of the scene.");
        FbxNode* lRootNode = lScene->GetRootNode();

        puts("// Create a child node.");
        FbxNode* lNode = FbxNode::Create(lScene, "child");

        puts("// Add the child to the root node.");
        lRootNode->AddChild(lNode);

        puts("// Destroy it.");
        lNode->Destroy();
    }
    
    //////// ...end new code //////


    puts("// Destroy the SDK manager and all the other objects it was handling.");
    lSdkManager->Destroy();
    Allocator::report();
    return 0;
}
