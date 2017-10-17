using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{

	public enum ARBlendShapeLocation
	{
		 ARBlendShapeLocationBrowDownLeft        ,   
	     ARBlendShapeLocationBrowDownRight       ,   
	     ARBlendShapeLocationBrowInnerUp         ,   
	     ARBlendShapeLocationBrowOuterUpLeft     ,   
	     ARBlendShapeLocationBrowOuterUpRight    ,   
	     ARBlendShapeLocationCheekPuff           ,   
	     ARBlendShapeLocationCheekSquintLeft     ,   
	     ARBlendShapeLocationCheekSquintRight    ,   
	     ARBlendShapeLocationEyeBlinkLeft        ,   
	     ARBlendShapeLocationEyeBlinkRight       ,   
	     ARBlendShapeLocationEyeLookDownLeft     ,   
	     ARBlendShapeLocationEyeLookDownRight    ,   
	     ARBlendShapeLocationEyeLookInLeft       ,   
	     ARBlendShapeLocationEyeLookInRight      ,   
	     ARBlendShapeLocationEyeLookOutLeft      ,   
	     ARBlendShapeLocationEyeLookOutRight     ,   
	     ARBlendShapeLocationEyeLookUpLeft       ,   
	     ARBlendShapeLocationEyeLookUpRight      ,   
	     ARBlendShapeLocationEyeSquintLeft       ,   
	     ARBlendShapeLocationEyeSquintRight      ,   
	     ARBlendShapeLocationEyeWideLeft         ,   
	     ARBlendShapeLocationEyeWideRight        ,   
	     ARBlendShapeLocationJawForward          ,   
	     ARBlendShapeLocationJawLeft             ,   
	     ARBlendShapeLocationJawOpen             ,   
	     ARBlendShapeLocationJawRight            ,   
	     ARBlendShapeLocationMouthClose          ,   
	     ARBlendShapeLocationMouthDimpleLeft     ,   
	     ARBlendShapeLocationMouthDimpleRight    ,   
	     ARBlendShapeLocationMouthFrownLeft      ,   
	     ARBlendShapeLocationMouthFrownRight     ,   
	     ARBlendShapeLocationMouthFunnel         ,   
	     ARBlendShapeLocationMouthLeft           ,   
	     ARBlendShapeLocationMouthLowerDownLeft  ,   
	     ARBlendShapeLocationMouthLowerDownRight ,   
	     ARBlendShapeLocationMouthPressLeft      ,   
	     ARBlendShapeLocationMouthPressRight     ,   
	     ARBlendShapeLocationMouthPucker         ,   
	     ARBlendShapeLocationMouthRight          ,   
	     ARBlendShapeLocationMouthRollLower      ,   
	     ARBlendShapeLocationMouthRollUpper      ,   
	     ARBlendShapeLocationMouthShrugLower     ,   
	     ARBlendShapeLocationMouthShrugUpper     ,   
	     ARBlendShapeLocationMouthSmileLeft      ,   
	     ARBlendShapeLocationMouthSmileRight     ,   
	     ARBlendShapeLocationMouthStretchLeft    ,   
	     ARBlendShapeLocationMouthStretchRight   ,   
	     ARBlendShapeLocationMouthUpperUpLeft    ,   
	     ARBlendShapeLocationMouthUpperUpRight   ,   
	     ARBlendShapeLocationNoseSneerLeft       ,   
	     ARBlendShapeLocationNoseSneerRight         
	}


	public struct UnityARFaceGeometry
	{
		public int vertexCount;
		public IntPtr vertices;
		public int textureCoordinateCount;
		public IntPtr textureCoordinates;
		public int triangleCount;
		public IntPtr triangleIndices;

	}

	public struct UnityARFaceAnchorData 
	{

		public IntPtr ptrIdentifier;

		/**
 		The transformation matrix that defines the anchor's rotation, translation and scale in world coordinates.
		 */
		public UnityARMatrix4x4 transform;

		public string identifierStr { get { return Marshal.PtrToStringAuto(this.ptrIdentifier); } }

		public UnityARFaceGeometry faceGeometry;
		public IntPtr blendShapes;

	};


	public class ARFaceGeometry
	{
		private UnityARFaceGeometry uFaceGeometry;

		public ARFaceGeometry (UnityARFaceGeometry ufg)
		{
			uFaceGeometry = ufg;
		}

		public int vertexCount { get { return uFaceGeometry.vertexCount; } }
		public int triangleCount {  get  { return uFaceGeometry.triangleCount; } }
		public int textureCoordinateCount { get { return uFaceGeometry.textureCoordinateCount; } }

		public Vector3 [] vertices { get { return MarshalVertices(uFaceGeometry.vertices,vertexCount); } }

		public Vector2 [] textureCoordinates { get { return MarshalTexCoords(uFaceGeometry.textureCoordinates, textureCoordinateCount); } }

		public int [] triangleIndices { get { return MarshalIndices(uFaceGeometry.triangleIndices, triangleCount); } }

		Vector3 [] MarshalVertices(IntPtr ptrFloatArray, int vertCount)
		{
			int numFloats = vertCount * 4;
			float [] workVerts = new float[numFloats];
			Marshal.Copy (ptrFloatArray, workVerts, 0, (int)(numFloats)); 

			Vector3[] verts = new Vector3[vertCount];

			for (int count = 0; count < numFloats; count++)
			{
				verts [count / 4].x = workVerts[count++];
				verts [count / 4].y = workVerts[count++];
				verts [count / 4].z = -workVerts[count++];
			}

			return verts;
		}

		int [] MarshalIndices(IntPtr ptrIndices, int triCount)
		{
			int numIndices = triCount * 3;
			short [] workIndices = new short[numIndices];  //since ARKit returns Int16
			Marshal.Copy (ptrIndices, workIndices, 0, numIndices);

			int[] triIndices = new int[numIndices];
			for (int count = 0; count < numIndices; count+=3) {
				//reverse winding order
				triIndices [count] = workIndices [count];
				triIndices [count + 1] = workIndices [count + 2];
				triIndices [count + 2] = workIndices [count + 1];
			}

			return triIndices;
		}

		Vector2 [] MarshalTexCoords(IntPtr ptrTexCoords, int texCoordCount)
		{
			int numFloats = texCoordCount * 2;
			float [] workTexCoords = new float[numFloats];
			Marshal.Copy (ptrTexCoords, workTexCoords, 0, (int)(numFloats)); 

			Vector2[] texCoords = new Vector2[texCoordCount];

			for (int count = 0; count < numFloats; count++)
			{
				texCoords [count / 2].x = workTexCoords[count++];
				texCoords [count / 2].y = workTexCoords[count];
			}

			return texCoords;

		}
	}


	public class ARFaceAnchor 
	{
		private UnityARFaceAnchorData faceAnchorData;
		private Dictionary<ARBlendShapeLocation, float> managedDictionary;

		public ARFaceAnchor (UnityARFaceAnchorData ufad)
		{
			faceAnchorData = ufad;
			managedDictionary = new Dictionary<ARBlendShapeLocation, float> ();
		}
		

		public string identifierStr { get { return faceAnchorData.identifierStr; } }

		public Matrix4x4 transform { 
			get { 
				Matrix4x4 matrix = new Matrix4x4 ();
				matrix.SetColumn (0, faceAnchorData.transform.column0);
				matrix.SetColumn (1, faceAnchorData.transform.column1);
				matrix.SetColumn (2, faceAnchorData.transform.column2);
				matrix.SetColumn (3, faceAnchorData.transform.column3);
				return matrix;
			}
		}

		public ARFaceGeometry faceGeometry { get { return new ARFaceGeometry (faceAnchorData.faceGeometry);	} }

		public Dictionary<ARBlendShapeLocation, float> blendShapes { get { return GetBlendShapesFromNative(faceAnchorData.blendShapes); } }

		public delegate void DictionaryVisitorHandler(string key, float value);

		[DllImport("__Internal")]
		private static extern void GetBlendShapesInfo(IntPtr ptrDic, DictionaryVisitorHandler handler);

		Dictionary<ARBlendShapeLocation, float> GetBlendShapesFromNative(IntPtr blendShapesPtr)
		{
			managedDictionary.Clear ();
			GetBlendShapesInfo (blendShapesPtr, AddElementToManagedDictionary);
			return managedDictionary;
		}

		void AddElementToManagedDictionary(string key, float value)
		{
			ARBlendShapeLocation arb = (ARBlendShapeLocation) Enum.Parse (typeof(ARBlendShapeLocation), key);
			managedDictionary.Add (arb, value);
		}
	}
}
