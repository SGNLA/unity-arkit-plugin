using UnityEngine;
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

		Texture2D remoteScreenTex;

		// Use this for initialization
		void Start () {
			firstmessageid = new System.Guid("000000000000000000000000000000a1");
			toEditorMessageid = new System.Guid("000000000000000000000000000000e1");
			toEditorMessageid2 = new System.Guid("000000000000000000000000000000e2");

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
			editorConnection.Register (ConnectionMessageIds.screenCaptureMsgId, ReceiveRemoteScreen);

			remoteScreenTex = new Texture2D (2, 2); //LoadImage will resize to fit
			byte[] pngBytes = new byte[] {
				0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
				0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x40, 0x08, 0x00, 0x00, 0x00, 0x00, 0x8F, 0x02, 0x2E,
				0x02, 0x00, 0x00, 0x01, 0x57, 0x49, 0x44, 0x41, 0x54, 0x78, 0x01, 0xA5, 0x57, 0xD1, 0xAD, 0xC4,
				0x30, 0x08, 0x83, 0x81, 0x32, 0x4A, 0x66, 0xC9, 0x36, 0x99, 0x85, 0x45, 0xBC, 0x4E, 0x74, 0xBD,
				0x8F, 0x9E, 0x5B, 0xD4, 0xE8, 0xF1, 0x6A, 0x7F, 0xDD, 0x29, 0xB2, 0x55, 0x0C, 0x24, 0x60, 0xEB,
				0x0D, 0x30, 0xE7, 0xF9, 0xF3, 0x85, 0x40, 0x74, 0x3F, 0xF0, 0x52, 0x00, 0xC3, 0x0F, 0xBC, 0x14,
				0xC0, 0xF4, 0x0B, 0xF0, 0x3F, 0x01, 0x44, 0xF3, 0x3B, 0x3A, 0x05, 0x8A, 0x41, 0x67, 0x14, 0x05,
				0x18, 0x74, 0x06, 0x4A, 0x02, 0xBE, 0x47, 0x54, 0x04, 0x86, 0xEF, 0xD1, 0x0A, 0x02, 0xF0, 0x84,
				0xD9, 0x9D, 0x28, 0x08, 0xDC, 0x9C, 0x1F, 0x48, 0x21, 0xE1, 0x4F, 0x01, 0xDC, 0xC9, 0x07, 0xC2,
				0x2F, 0x98, 0x49, 0x60, 0xE7, 0x60, 0xC7, 0xCE, 0xD3, 0x9D, 0x00, 0x22, 0x02, 0x07, 0xFA, 0x41,
				0x8E, 0x27, 0x4F, 0x31, 0x37, 0x02, 0xF9, 0xC3, 0xF1, 0x7C, 0xD2, 0x16, 0x2E, 0xE7, 0xB6, 0xE5,
				0xB7, 0x9D, 0xA7, 0xBF, 0x50, 0x06, 0x05, 0x4A, 0x7C, 0xD0, 0x3B, 0x4A, 0x2D, 0x2B, 0xF3, 0x97,
				0x93, 0x35, 0x77, 0x02, 0xB8, 0x3A, 0x9C, 0x30, 0x2F, 0x81, 0x83, 0xD5, 0x6C, 0x55, 0xFE, 0xBA,
				0x7D, 0x19, 0x5B, 0xDA, 0xAA, 0xFC, 0xCE, 0x0F, 0xE0, 0xBF, 0x53, 0xA0, 0xC0, 0x07, 0x8D, 0xFF,
				0x82, 0x89, 0xB4, 0x1A, 0x7F, 0xE5, 0xA3, 0x5F, 0x46, 0xAC, 0xC6, 0x0F, 0xBA, 0x96, 0x1C, 0xB1,
				0x12, 0x7F, 0xE5, 0x33, 0x26, 0xD2, 0x4A, 0xFC, 0x41, 0x07, 0xB3, 0x09, 0x56, 0xE1, 0xE3, 0xA1,
				0xB8, 0xCE, 0x3C, 0x5A, 0x81, 0xBF, 0xDA, 0x43, 0x73, 0x75, 0xA6, 0x71, 0xDB, 0x7F, 0x0F, 0x29,
				0x24, 0x82, 0x95, 0x08, 0xAF, 0x21, 0xC9, 0x9E, 0xBD, 0x50, 0xE6, 0x47, 0x12, 0x38, 0xEF, 0x03,
				0x78, 0x11, 0x2B, 0x61, 0xB4, 0xA5, 0x0B, 0xE8, 0x21, 0xE8, 0x26, 0xEA, 0x69, 0xAC, 0x17, 0x12,
				0x0F, 0x73, 0x21, 0x29, 0xA5, 0x2C, 0x37, 0x93, 0xDE, 0xCE, 0xFA, 0x85, 0xA2, 0x5F, 0x69, 0xFA,
				0xA5, 0xAA, 0x5F, 0xEB, 0xFA, 0xC3, 0xA2, 0x3F, 0x6D, 0xFA, 0xE3, 0xAA, 0x3F, 0xEF, 0xFA, 0x80,
				0xA1, 0x8F, 0x38, 0x04, 0xE2, 0x8B, 0xD7, 0x43, 0x96, 0x3E, 0xE6, 0xE9, 0x83, 0x26, 0xE1, 0xC2,
				0xA8, 0x2B, 0x0C, 0xDB, 0xC2, 0xB8, 0x2F, 0x2C, 0x1C, 0xC2, 0xCA, 0x23, 0x2D, 0x5D, 0xFA, 0xDA,
				0xA7, 0x2F, 0x9E, 0xFA, 0xEA, 0xAB, 0x2F, 0xDF, 0xF2, 0xFA, 0xFF, 0x01, 0x1A, 0x18, 0x53, 0x83,
				0xC1, 0x4E, 0x14, 0x1B, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82,
			};
			// Load data into the texture.
			remoteScreenTex.LoadImage(pngBytes);
			UnityARVideo arVideo = Camera.main.GetComponent<UnityARVideo>();
			if (arVideo) {
				arVideo.m_EditorRemoteTexture = remoteScreenTex;
			}

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

		void EditorMessageHandler(MessageEventArgs mea)
		{
			Debug.Log("EditorMessageHandler");
		}

		void UpdateCameraFrame(MessageEventArgs mea)
		{
			Debug.Log("UpdateCameraFrame");
			serializableUnityARCamera serCamera = mea.data.Deserialize<serializableUnityARCamera> ();

			UnityARCamera scamera = new UnityARCamera ();
			//scamera.worldTransform = worldTransform;
			scamera = serCamera;
			UnityARSessionNativeInterface.SetStaticCamera (scamera);
			UnityARSessionNativeInterface.RunFrameUpdateCallbacks ();
		}

		void AddPlaneAnchor(MessageEventArgs mea)
		{
			Debug.Log("AddPlaneAnchor");
			serializableUnityARPlaneAnchor serPlaneAnchor = mea.data.Deserialize<serializableUnityARPlaneAnchor> ();

			ARPlaneAnchor arPlaneAnchor = serPlaneAnchor;
			UnityARSessionNativeInterface.RunAddAnchorCallbacks (arPlaneAnchor);
		}

		void UpdatePlaneAnchor(MessageEventArgs mea)
		{
			Debug.Log("UpdatePlaneAnchor");

			serializableUnityARPlaneAnchor serPlaneAnchor = mea.data.Deserialize<serializableUnityARPlaneAnchor> ();

			ARPlaneAnchor arPlaneAnchor = serPlaneAnchor;
			UnityARSessionNativeInterface.RunUpdateAnchorCallbacks (arPlaneAnchor);
		}

		void RemovePlaneAnchor(MessageEventArgs mea)
		{
			Debug.Log("RemovePlaneAnchor");

			serializableUnityARPlaneAnchor serPlaneAnchor = mea.data.Deserialize<serializableUnityARPlaneAnchor> ();

			ARPlaneAnchor arPlaneAnchor = serPlaneAnchor;
			UnityARSessionNativeInterface.RunRemoveAnchorCallbacks (arPlaneAnchor);
		}

		void ReceiveRemoteScreen(MessageEventArgs mea)
		{
			remoteScreenTex.LoadImage (mea.data);
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
			const string input = "Hello Player";
			byte[] array = Encoding.ASCII.GetBytes(input);
			SendToPlayer (ConnectionMessageIds.initARKitSessionMsgId, array);
		}

		void SendToPlayer(System.Guid msgId, byte[] data)
		{
			//if (editorConnection.ConnectedPlayers.Find (cp => cp.PlayerId == currentPlayerID) != null) {
				editorConnection.Send (msgId, data);
			//}
		}


		// Update is called once per frame
		void Update () {
			
		}
	}
}
#endif