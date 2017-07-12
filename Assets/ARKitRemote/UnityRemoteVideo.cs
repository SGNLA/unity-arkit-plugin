using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.XR.iOS
{

	public class UnityRemoteVideo : MonoBehaviour
	{
		public UnityARVideo arVideoGO;
		public ConnectToEditor connectToEditor;

		private CommandBuffer m_VideoCommandBuffer;
		private Texture2D _videoTextureY;
		private Texture2D _videoTextureCbCr;

		private UnityARSessionNativeInterface m_Session;
		private bool bCommandBufferInitialized;

		private RenderTexture [] m_copyTexturesY;
		private int currentIndex;


		#if !UNITY_EDITOR

		public void Start()
		{
			m_Session = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
			//bCommandBufferInitialized = false;
			currentIndex = 0;
		}

//		void InitializeCommandBuffer()
//		{
//			m_VideoCommandBuffer = new CommandBuffer(); 
//			m_copyTexturesY = new RenderTexture[2];
//			arVideoGO.GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
//			bCommandBufferInitialized = true;
//
//		}

		void OnDestroy()
		{
			arVideoGO.GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
		}

		public void OnPreRender()
		{
			int previousIndex = currentIndex;
			currentIndex = (currentIndex + 1) % 2;
			ARTextureHandles handles = m_Session.GetARVideoTextureHandles();
			if (handles.textureY == System.IntPtr.Zero || handles.textureCbCr == System.IntPtr.Zero)
			{
				return;
			}

//			if (!bCommandBufferInitialized) {
//				InitializeCommandBuffer ();
//			}

			Resolution currentResolution = Screen.currentResolution;

			// Texture Y
//			_videoTextureY = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height,
//			TextureFormat.R8, false, false, (System.IntPtr)handles.textureY);
//			_videoTextureY.filterMode = FilterMode.Bilinear;
//			_videoTextureY.wrapMode = TextureWrapMode.Repeat;
//			_videoTextureY.UpdateExternalTexture(handles.textureY);
//
//			// Texture CbCr
//			_videoTextureCbCr = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height,
//			TextureFormat.RG16, false, false, (System.IntPtr)handles.textureCbCr);
//			_videoTextureCbCr.filterMode = FilterMode.Bilinear;
//			_videoTextureCbCr.wrapMode = TextureWrapMode.Repeat;
//			_videoTextureCbCr.UpdateExternalTexture(handles.textureCbCr);
			byte[] textureYData = new byte[currentResolution.width * currentResolution.height];
			Marshal.Copy ((System.IntPtr)handles.textureY, textureYData, 0, textureYData.Length);
			connectToEditor.SendToEditor (ConnectionMessageIds.screenCaptureMsgId, textureYData);

			//if (m_copyTexturesY [currentIndex] == null) {
			//	m_copyTexturesY [currentIndex] = new RenderTexture (Screen.width, Screen.height, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
			//}

			//m_VideoCommandBuffer.Clear ();
			//m_VideoCommandBuffer.Blit(_videoTextureY, m_copyTexturesY[currentIndex]);


			//send texture data from previous frame
//			if (m_copyTexturesY [previousIndex]) {
//				byte[] texData = new byte[3];//m_copyTexturesY [previousIndex];
//				string debgStr = "texData.length = " + texData.Length;
//				Debug.Log (debgStr);
//				if (texData.Length > 0)
//				connectToEditor.SendToEditor (ConnectionMessageIds.screenCaptureMsgId, texData);
//			}
		}
		#endif
	}
}
