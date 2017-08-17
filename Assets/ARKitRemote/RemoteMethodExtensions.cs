using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace UnityEngine.XR.iOS
{
	public static class RemoteMethodExtensions
	{
		//extension method for UnityARSessionNativeInterface that will do the hittest on Editor
		public static List<ARHitTestResult> HitTest(this UnityARSessionNativeInterface uarsni, ARPoint point, ARHitTestResultType types)
		{
			return RemoteHitTestManager.globalHitTestManager.RemoteHitTest (point, types);
		}
	}
}
#endif

