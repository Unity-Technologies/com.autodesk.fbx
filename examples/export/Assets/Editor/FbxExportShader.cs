// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/* Create a simple mesh with a simple shader. */
using UnityEngine;
using UnityEditor;

namespace FbxSdk.Examples.Editor
{
    public static class ShaderExporter {
        [MenuItem ("FbxSharp Examples/Export Shaded Plane")]
        public static void ExportShader()
        {
            // Find an export filename
            var filePath = EditorUtility.SaveFilePanel ("Export fbx name", Application.dataPath, "shaded-plane.fbx", "");
            if (filePath == "") { return; }

            using (var mgr = FbxManager.Create()) {
                using (var scene = FbxScene.Create(mgr, "scene")) {
                    // We're using meters.
                    scene.GetGlobalSettings().SetSystemUnit(FbxSystemUnit.m);

                    // Create the scene.
                    var node = CreateMesh(scene.GetRootNode(), new Vector3(0, 1, 0));
                    var mat = CreateShader(scene);
                    node.AddMaterial(mat);
                    Debug.Log("created material " + mat);

                    // Export it.
                    using (var exporter = FbxExporter.Create(mgr, "exporter")) {
                        exporter.Initialize(filePath);
                        exporter.SetFileExportVersion("FBX201400"); // force compatibility with old apps
                        exporter.Export(scene);
                    }
                }
            }
        }

        static FbxNode CreateMesh(FbxObject parent, Vector3 location) {
            var node = FbxNode.Create(parent, "mesh at " + location.ToString());
            node.LclTranslation.Set(new FbxDouble3(location.x, location.y, location.z));

            var mesh = FbxMesh.Create(node, "mesh at " + location.ToString());
            mesh.InitControlPoints(4);
            mesh.SetControlPointAt(new FbxVector4(-1, 0, -1), 0);
            mesh.SetControlPointAt(new FbxVector4( 1, 0, -1), 1);
            mesh.SetControlPointAt(new FbxVector4( 1, 0,  1), 2);
            mesh.SetControlPointAt(new FbxVector4(-1, 0,  1), 3);

            mesh.BeginPolygon();
            mesh.AddPolygon(0);
            mesh.AddPolygon(1);
            mesh.AddPolygon(2);
            mesh.AddPolygon(3);
            mesh.EndPolygon();

            node.SetNodeAttribute(mesh);
            return node;
        }

        static FbxSurfaceMaterial CreateShader(FbxObject parent) {
            var mat = FbxSurfaceMaterial.Create(parent, "MyExportShader");
            var impl = FbxImplementation.Create(mat, "impl");

            impl.RenderAPI.Set(Globals.FBXSDK_RENDERING_API_OPENGL);
            impl.RenderAPIVersion.Set("1.5");
            impl.Language.Set(Globals.FBXSDK_SHADING_LANGUAGE_GLSL);
            impl.LanguageVersion.Set("1.5");

            // Create the bindings.
            impl.RootBindingName.Set("table");
            var table = impl.AddNewTable("table", "shader");
            table.DescRelativeURL.Set("fbxexportshader.glsl");

            // Create the property tree. We need to have it for blender, though it needn't have any
            // properties inside.
            var props = FbxProperty.Create(mat, Globals.FbxCompoundDT, "shaderprops");

            // Create a custom property inside.
            var customprop = FbxProperty.Create(props, Globals.FbxFloatDT, "customprop");
            customprop.Set(5.0f);

            // Add it to the bindings table; bind the semantic name (that the shader sees) to
            // the fbx property.
            var custompropentry = table.AddNewEntry();
            var entryAsProp = new FbxPropertyEntryView(custompropentry, true, true);
            var entryAsSemantic = new FbxSemanticEntryView(custompropentry, false, true);
            entryAsProp.SetProperty(customprop.GetHierarchicalName());
            entryAsSemantic.SetSemantic(customprop.GetName());

            // Set the material implementation and return the material.
            mat.AddImplementation(impl);
            mat.SetDefaultImplementation(impl);
            return mat;
        }

    }
}
