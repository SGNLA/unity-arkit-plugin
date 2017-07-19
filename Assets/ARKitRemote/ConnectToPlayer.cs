﻿using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using System.Text;
using Utils; 

#if UNITY_EDITOR
 
using UnityEditor.Networking.PlayerConnection;

namespace UnityEngine.XR.iOS
{
	public class ConnectToPlayer : MonoBehaviour
	{
		EditorConnection editorConnection ;

		System.Guid firstmessageid;
		System.Guid toEditorMessageid;
		System.Guid toEditorMessageid2;
		int currentPlayerID = -1;
		Vector3 vectorReceived = Vector3.zero;
		string othermessage = "none";

		Texture2D remoteScreenYTex;
		Texture2D remoteScreenUVTex;

		bool bTexturesInitialized;

		// Use this for initialization
		void Start () {
			firstmessageid = new System.Guid("000000000000000000000000000000a1");
			toEditorMessageid = new System.Guid("000000000000000000000000000000e1");
			toEditorMessageid2 = new System.Guid("000000000000000000000000000000e2");

			bTexturesInitialized = false;
			//put some defaults so that it doesnt complain
			UnityARCamera scamera = new UnityARCamera ();
			scamera.worldTransform = new UnityARMatrix4x4 (new Vector4 (1, 0, 0, 0), new Vector4 (0, 1, 0, 0), new Vector4 (0, 0, 1, 0), new Vector4 (0, 0, 0, 1));
			Matrix4x4 projMat = Matrix4x4.Perspective (60.0f, 1.33f, 0.1f, 30.0f);
			scamera.projectionMatrix = new UnityARMatrix4x4 (projMat.GetColumn(0),projMat.GetColumn(1),projMat.GetColumn(2),projMat.GetColumn(3));

			UnityARSessionNativeInterface.SetStaticCamera (scamera);


			editorConnection = EditorConnection.instance;
			editorConnection.Initialize ();
			editorConnection.RegisterConnection (PlayerConnected);
			editorConnection.RegisterDisconnection (PlayerDisconnected);
			editorConnection.Register (toEditorMessageid, EditorMessageHandler);
			editorConnection.Register (toEditorMessageid2, EditorMessageHandler2);
			editorConnection.Register (ConnectionMessageIds.updateCameraFrameMsgId, UpdateCameraFrame);
			editorConnection.Register (ConnectionMessageIds.addPlaneAnchorMsgeId, AddPlaneAnchor);
			editorConnection.Register (ConnectionMessageIds.updatePlaneAnchorMsgeId, UpdatePlaneAnchor);
			editorConnection.Register (ConnectionMessageIds.removePlaneAnchorMsgeId, RemovePlaneAnchor);
			editorConnection.Register (ConnectionMessageIds.screenCaptureYMsgId, ReceiveRemoteScreenYTex);
			editorConnection.Register (ConnectionMessageIds.screenCaptureUVMsgId, ReceiveRemoteScreenUVTex);

			SendInitToPlayer ();
		}

		void PlayerConnected(int playerID)
		{
			currentPlayerID = playerID;

		}

		void OnGUI()
		{
			if (GUI.Button (new Rect (300, 20, 400, 100), "Send Test")) {
				SendTestToPlayer ();
			}

			if (GUI.Button (new Rect (300, 140, 400, 100), "Send Init")) {
				SendInitToPlayer ();
			}


			if (currentPlayerID != -1) {
				GUI.Box (new Rect (300, 250, 400, 50), currentPlayerID.ToString());
			}

			string guiMessage = string.Format("vector: {0},{1},{2}", vectorReceived.x, vectorReceived.y, vectorReceived.z);

			GUI.Box (new Rect (400, 320, 400, 50), guiMessage);

			GUI.Box (new Rect (0, 320, 400, 50), othermessage);

			Matrix4x4 matrix = UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraPose();
			Vector4 position = UnityARMatrixOps.GetPosition(matrix);

			GUI.Box (new Rect (0, 20, 250, 200), position.ToString ());

		}

		void PlayerDisconnected(int playerID)
		{
			if (currentPlayerID == playerID) {
				currentPlayerID = -1;
			}
		}

