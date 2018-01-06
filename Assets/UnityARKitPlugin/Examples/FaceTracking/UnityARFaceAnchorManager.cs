using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class UnityARFaceAnchorManager : MonoBehaviour {

	[SerializeField]
	private GameObject anchorPrefab;

	private UnityARSessionNativeInterface m_session;

	// Use this for initialization
	void Start () {
		m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

		#if !UNITY_EDITOR
		Application.targetFrameRate = 60;
		ARKitFaceTrackingConfiguration config = new ARKitFaceTrackingConfiguration();
		config.alignment = UnityARAlignment.UnityARAlignmentGravity;
		config.enableLightEstimation = true;

		if (config.IsSupported ) {
			
			m_session.RunWithConfig (config);

			UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
			UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
			UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;

		}

		#else 
		//put some defaults so that it doesnt complain
		UnityARCamera scamera = new UnityARCamera ();
		scamera.worldTransform = new UnityARMatrix4x4 (new Vector4 (1, 0, 0, 0), new Vector4 (0, 1, 0, 0), new Vector4 (0, 0, 1, 0), new Vector4 (0, 0, 0, 1));
		Matrix4x4 projMat = Matrix4x4.Perspective (60.0f, 1.33f, 0.1f, 30.0f);
		scamera.projectionMatrix = new UnityARMatrix4x4 (projMat.GetColumn(0),projMat.GetColumn(1),projMat.GetColumn(2),projMat.GetColumn(3));

		UnityARSessionNativeInterface.SetStaticCamera (scamera);
		UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
		UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
		UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;

		#endif

	}

	void FaceAdded (ARFaceAnchor anchorData)
	{
		anchorPrefab.transform.position = UnityARMatrixOps.GetPosition (anchorData.transform);
		anchorPrefab.transform.rotation = UnityARMatrixOps.GetRotation (anchorData.transform);
		anchorPrefab.SetActive (true);
	}

	void FaceUpdated (ARFaceAnchor anchorData)
	{
		anchorPrefab.transform.position = UnityARMatrixOps.GetPosition (anchorData.transform);
		anchorPrefab.transform.rotation = UnityARMatrixOps.GetRotation (anchorData.transform);
	}

	void FaceRemoved (ARFaceAnchor anchorData)
	{
		anchorPrefab.SetActive (false);
	}



	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy()
	{
		
	}
}
