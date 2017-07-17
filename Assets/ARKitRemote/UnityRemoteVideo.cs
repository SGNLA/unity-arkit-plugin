using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.XR.iOS
{

	public class UnityRemoteVideo : MonoBehaviour
	{
		public ConnectToEditor connectToEditor;

		private CommandBuffer m_VideoCommandBuffer;
		private Texture2D _videoTextureY;
		private Texture2D _videoTextureCbCr;

		private UnityARSessionNativeInterface m_Session;
		private bool bCommandBufferInitialized;

		private RenderTexture [] m_copyTexturesY;
		private int currentFrameIndex;
		private byte[] m_textureYBytes;
		private byte[] m_textureUVBytes;
		private byte[] m_textureYBytes2;
		private byte[] m_textureUVBytes2;
		private GCHandle m_pinnedYArray;
		private GCHandle m_pinnedUVArray;

		#if !UNITY_EDITOR

		public void Start()
		{
			m_Session = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
			currentFrameIndex = 0;
			m_textureYBytes = new byte[1280 * 720];
			m_textureUVBytes = new byte[640 * 360 * 2];
			m_textureYBytes2 = new byte[1280 * 720];
			m_textureUVBytes2 = new byte[640 * 360 * 2];
			m_pinnedYArray = GCHandle.Alloc (m_textureYBytes);
			m_pinnedUVArray = GCHandle.Alloc (m_textureUVBytes);
		}

		IntPtr PinByteArray(ref GCHandle handle, byte[] array)
		{
			handle.Free ();
			handle = GCHandle.Alloc (array, GCHandleType.Pinned);
			return handle.AddrOfPinnedObject ();
		}

		byte [] ByteArrayForFrame(int frame,  byte[] array0,  byte[] array1)
		{
			return frame == 1 ? array1 : array0;
		}

		byte [] YByteArrayForFrame(int frame)
		{
			return ByteArrayForFrame (frame, m_textureYBytes, m_textureYBytes2);
		}

		byte [] UVByteArrayForFrame(int frame)
		{
			return ByteArrayForFrame (frame, m_textureUVBytes, m_textureUVBytes2);
		}

		void OnDestroy()
		{
			m_Session.SetCapturePixelData (false, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

			m_pinnedYArray.Free ();
			m_pinnedUVArray.Free ();

		}

		public void OnPreRender()
		{
			ARTextureHandles handles = m_Session.GetARVideoTextureHandles();
			if (handles.textureY == System.IntPtr.Zero || handles.textureCbCr == System.IntPtr.Zero)
			{
				return;
			}
			currentFrameIndex = (currentFrameIndex + 1) % 2;

			Resolution currentResolution = Screen.currentResolution;


			m_Session.SetCapturePixelData (true, PinByteArray(ref m_pinnedYArray,YByteArrayForFrame(currentFrameIndex)), PinByteArray(ref m_pinnedUVArray,UVByteArrayForFrame(currentFrameIndex)), IntPtr.Zero);

			connectToEditor.SendToEditor (ConnectionMessageIds.screenCaptureYMsgId, YByteArrayForFrame(1-currentFrameIndex));
			connectToEditor.SendToEditor (ConnectionMessageIds.screenCaptureUVMsgId, UVByteArrayForFrame(1-currentFrameIndex));

		}
		#endif
	}
}
