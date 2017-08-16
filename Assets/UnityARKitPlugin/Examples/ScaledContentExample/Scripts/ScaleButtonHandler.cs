using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleButtonHandler : MonoBehaviour {


	public Button button;
	public float scaleFactor = 1;
	public GameObject contentScaleManager;
	private ContentScaleManager m_ContentScaleManager;

	void Start()
	{
		button.onClick.AddListener(OnClick);
		m_ContentScaleManager = contentScaleManager.GetComponent<ContentScaleManager>();
	}
	
	public void OnClick()
	{
		m_ContentScaleManager.ContentScale = m_ContentScaleManager.ContentScale + new Vector3(scaleFactor, scaleFactor, scaleFactor);
	}
}