		void OnDestroy()
		{
			editorConnection.DisconnectAll ();
		}

		void EditorMessageHandler(MessageEventArgs mea)
		{
			Debug.Log("EditorMessageHandler");
		}

		void InitializeTextures(UnityARCamera camera)
		{
			int yWidth = camera.videoParams.yWidth;
			int yHeight = camera.videoParams.yHeight;
			int uvWidth = yWidth / 2;
			int uvHeight = yHeight / 2;
			remoteScreenYTex = new Texture2D(yWidth,yHeight, TextureFormat.R8, false, true);
			remoteScreenUVTex = new Texture2D(uvWidth,uvHeight, TextureFormat.RG16, false, true);

			bTexturesInitialized = true;
		}

		void UpdateCameraFrame(MessageEventArgs mea)
		{
			serializableUnityARCamera serCamera = mea.data.Deserialize<serializableUnityARCamera> ();

			UnityARCamera scamera = new UnityARCamera ();
			scamera = serCamera;

			InitializeTextures (scamera);

			UnityARSessionNativeInterface.SetStaticCamera (scamera);
			UnityARSessionNativeInterface.RunFrameUpdateCallbacks ();
		}

		void AddPlaneAnchor(MessageEventArgs mea)
		{
			serializableUnityARPlaneAnchor serPlaneAnchor = mea.data.Deserialize<serializableUnityARPlaneAnchor> ();

			ARPlaneAnchor arPlaneAnchor = serPlaneAnchor;
			UnityARSessionNativeInterface.RunAddAnchorCallbacks (arPlaneAnchor);
		}

		void UpdatePlaneAnchor(MessageEventArgs mea)
		{
			serializableUnityARPlaneAnchor serPlaneAnchor = mea.data.Deserialize<serializableUnityARPlaneAnchor> ();

			ARPlaneAnchor arPlaneAnchor = serPlaneAnchor;
			UnityARSessionNativeInterface.RunUpdateAnchorCallbacks (arPlaneAnchor);
		}

		void RemovePlaneAnchor(MessageEventArgs mea)
		{
			serializableUnityARPlaneAnchor serPlaneAnchor = mea.data.Deserialize<serializableUnityARPlaneAnchor> ();

			ARPlaneAnchor arPlaneAnchor = serPlaneAnchor;
			UnityARSessionNativeInterface.RunRemoveAnchorCallbacks (arPlaneAnchor);
		}

		void ReceiveRemoteScreenYTex(MessageEventArgs mea)
		{
			if (!bTexturesInitialized)
				return;
			remoteScreenYTex.LoadRawTextureData(mea.data);
			remoteScreenYTex.Apply ();
			UnityARVideo arVideo = Camera.main.GetComponent<UnityARVideo>();
			if (arVideo) {
				arVideo.SetYTexure(remoteScreenYTex);
			}

		}

		void ReceiveRemoteScreenUVTex(MessageEventArgs mea)
		{
			if (!bTexturesInitialized)
				return;
			remoteScreenUVTex.LoadRawTextureData(mea.data);
			remoteScreenUVTex.Apply ();
			UnityARVideo arVideo = Camera.main.GetComponent<UnityARVideo>();
			if (arVideo) {
				arVideo.SetUVTexure(remoteScreenUVTex);
			}

		}


		void EditorMessageHandler2(MessageEventArgs mea)
		{
			if (mea.playerId == currentPlayerID) {
				othermessage = ASCIIEncoding.ASCII.GetString(mea.data);
			}
		}

		void SendTestToPlayer()
		{
			// Input string.
			const string input = "Hello Player";
			byte[] array = Encoding.ASCII.GetBytes(input);
			SendToPlayer (firstmessageid, array);
		}

		void SendInitToPlayer()
		{
			// Input string.
			const string input = "Hello Player 2";
			byte[] array = Encoding.ASCII.GetBytes(input);
			SendToPlayer (ConnectionMessageIds.initARKitSessionMsgId, array);
		}

		void SendToPlayer(System.Guid msgId, byte[] data)
		{
			editorConnection.Send (msgId, data);
		}


		// Update is called once per frame
		void Update () {
			
		}

	}
}
#endif