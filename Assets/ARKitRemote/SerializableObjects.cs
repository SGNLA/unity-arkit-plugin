﻿using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using System.Text;

namespace Utils
{
	/// <summary>
	/// Since unity doesn't flag the Vector4 as serializable, we
	/// need to create our own version. This one will automatically convert
	/// between Vector4 and SerializableVector4
	/// </summary>
	[Serializable]
	public class SerializableVector4
	{
		/// <summary>
		/// x component
		/// </summary>
		public float x;

		/// <summary>
		/// y component
		/// </summary>
		public float y;

		/// <summary>
		/// z component
		/// </summary>
		public float z;

		/// <summary>
		/// w component
		/// </summary>
		public float w;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rX"></param>
		/// <param name="rY"></param>
		/// <param name="rZ"></param>
		/// <param name="rW"></param>
		public SerializableVector4(float rX, float rY, float rZ, float rW)
		{
			x = rX;
			y = rY;
			z = rZ;
			w = rW;
		}

		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
		}

		/// <summary>
		/// Automatic conversion from SerializableVector4 to Vector4
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator Vector4(SerializableVector4 rValue)
		{
			return new Vector4(rValue.x, rValue.y, rValue.z, rValue.w);
		}

		/// <summary>
		/// Automatic conversion from Vector4 to SerializableVector4
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator SerializableVector4(Vector4 rValue)
		{
			return new SerializableVector4(rValue.x, rValue.y, rValue.z, rValue.w);
		}
	}

	[Serializable]  
	public class serializableUnityARMatrix4x4
	{
		public SerializableVector4 column0;
		public SerializableVector4 column1;
		public SerializableVector4 column2;
		public SerializableVector4 column3;

		public serializableUnityARMatrix4x4(SerializableVector4 v0, SerializableVector4 v1, SerializableVector4 v2, SerializableVector4 v3)
		{
			column0 = v0;
			column1 = v1;
			column2 = v2;
			column3 = v3;
		}

		/// <summary>
		/// Automatic conversion from UnityARMatrix4x4 to serializableUnityARMatrix4x4
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator serializableUnityARMatrix4x4(UnityARMatrix4x4 rValue)
		{
			return new serializableUnityARMatrix4x4(rValue.column0, rValue.column1, rValue.column2, rValue.column3);
		}

		/// <summary>
		/// Automatic conversion from serializableUnityARMatrix4x4 to UnityARMatrix4x4
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator UnityARMatrix4x4(serializableUnityARMatrix4x4 rValue)
		{
			return new UnityARMatrix4x4(rValue.column0, rValue.column1, rValue.column2, rValue.column3);
		}


		public static implicit operator serializableUnityARMatrix4x4(Matrix4x4 rValue)
		{
			return new serializableUnityARMatrix4x4(rValue.GetColumn(0), rValue.GetColumn(1), rValue.GetColumn(2), rValue.GetColumn(3));
		}

		public static implicit operator Matrix4x4(serializableUnityARMatrix4x4 rValue)
		{
			return new Matrix4x4(rValue.column0, rValue.column1, rValue.column2, rValue.column3);
		}

	};

	[Serializable]  
	public class serializableUnityARCamera
	{
		public serializableUnityARMatrix4x4 worldTransform;
		public serializableUnityARMatrix4x4 projectionMatrix;
		public ARTrackingState trackingState;
		public ARTrackingStateReason trackingReason;



		public serializableUnityARCamera( serializableUnityARMatrix4x4 wt, serializableUnityARMatrix4x4 pm, ARTrackingState ats, ARTrackingStateReason atsr)
		{
			worldTransform = wt;
			projectionMatrix = pm;
			trackingState = ats;
			trackingReason = atsr;
		}

		public static implicit operator serializableUnityARCamera(UnityARCamera rValue)
		{
			return new serializableUnityARCamera(rValue.worldTransform, rValue.projectionMatrix, rValue.trackingState, rValue.trackingReason);
		}

		public static implicit operator UnityARCamera(serializableUnityARCamera rValue)
		{
			return new UnityARCamera (rValue.worldTransform, rValue.projectionMatrix, rValue.trackingState, rValue.trackingReason);
		}


	};

	[Serializable]  
	public class serializableUnityARPlaneAnchor
	{
		public serializableUnityARMatrix4x4 worldTransform;
		public SerializableVector4 center;
		public SerializableVector4 extent;
		public ARPlaneAnchorAlignment planeAlignment;
		public byte[] identifierStr;

		public serializableUnityARPlaneAnchor( serializableUnityARMatrix4x4 wt, SerializableVector4 ctr, SerializableVector4 ext, ARPlaneAnchorAlignment apaa,
			byte [] idstr)
		{
			worldTransform = wt;
			center = ctr;
			extent = ext;
			planeAlignment = apaa;
			identifierStr = idstr;
		}

		public static implicit operator serializableUnityARPlaneAnchor(ARPlaneAnchor rValue)
		{
			serializableUnityARMatrix4x4 wt = rValue.transform;
			SerializableVector4 ctr = new SerializableVector4 (rValue.center.x, rValue.center.y, rValue.center.z, 1.0f);
			SerializableVector4 ext = new SerializableVector4 (rValue.extent.x, rValue.extent.y, rValue.extent.z, 1.0f);
			byte[] idstr = Encoding.UTF8.GetBytes (rValue.identifier);
			return new serializableUnityARPlaneAnchor(wt, ctr, ext, rValue.alignment, idstr);
		}

		public static implicit operator ARPlaneAnchor(serializableUnityARPlaneAnchor rValue)
		{
			ARPlaneAnchor retValue;

			retValue.identifier = Encoding.UTF8.GetString (rValue.identifierStr);
			retValue.center = new Vector3 (rValue.center.x, rValue.center.y, rValue.center.z);
			retValue.extent = new Vector3 (rValue.extent.x, rValue.extent.y, rValue.extent.z);
			retValue.alignment = rValue.planeAlignment;
			retValue.transform = rValue.worldTransform;
			
			return retValue;
		}

	};
}