using UnityEngine;
using UnityEditor;

namespace KrablMesh {
	public class ProcessorEditor : Editor {
		public bool usedByMeshImporter = false; // used to change appearance when used as realtime processor.
		public bool modifiedDuringLastUpdate = false; // used to collect information about editor changes for multiple processors.
					
		// Shown as tooltip on the processor title in import programs.
		virtual public string ProcessorToolTip() {
			return "";
		}
	}
}