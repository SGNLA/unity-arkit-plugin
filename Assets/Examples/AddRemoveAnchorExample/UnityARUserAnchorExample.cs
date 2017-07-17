using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.iOS;

/**
	This Class will place a game object with an UnityARUserAnchorComponent attached to it.
	It will then call the RemoveAnchor API after 5 seconds. This scipt will subscribe to the
	AnchorRemoved event and remove the game object from the scene.
 */
public class UnityARUserAnchorExample : MonoBehaviour {


	public GameObject prefabObject;
	// Distance in Meters
	public int distanceFromCamera = 1;
	private HashSet<string> m_Clones;


	private float m_TimeUntilRemove = 5.0f;

	void  Awake() {
		UnityARSessionNativeInterface.ARAnchorAddedEvent += ExampleAddAnchor;
		UnityARSessionNativeInterface.ARAnchorRemovedEvent += AnchorRemoved;
		m_Clones = new HashSet<string>();
	}
	
	public void ExampleAddAnchor(ARPlaneAnchor anchor)
	{
		Console.WriteLine("AnchorAddedExample: " + anchor.identifier);
		if (m_Clones.Contains(anchor.identifier))
		{
            Console.WriteLine("Our anchor was added!");
		}
	}

	public void AnchorRemoved(ARPlaneAnchor anchor)
	{
		if (m_Clones.Contains(anchor.identifier))
		{
            m_Clones.Remove(anchor.identifier);
            Console.WriteLine("AnchorRemovedExample: " + anchor.identifier);
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			Console.WriteLine("Before Instantiation");
			GameObject clone = Instantiate(prefabObject, Camera.main.transform.position + (this.distanceFromCamera * Camera.main.transform.forward), Quaternion.identity);
			Console.WriteLine("After Instantiation");
			UnityARUserAnchorComponent component = clone.GetComponent<UnityARUserAnchorComponent>();
			m_Clones.Add(component.AnchorId);
			Console.WriteLine("Added Component to Game Object: " + component.AnchorId);
			m_TimeUntilRemove = 4.0f;
		}

		// just remove anchors afte a certain amount of time for example's sake.
		m_TimeUntilRemove -= Time.deltaTime;
		if (m_TimeUntilRemove <= 0.0f)
		{
            foreach (string id in m_Clones)
            {
                Console.WriteLine("Removing anchor with id: " + id);
                UnityARSessionNativeInterface.GetARSessionNativeInterface().RemoveAnchor(id);
                break;
            }
            m_TimeUntilRemove = 4.0f;
		}
	}
}
