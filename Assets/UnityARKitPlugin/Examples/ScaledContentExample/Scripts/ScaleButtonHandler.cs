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
		if (Mathf.Abs(scaleFactor) >= m_ContentScaleManager.ContentScale)
			scaleFactor *= 0.1f;
		else if (Mathf.Abs(scaleFactor) * 10.0f < m_ContentScaleManager.ContentScale )
			scaleFactor *= 10.0f;
		
		m_ContentScaleManager.ContentScale = m_ContentScaleManager.ContentScale +  scaleFactor;
	}
}
