using System.Collections.Generic;
using UnityEngine;

namespace FbxSdk.Examples
{
    public class FbxSdkExporter : MonoBehaviour
    {
        private static string msgPrefix { get { return "FbxSdkCSharp"; }}

        [Tooltip ("Filename to export into.")]
        public string DestinationFilename;

        [Tooltip ("If false, we always use the ExportMethod below. If true, we'll guess at what export method to use based on the filename extension, and only default to ExportMethod if we can't guess.")]
        public bool GuessExportMethod = true;

        [Tooltip ("Method of export. Must be one of the types in the export method registry.")]
        public string ExportMethod = "fbx";

        [Tooltip ("Should we create an empty file if there's nothing to export?")]
        public bool ExportEvenIfEmpty = false;

        public UnityEngine.Object [] ObjectsToExport;

        /// <summary>
        /// Starts the export at the current object.
        /// All the filenames and etc should be set already or you might be sad.
        /// This function is intended to be set up at the callback for a GUI element.
        /// </summary>
        public void Export ()
        {
            ExportMethod method = null;

            if (GuessExportMethod) {
                method = FBXExporter.StaticExportMethod;
            }

            if (method == null) {
                method = FBXExporter.StaticExportMethod;
            }

            if (method == null) {
                string msg = string.Format ("{0}: Unknown export method {1}", msgPrefix, System.IO.Path.GetExtension (DestinationFilename));
                Debug.LogWarning (msg);
                return;
            }

            if (!ExportEvenIfEmpty) {
                if (TestIfExportIsEmpty (msgPrefix, ObjectsToExport)) {
                    return;
                }
            }

            var temp = System.IO.Path.GetExtension (DestinationFilename).TrimStart ('.').ToLower ();
            if (!temp.Equals (method.Extension)) {
                DestinationFilename += "." + method.Extension;
            }

            using (var exporter = method.Instantiate (DestinationFilename, 1)) {
                ExportAll (exporter, ObjectsToExport, warn: false);
            }
        }

        /// <summary>
        /// Sets the filename. This should be a full path.
        /// </summary>
        /// <param name="name">Full path of the file we'll export into.</param>
        public void SetFilename (string name)
        {
            DestinationFilename = name;
        }

        /// <summary>
        /// Tests if what we want to export is in fact an empty mesh.
        /// 
        /// Log a warning to the log if it is.
        /// 
        /// Return true if empty, false if there's at least one triangle to export.
        /// </summary>
        static public bool TestIfExportIsEmpty (string dialogTitle, System.Collections.Generic.IEnumerable<UnityEngine.Object> objectsToExport)
        {
            // No, the export is not empty.
            return false;
        }

        /// <summary>
        /// Export all the objects in the set.
        /// Return the number of objects in the set that we exported.
        /// </summary>
        static public int ExportAll (AbstractExporter exporter, IEnumerable<UnityEngine.Object> exportSet, bool warn = true)
        {
            exporter.BeginExport ();
            
            int n = 0;
            foreach (var obj in exportSet) {
                ++n;
                if (obj is UnityEngine.Transform) {
                    var xform = obj as UnityEngine.Transform;
                    exporter.Export (xform.gameObject);
                } else if (obj is UnityEngine.GameObject) {
                    exporter.Export (obj as UnityEngine.GameObject);
                } else if (obj is MonoBehaviour) {
                    var mono = obj as MonoBehaviour;
                    exporter.Export (mono.gameObject);
                } else {
                    if (warn) {
                        if (obj != null) {
                            string msg = string.Format ("{0}:Not exporting object of type {1} ({2})", msgPrefix, obj.GetType (), obj.name);
                            Debug.LogWarning (msg);
                        } else {
                            string msg = string.Format ("{0}: Not exporting null object", msgPrefix);
                            Debug.LogWarning (msg);
                        }
                    }
                    --n;
                }
            }

            exporter.EndExport ();

            return n;
        }
    }
}