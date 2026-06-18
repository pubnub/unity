using UnityEditor;

namespace PubNubUnity.Internal {
	[CustomEditor(typeof(PNDispatcher))]
	public class PNDispatcherEditor : Editor {
		public class DispatcherEditor : Editor {
        		public override void OnInspectorGUI() {
        			EditorGUILayout.HelpBox("This script allows dispatching to the main Unity render thread", MessageType.Info);
        		}
        	}
	}
}