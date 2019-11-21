using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autodesk.Fbx;

public class MiniFbxImporter : MonoBehaviour
{
    [System.Serializable]
    struct Settings
    {
        public string m_FileName;
        public enum Conversion
        {
            Legacy,
            Deep,
            None
        }
        public Conversion m_Conversion;

        public bool Equals(Settings other)
        {
            return m_FileName == other.m_FileName 
                && m_Conversion == other.m_Conversion;
        }
    }

    [SerializeField] Settings m_Settings;
    Settings m_OldSettings;

    [SerializeField] Material m_defaultMaterial;
    Material DefaultMaterial
    {
        get
        {
            if (!m_defaultMaterial)
            {
                var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                m_defaultMaterial = quad.GetComponent<MeshRenderer>().sharedMaterial;
                DestroyImmediate(quad);
            }
            return m_defaultMaterial;
        }
    }

    public double m_time = 0;
    public double m_timeScale = 1;

    [Tooltip("Apply FBX animation, clobbering transforms each Update")]
    public bool m_animate = true;

    [Tooltip("Apply FBX limits to transforms each LateUpdate")]
    public bool m_applyLimits = false;

    FbxManager m_fbxManager;

    ~MiniFbxImporter()
    {
        m_fbxManager.Dispose();
    }

    Dictionary<GameObject, FbxNode> m_nodes = new Dictionary<GameObject, FbxNode>();

    // Update is called once per frame
    void Update()
    {
        // If we need to re-import, do that.
        if (!m_Settings.Equals(m_OldSettings)) {
            // Try to import; only nuke the old stuff if we succeed.
            var newManager = FbxManager.Create();
            var newScene = FbxScene.Create(newManager, "");
            var newNodes = TryImport(m_Settings, newScene);
            if (newNodes != null) {
                // success; zero out the time, destroy what we previously
                // imported, save the settings so we can compare
                m_time = 0;
                m_OldSettings = m_Settings;

                if (m_fbxManager != null) { m_fbxManager.Dispose(); }
                m_fbxManager = newManager;

                foreach (var kvp in m_nodes) { DestroyImmediate(kvp.Key); }
                m_nodes = newNodes;
            }
        }

        // Update the time
        m_time += Time.deltaTime * m_timeScale;
        var fbxtime = FbxTime.FromSecondDouble(m_time);

        // Update the transforms (todo: update other animatable properties)
        if (m_animate) {
            foreach (var kvp in m_nodes) {
                var fbxNode = kvp.Value;
                var unityNode = kvp.Key.transform;
                var mx = fbxNode.EvaluateLocalTransform(fbxtime);

                unityNode.localPosition = V3(mx.GetT());
                unityNode.localRotation = Q(mx.GetQ());
                unityNode.localScale = V3(mx.GetS());
            }
        }
    }

    void LateUpdate()
    {
        if (m_applyLimits) {
            foreach (var kvp in m_nodes) {
                var fbxNode = kvp.Value;
                var unityNode = kvp.Key.transform;

                var tlimits = fbxNode.GetTranslationLimits();
                var rlimits = fbxNode.GetRotationLimits();
                var slimits = fbxNode.GetScalingLimits();
                var t = V3(tlimits.Apply(D3(unityNode.localPosition)));
                var s = V3(slimits.Apply(D3(unityNode.localScale)));

                // For rotation, artists tend to write limits in [-180,180)
                // rather than in [0,360).
                var unityEuler = unityNode.localEulerAngles;
                for(int i = 0; i < 3; ++i) {
                    if (float.IsNaN(unityEuler[i]) || float.IsInfinity(unityEuler[i])) { continue; }
                    while (unityEuler[i] < -180) { unityEuler[i] += 360; }
                    while (unityEuler[i] >= 180) { unityEuler[i] -= 360; }
                }
                var r = V3(rlimits.Apply(D3(unityEuler)));
                
                unityNode.localPosition = t;
                unityNode.localEulerAngles = r;
                unityNode.localScale = s;
            }
        }
    }

