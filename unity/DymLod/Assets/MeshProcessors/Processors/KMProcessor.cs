using UnityEngine;

namespace KrablMesh {	
	public class Processor : ScriptableObject {
		public bool enabled = true;
		
		protected string platformID = null; // set during processing so select the correct values
		
		virtual public string Name() { return "Processor"; }	
		
		virtual public void Calculate(ref KrablMesh.MeshEdges mesh, KMProcessorProgram parentProgram = null) {
			ReportProgress("", 0.0f); 	
		}
		
		virtual public bool IsPlatformDependent() { return false; }
		
		public void SetBuildPlatform(string pid) { platformID = pid; }
		
		public delegate void ProcessorProgressDelegate(string text, float val);
		
		public ProcessorProgressDelegate progressDelegate = null;
		
		
		// progress 0...1
		protected void ReportProgress(string text, float val) {
			if (progressDelegate != null) {
				string txt = Name();
				if (text != null && text != "") txt += ": " + text;
				progressDelegate(txt, val);
			}
		}
		
		void Start() {
		}
	}
}