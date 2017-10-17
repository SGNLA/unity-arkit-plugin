using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class BlendshapePrinter : MonoBehaviour {

	bool enabled = false;
	Dictionary<ARBlendShapeLocation, float> currentBlendShapes;

	// Use this for initialization
	void Start () {
		UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
		UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
		UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;

	}

	void OnGUI()
	{
		if (enabled) {

			string blendshapes = "";
			foreach(KeyValuePair<ARBlendShapeLocation,float> kvp in currentBlendShapes) {
				blendshapes += " [";
				blendshapes += kvp.Key.ToString ();
				blendshapes += ":";
				blendshapes += kvp.Value.ToString ();
				blendshapes += "] ";
			}

			GUILayout.Box (blendshapes);

		}
	}

	void FaceAdded (ARFaceAnchor anchorData)
	{
		enabled = true;
		currentBlendShapes = anchorData.blendShapes;
	}

	void FaceUpdated (ARFaceAnchor anchorData)
	{
		currentBlendShapes = anchorData.blendShapes;
	}

	void FaceRemoved (ARFaceAnchor anchorData)
	{
		enabled = false;
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
