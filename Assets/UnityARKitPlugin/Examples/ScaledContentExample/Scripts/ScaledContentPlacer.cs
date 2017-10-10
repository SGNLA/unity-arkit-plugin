using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScaledContentPlacer : MonoBehaviour
{

    public Transform m_HitTransform;

	private float m_ContentScale;

	void Awake()
	{
		ContentScaleManager.ContentScaleChangedEvent += ContentScaleChanged;
	}

	void ContentScaleChanged(float scale, float prevScale)
	{
		m_ContentScale = scale;
		UpdateScaledContent(scale, prevScale);
	}

	void UpdateScaledContent(float newScale, float prevScale)
	{
        Vector3 pos = m_HitTransform.position;
		// undo the previous scale and         // apply the new scale
		pos.Scale(new Vector3(newScale / prevScale, newScale / prevScale, newScale / prevScale));
        m_HitTransform.position = pos;
	 }

    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count == 1)
        {
            foreach (var hitResult in hitResults)
            {
                Vector3 pos = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
				m_HitTransform.position = pos * m_ContentScale;
                m_HitTransform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                return true;
            }
        }
        return false;
	}

    void Update()
    {
        if (!IsPointerOverUIObject() && Input.touchCount == 1 && m_HitTransform != null && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var touch = Input.GetTouch(0);
			var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
			ARPoint point = new ARPoint
			{
				x = screenPosition.x,
				y = screenPosition.y
			};

			// prioritize reults types
			ARHitTestResultType[] resultTypes = {
					ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
					// if you want to use infinite planes use this:
					//ARHitTestResultType.ARHitTestResultTypeExistingPlane,
					ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
					ARHitTestResultType.ARHitTestResultTypeFeaturePoint
				};

			foreach (ARHitTestResultType resultType in resultTypes)
			{
				if (HitTestWithResultType(point, resultType))
				{
					return;
				}
			}
        }
    }

    // Taken from http://answers.unity3d.com/questions/1115464/ispointerovergameobject-not-working-with-touch-inp.html#answer-1115473
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
