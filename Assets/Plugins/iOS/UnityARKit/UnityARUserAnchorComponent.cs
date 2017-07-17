using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace UnityEngine.XR.iOS
{
public class UnityARUserAnchorComponent : MonoBehaviour {

    private string m_AnchorId;

	public string AnchorId  { get { return m_AnchorId; } }

	void Awake()
	{
		UnityARSessionNativeInterface.ARAnchorUpdatedEvent += GameObjectAnchorUpdated;
		UnityARSessionNativeInterface.ARAnchorRemovedEvent += AnchorRemoved;
		Console.WriteLine("Start in UnityARUserAnchorComponent");
		this.m_AnchorId = UnityARSessionNativeInterface.GetARSessionNativeInterface ().AddAnchorFromGameObject(this.gameObject).identifierStr; 
		Console.WriteLine("UnityARUserAnchorComponent.Start(): " + m_AnchorId);
	}
	void Start () {

	}

	public void AnchorRemoved(ARPlaneAnchor anchor)
	{
		Console.WriteLine("Anchor Being Removed: " + anchor.identifier);
		Console.WriteLine("Our Anchor ID: " + m_AnchorId);
		if (anchor.identifier.Equals(m_AnchorId))
		{
			Console.WriteLine("AnchorComponent.AnchorRemoved: " + m_AnchorId);
			Destroy(this.gameObject);
		}
	}

    void OnDestroy() {
		UnityARSessionNativeInterface.ARAnchorUpdatedEvent -= GameObjectAnchorUpdated;
		UnityARSessionNativeInterface.ARAnchorRemovedEvent -= AnchorRemoved;
		Console.WriteLine("OnDestroy: " + this.m_AnchorId);
		UnityARSessionNativeInterface.GetARSessionNativeInterface ().RemoveAnchor(this.m_AnchorId); 
    }

	private void GameObjectAnchorUpdated(ARPlaneAnchor anchor)
	{
		
	}
}
}