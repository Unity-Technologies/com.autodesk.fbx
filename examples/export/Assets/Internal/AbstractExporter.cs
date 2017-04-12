using System;

using UnityEngine;
using System.Collections.Generic;

namespace FbxSdk.Examples
{
    public abstract class AbstractExporter : System.IDisposable
    {
        List<GameObject> m_stack = new List<GameObject> ();
        HashSet<GameObject> m_visited = new HashSet<GameObject> ();
        
        /// <summary>
        /// Called automatically when the mesh exporter is used in a "using" block.
        /// Typically you'd use this to close files you are writing to.
        /// </summary>
        public abstract void Dispose ();

        /// <summary>
        /// Gets the export settings from the constructor.
        /// </summary>
        protected ExportSettings Settings { get; private set; }

        /// <summary>
        /// Initialize a new exporter with the given settings.
        /// 
        /// You can play games with the settings: the export markers believe whatever you set in the settings,
        /// even if it doesn't match the actual export method.
        /// </summary>
        protected AbstractExporter (ExportSettings settings)
        {
        	Settings = settings;
        }

        /// <summary>
        /// Export a single object.
        /// </summary>
        public void Export (GameObject gameObject)
        {
        	m_stack.Add (gameObject);
        	DFS ();
        }

        /// <summary>
        /// Export a set of objects.
        /// </summary>
        public void Export (IEnumerable<GameObject> gameObjects)
        {
        	m_stack.AddRange (gameObjects);
        	DFS ();
        }

        /// <summary>
        /// In depth-first order, process the stack of GameObjects to be exported.
        /// </summary>
        void DFS ()
        {
        	while (m_stack.Count > 0) {
        		var top = m_stack [m_stack.Count - 1];
        		m_stack.RemoveAt (m_stack.Count - 1);

        		if (!top.activeInHierarchy) {
        			continue;
        		}

        		if (!m_visited.Add (top)) {
        			// Already exported? Then we don't look at it again.
        			continue;
        		}

                bool exportSelf = true;
                bool exportChildren = true;

        		if (exportChildren) {
        			var xform = top.transform;
        			for (int i = 0, n = xform.childCount; i < n; ++i) {
        				m_stack.Add (top.transform.GetChild (i).gameObject);
        			}
        		}

                // Now export the object itself.
                if (exportSelf) {
                    ExportComponents (top);
                }

        	}
        }

        /// <summary>
        /// do stuff before we start exporting the hierarchy
        /// </summary>
        public virtual void BeginExport () {}

        /// <summary>
        /// finish up after the hierarchy has been exported
        /// </summary>
        public virtual void EndExport () {}

        /// <summary>
        /// Unconditionally export this object to the file.
        /// We have decided; this object is definitely getting exported.
        /// </summary>
        protected abstract void ExportComponents (GameObject gameObject);

    }
}