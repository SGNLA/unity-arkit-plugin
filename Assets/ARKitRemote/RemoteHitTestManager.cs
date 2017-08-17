using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using Utils;

#if UNITY_EDITOR
using UnityEditor.Networking.PlayerConnection;
#endif

namespace UnityEngine.XR.iOS
{

	public class RemoteHitTestManager : MonoBehaviour {

		public static RemoteHitTestManager globalHitTestManager;

		#if UNITY_EDITOR
		bool bTimeout;
		bool bResultsReceived;
		List<ARHitTestResult> resultsCollector;

		void Start()
		{
			globalHitTestManager = this;
			resultsCollector = new List<ARHitTestResult> ();
			EditorConnection.instance.Register (ConnectionMessageIds.receiveHitTestResults, ReceiveHitTestResults);
		}

		void ReceiveHitTestResults(MessageEventArgs mea)
		{
			Debug.Log ("Received HitTestResults");
			serializableHitTestResults shtr = mea.data.Deserialize<serializableHitTestResults> ();
			if (shtr != null && shtr.numResults > 0) 
			{
				for (int i = 0; i < shtr.numResults; i++) {
					resultsCollector.Add(shtr.hitTestResults [i]);
				}
			}
			bResultsReceived = true;
		}

		public List<ARHitTestResult> RemoteHitTest(ARPoint point, ARHitTestResultType types)
		{
			//send query across to remote
			resultsCollector.Clear();
			bResultsReceived = false;
			bTimeout = false;

			serializableFromEditorMessage sfem = new serializableFromEditorMessage ();
			sfem.subMessageId = SubMessageIds.editorHitTestQuery;
			sfem.arHitTestQuery = new serializableHitTestQuery (point, types);
			EditorConnection.instance.Send (ConnectionMessageIds.fromEditorARKitSessionMsgId, sfem.SerializeToByteArray ());

			//StartCoroutine (RunTimeout ());
			//long counter = 1000000;

			//wait for remote to reply (or timeout?)
			while (!bResultsReceived)
				;

			//StopAllCoroutines ();

			//return result
			return resultsCollector;
		}

		IEnumerator RunTimeout()
		{
			yield return new WaitForSeconds (0.5f);
			bTimeout = true;
		}
		#else
		bool pendingHitTest;
		List<serializableHitTestQuery> queryList;

		void Start()
		{
			globalHitTestManager = this;
			pendingHitTest = false;
			queryList = new List<serializableHitTestQuery> ();
		}

		public void HandleEditorMessage(serializableFromEditorMessage sfem)
		{
			if (sfem != null && sfem.subMessageId == SubMessageIds.editorHitTestQuery) {
				if (sfem.arHitTestQuery != null) {
					queryList.Add (sfem.arHitTestQuery);
					pendingHitTest = true;
				}
			}
		}

		void Update()
		{
			if (pendingHitTest) {
				foreach (serializableHitTestQuery shtq in queryList) {
					UnityARSessionNativeInterface session = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
					List<ARHitTestResult> hitTestResults = session.HitTest (shtq.arPoint, shtq.hitTypes);
					serializableHitTestResults shtr = new serializableHitTestResults (hitTestResults);
					PlayerConnection.instance.Send (ConnectionMessageIds.receiveHitTestResults, shtr.SerializeToByteArray ());
					Debug.Log ("HitTestResults sent: " + hitTestResults.Count);
				}
				queryList.Clear ();
				pendingHitTest = false;
			}
		}

		#endif
	}
		
}