    /// <summary>
    /// Try to import into the given scene.
    /// Return the nodes we created.
    /// </summary>
    Dictionary<GameObject, FbxNode> TryImport(Settings settings, FbxScene scene)
    {
        using (var importer = FbxImporter.Create(scene.GetFbxManager(), ""))
        {
            if (!importer.Initialize(settings.m_FileName))
            {
                return null;
            }
            if (!importer.Import(scene))
            {
                return null;
            }
            // Unity: Y-up, right is +X, forward is +Z (left-handed)
            var axes = new FbxAxisSystem(
                    FbxAxisSystem.EUpVector.eYAxis,
                    FbxAxisSystem.EFrontVector.eParityOdd,
                    FbxAxisSystem.ECoordSystem.eLeftHanded);

            // Convert as needed.
            switch(settings.m_Conversion)
            {
                case Settings.Conversion.Legacy:
                    axes.ConvertScene(scene);
                    break;
                // case Settings.Conversion.Deep:
                //     axes.DeepConvertScene(scene);
                //     break;
                case Settings.Conversion.None:
                    break;
            }

            // The root node is this node. We won't set ourselves up to match the FBX root;
            // normally it's just a null.
            var root = scene.GetRootNode();
            var nodes = new Dictionary<GameObject, FbxNode>();
            CreateChildren(this.transform, root, nodes);

            return nodes;
        }
    }

    void CreateChildren(Transform unityParent, FbxNode fbxParent, Dictionary<GameObject, FbxNode> nodes)
    {
        for (int i = 0, n = fbxParent.GetChildCount(); i < n; ++i)
        {
            var fbxChild = fbxParent.GetChild(i);
            var unityChild = CreateNode(unityParent, fbxChild);
            nodes[unityChild.gameObject] = fbxChild;
            CreateChildren(unityChild, fbxChild, nodes);
        }
    }

    static FbxDouble3 D3(Vector3 fbx)
    {
        return new FbxDouble3(fbx.x, fbx.y, fbx.z);
    }

    static Vector3 V3(FbxDouble3 fbx)
    {
        return new Vector3((float)fbx.X, (float)fbx.Y, (float)fbx.Z);
    }

    static Vector3 V3(FbxVector4 fbx)
    {
        return new Vector3((float)fbx.X, (float)fbx.Y, (float)fbx.Z);
    }

    static Quaternion Q(FbxQuaternion q)
    {
        return new Quaternion((float)q.X, (float)q.Y, (float)q.Z, (float)q.W);
    }

    Dictionary<FbxMesh, Mesh> m_meshInstances = new Dictionary<FbxMesh, Mesh>();
    Mesh GetOrCreateInstance(FbxMesh fbxMesh)
    {
        Mesh mesh;
        if (m_meshInstances.TryGetValue(fbxMesh, out mesh))
        {
            return mesh;
        }

        // create the unity mesh vertices
        var nVertices = fbxMesh.GetControlPointsCount();
        var unityVertices = new Vector3[nVertices];
        for (int i = 0; i < nVertices; ++i)
        {
            unityVertices[i] = V3(fbxMesh.GetControlPointAt(i));
        }

        // create the unity mesh triangles. TODO: maybe handle non-triangular faces more intelligently
        var nPoly = fbxMesh.GetPolygonCount();
        var unityTriangles = new List<int>();
        for (int polyIndex = 0; polyIndex < nPoly; ++polyIndex)
        {
            var polySize = fbxMesh.GetPolygonSize(polyIndex);
            if (polySize < 3)
            {
                // ignore lines and points
                continue;
            }

            // Add the first triangle
            unityTriangles.Add(fbxMesh.GetPolygonVertex(polyIndex, 0));
            unityTriangles.Add(fbxMesh.GetPolygonVertex(polyIndex, 1));
            unityTriangles.Add(fbxMesh.GetPolygonVertex(polyIndex, 2));

            // If there's more triangles, assume they're a fan around the first vertex.
            // Not necessarily true, but them's the breaks.
            for (int i = 3; i < polySize; ++i)
            {
                unityTriangles.Add(fbxMesh.GetPolygonVertex(polyIndex, 0));
                unityTriangles.Add(fbxMesh.GetPolygonVertex(polyIndex, i - 1));
                unityTriangles.Add(fbxMesh.GetPolygonVertex(polyIndex, i));
            }
        }

        mesh = new Mesh
        {
            name = fbxMesh.GetName(),
            vertices = unityVertices,
            triangles = unityTriangles.ToArray()
        };
        mesh.RecalculateNormals();
        m_meshInstances[fbxMesh] = mesh;
        return mesh;
    }

    Transform CreateNode(Transform parent, FbxNode fbxNode)
    {
        var unityNode = new GameObject(fbxNode.GetName());
        Transform unityTransform = unityNode.transform;
        unityTransform.parent = parent;

        // If there's a mesh attached, create it
        var fbxMesh = fbxNode.GetMesh();
        if (fbxMesh != null)
        {
            var mesh = GetOrCreateInstance(fbxMesh);
            var meshFilter = unityNode.AddComponent<MeshFilter>();
            var meshRender = unityNode.AddComponent<MeshRenderer>();
            meshFilter.sharedMesh = mesh;
            meshRender.material = DefaultMaterial;
        }

        return unityTransform;
    }
}
