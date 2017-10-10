using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class CameraScaler : MonoBehaviour {

	public Camera scaledCamera;
	public Vector3 scaledObjectOrigin;
	public float cameraScale = 1.0f;

	// Use this for initialization
	void Start () {
		ContentScaleManager.ContentScaleChangedEvent += ContentScaleChanged;
	}

	void ContentScaleChanged(float scale, float prevScale)
	{
		cameraScale = scale;
	}


	void Update() {
		if (scaledCamera != null && cameraScale> 0.0001f && cameraScale < 10000.0f) {
			Matrix4x4 matrix = UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraPose();
			float invScale = 1.0f/cameraScale;
			Vector3 cameraPos = UnityARMatrixOps.GetPosition (matrix);
			Vector3 vecAnchorToCamera =  cameraPos - scaledObjectOrigin;
			scaledCamera.transform.localPosition = scaledObjectOrigin + (vecAnchorToCamera * invScale);
			scaledCamera.transform.localRotation = UnityARMatrixOps.GetRotation (matrix);


			//this needs to be adjusted for near/far
			scaledCamera.projectionMatrix = UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraProjection ();

		}
	}

}
