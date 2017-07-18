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
		UnityARSessionNativeInterface.ARUserAnchorUpdatedEvent += GameObjectAnchorUpdated;
		UnityARSessionNativeInterface.ARUserAnchorRemovedEvent += AnchorRemoved;
		this.m_AnchorId = UnityARSessionNativeInterface.GetARSessionNativeInterface ().AddUserAnchorFromGameObject(this.gameObject).identifierStr; 
		Console.WriteLine("UnityARUserAnchorComponent.Start(): " + m_AnchorId);
	}
	void Start () {

	}

	public void AnchorRemoved(ARUserAnchor anchor)
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
		UnityARSessionNativeInterface.ARUserAnchorUpdatedEvent -= GameObjectAnchorUpdated;
		UnityARSessionNativeInterface.ARUserAnchorRemovedEvent -= AnchorRemoved;
		Console.WriteLine("OnDestroy: " + this.m_AnchorId);
		UnityARSessionNativeInterface.GetARSessionNativeInterface ().RemoveUserAnchor(this.m_AnchorId); 
    }

	private void GameObjectAnchorUpdated(ARUserAnchor anchor)
	{
		
	}
}
}