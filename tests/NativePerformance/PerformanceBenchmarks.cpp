// PerformanceBenchmarks.cpp : Defines the entry point for the console application.
//

#include <chrono>
#include <iostream>
#include <sstream>  
#include <string>
#include <unordered_map>
#include "fbxsdk.h"

using namespace std;
using namespace std::chrono;

struct ResultJson {
	string testName;
	double result;
	bool success;
	string error = "";

	string toString() {
		stringstream ss;

		ss << "{\"testName\":\"" << testName << "\",";
		ss << "\"result\":" << result << ",";
		ss << "\"success\":" << success;

		if (error.empty()) {
			ss << "}";
		}
		else {
			ss << "\"error\":" << error << "}";
		}

		return ss.str();
	}
};

string FbxObjectCreateTest(int n) {
	ResultJson json;

	int N = n < 0? 5000 : n;

	FbxManager* manager = FbxManager::Create();

	high_resolution_clock::time_point t1 = high_resolution_clock::now();
	for (int i = 0; i < N; i++) {
		FbxObject::Create(manager, "");
	}
	high_resolution_clock::time_point t2 = high_resolution_clock::now();

	auto duration = duration_cast<milliseconds>(t2 - t1).count();

	manager->Destroy();

	json.testName = "FbxObjectCreate";
	json.result = duration;
	json.success = true;

	return json.toString();
}

string EmptyExportImportTest(int n) {
	ResultJson json;

	json.testName = "EmptyExportImport";

	int N = n < 0? 10 : n;
	long long total = 0;

	FbxManager* fbxManager = FbxManager::Create();

	for (int i = 0; i < N; i++) {
		high_resolution_clock::time_point t1 = high_resolution_clock::now();

		FbxIOSettings* ioSettings = FbxIOSettings::Create(fbxManager, IOSROOT);
		fbxManager->SetIOSettings(ioSettings);

		// Create the exporter.
		FbxExporter* exporter = FbxExporter::Create(fbxManager, "");

		char* filename = "test.fbx";

		// Initialize the exporter.
		bool exportStatus = exporter->Initialize(filename, -1, fbxManager->GetIOSettings());

		if (!exportStatus) {
			fbxManager->Destroy();

			json.success = false;
			json.error = "Failed to initialize exporter";
			return json.toString();
		}

		// Create the empty scene to export
		FbxScene* lScene = FbxScene::Create(fbxManager, "myScene");

		// Export the scene
		exporter->Export(lScene);

		exporter->Destroy();

		// Import to make sure file is valid

		// Create an importer.
		FbxImporter* importer = FbxImporter::Create(fbxManager, "");

		// Initialize the importer.
		bool importStatus = importer->Initialize(filename, -1, fbxManager->GetIOSettings());

		if (!importStatus) {
			fbxManager->Destroy();

			json.success = false;
			json.error = "Failed to initialize importer";
			return json.toString();
		}

		// Create a new scene to import to
		FbxScene* newScene = FbxScene::Create(fbxManager, "myScene2");

		// Import the contents of the file into the scene
		importer->Import(newScene);

		importer->Destroy();

		high_resolution_clock::time_point t2 = high_resolution_clock::now();

		auto duration = duration_cast<milliseconds>(t2 - t1).count();

		total += duration;

		// Delete the file once the test is complete
		if (remove(filename) != 0) {
			fbxManager->Destroy();

			json.success = false;
			json.error = "Failed to destroy file";
			return json.toString();
		}
	}

	fbxManager->Destroy();

	json.success = true;
	json.result = total / (double)N;

	return json.toString();
}

typedef string(*FnPtr)(int);

int main(int argc, char *argv[])
{
	// have command line arguments specifying which tests to run
	// output the results as a json string
	// optionally, can include how many samples to take for a test by adding
	// :N after the function name in the command line. e.g. funName:100

	std::unordered_map<std::string, FnPtr> funMap;
	funMap["FbxObjectCreate"] = FbxObjectCreateTest;
	funMap["EmptyExportImport"] = EmptyExportImportTest;

	stringstream ss;

	ss << "{\"tests\": [";
	if (argc <= 1) {
		// run all tests
		for (auto it = funMap.begin(); it != funMap.end(); ++it) {
			if (funMap.begin() != it) {
				ss << ",";
			}
			ss << (it->second)(-1);
		}
	}
	else {
		// run only tests specified on the command line
		for (int i = 1; i < argc; i++) {
			// check if there is a sample number included
			string s = argv[i];
			string delimiter = ":";
			string token = s.substr(0, s.find(delimiter));

			// if the strings are not equal then get the number
			string N = "-1";
			if (s.compare(token) != 0) {
				N = s.substr(s.find(delimiter)+1);
			}

			if (funMap.find(token) == funMap.end()) {
				cout << "Error: Could not find test function: " << argv[i];
				return 0;
			}

			try {
				ss << funMap[token](stoi(N));
			}
			catch (invalid_argument) {
				cout << "Error: Could not convert \"" << N << "\" to an integer";
				return 0;
			}

			if (i < argc - 1) {
				ss << ",";
			}
		}
	}
	ss << "]}";

	// print json so we can pick it up in Unity
	cout << ss.str();

    return 0;
}

