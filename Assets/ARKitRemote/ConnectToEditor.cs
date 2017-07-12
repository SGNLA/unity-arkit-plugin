using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using System.Text;
using Utils;

namespace UnityEngine.XR.iOS
{
	
	public class ConnectToEditor : MonoBehaviour
	{
		PlayerConnection playerConnection;
		UnityARSessionNativeInterface m_session;
		string message;
		int editorID;

		System.Guid firstmessageid;
		System.Guid toEditorMessageid;
		System.Guid toEditorMessageid2;

		Texture2D frameBufferTex;

		// Use this for initialization
		void Start()
		{
			Debug.Log("STARTING ConnectToEditor");
			message = "none";
			editorID = -1;
			firstmessageid = new System.Guid("000000000000000000000000000000a1");
			toEditorMessageid = new System.Guid("000000000000000000000000000000e1");
			toEditorMessageid2 = new System.Guid("000000000000000000000000000000e2");
			playerConnection = PlayerConnection.instance;
			playerConnection.RegisterConnection(EditorConnected);
			playerConnection.RegisterDisconnection(EditorDisconnected);
			playerConnection.Register(firstmessageid, FirstMessageHandler);
			playerConnection.Register(ConnectionMessageIds.initARKitSessionMsgId, InitializeARKit);



		}

//		void OnGUI()
//		{
//			if (GUI.Button(new Rect(300, 20, 400, 100), "Send Test"))
//			{
//				SendTestToEditor();
//			}
//
//			if (GUI.Button(new Rect(300, 150, 400, 100), "Send Test 2"))
//			{
//				SendTest2ToEditor();
//			}
//
//			if (GUI.Button(new Rect(300, 400, 400, 100), "Disconnect"))
//			{
//				DisconnectFromEditor();
//			}
//
//			string guiMessage = "message:" + message;
//
//			GUI.Box(new Rect(300, 300, 400, 50), guiMessage);
//		}

		void FirstMessageHandler(MessageEventArgs mea)
		{
			Debug.Log("received first message");
			//if (mea.playerId == currentPlayerID) 
			{
				message = ASCIIEncoding.ASCII.GetString(mea.data);
				editorID = mea.playerId;
				 
				InitializeARKit(mea);
			}
		}

		void InitializeARKit(MessageEventArgs mea)
		{
			Debug.Log("init ARKit");
			message = "Connected... init ARKit";
			#if !UNITY_EDITOR
			Application.targetFrameRate = 60;
			m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
			ARKitWorldTackingSessionConfiguration config = new ARKitWorldTackingSessionConfiguration();
			config.planeDetection = UnityARPlaneDetection.Horizontal;
			config.alignment = UnityARAlignment.UnityARAlignmentGravity;
			config.getPointCloudData = true;
			config.enableLightEstimation = true;
			m_session.RunWithConfig(config);

		 	UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
			UnityARSessionNativeInterface.ARAnchorAddedEvent += ARAnchorAdded;
			UnityARSessionNativeInterface.ARAnchorUpdatedEvent += ARAnchorUpdated;
			UnityARSessionNativeInterface.ARAnchorRemovedEvent += ARAnchorRemoved;

			#endif
		}

		public void ARFrameUpdated(UnityARCamera camera)
		{
			//Debug.Log("sending camera");
			serializableUnityARCamera serARCamera = camera;
			SendToEditor(ConnectionMessageIds.updateCameraFrameMsgId, serARCamera);

		}

		public void ARAnchorAdded(ARPlaneAnchor planeAnchor)
		{
			//Debug.Log("adding anchor");
			serializableUnityARPlaneAnchor serPlaneAnchor = planeAnchor;
			SendToEditor (ConnectionMessageIds.addPlaneAnchorMsgeId, serPlaneAnchor);
		}

		public void ARAnchorUpdated(ARPlaneAnchor planeAnchor)
		{
			//Debug.Log("updating anchor");
			serializableUnityARPlaneAnchor serPlaneAnchor = planeAnchor;
			SendToEditor (ConnectionMessageIds.updatePlaneAnchorMsgeId, serPlaneAnchor);
		}

		public void ARAnchorRemoved(ARPlaneAnchor planeAnchor)
		{
			//Debug.Log("removing anchor");
			serializableUnityARPlaneAnchor serPlaneAnchor = planeAnchor;
			SendToEditor (ConnectionMessageIds.removePlaneAnchorMsgeId, serPlaneAnchor);
		}

		void EditorConnected(int playerID)
		{
			Debug.Log("connected");

			editorID = playerID;

		}

		void EditorDisconnected(int playerID)
		{
			if (editorID == playerID)
			{
				editorID = -1;
			}
		}

		public void SendTestToEditor()
		{
			Debug.Log("sending vector4");
			byte[] array = Encoding.ASCII.GetBytes("Hello Editor");
			SendToEditor(toEditorMessageid,array );
		}

		public void SendTest2ToEditor()
		{
			byte[] arrayToSend = Encoding.ASCII.GetBytes("Hello Editor");
			SendToEditor(toEditorMessageid2, arrayToSend);
		}

		public void SendToEditor(System.Guid msgId, object serializableObject)
		{
			byte[] arrayToSend = serializableObject.SerializeToByteArray ();
			SendToEditor (msgId, arrayToSend);
		}

		public void SendToEditor(System.Guid msgId, byte[] data)
		{
			if (playerConnection.isConnected)
			{
				//Debug.Log("Editor is connected.. sending");
				playerConnection.Send(msgId, data);
			}


		}

		public void DisconnectFromEditor()
		{
			playerConnection.DisconnectAll();
		}

		// Update is called once per frame
		void Update()
		{
		}


	}

}
