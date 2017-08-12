using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentScaleManager : MonoBehaviour {

	public delegate void ContentScaleChanged(Vector3 scale); 
	public static ContentScaleChanged ContentScaleChangedEvent;

	[SerializeField]
	private Vector3 m_ContentScale = Vector3.one;
	
	public Vector3 ContentScale {
			get { return m_ContentScale; }
			set { 
				if (value != m_ContentScale)
				{
					m_ContentScale = value;
					if (ContentScaleChangedEvent != null)
					{
						gameObject.transform.localScale = value;
						ContentScaleChangedEvent(m_ContentScale);
					}
				}
		}

	}
}
