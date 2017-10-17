using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
	public struct ARLightEstimate
	{
		public float ambientIntensity;
	}

	[Serializable]
	public struct UnityARLightEstimate
	{
		public float ambientIntensity;
		public float ambientColorTemperature;

		public UnityARLightEstimate(float intensity, float temperature)
		{
			ambientIntensity = intensity;
			ambientColorTemperature = temperature;
		}
	};


	public struct MarshalDirectionalLightEstimate
	{
		public Vector4 primaryDirAndIntensity;
		public IntPtr  sphericalHarmonicCoefficientsPtr;

		public float [] SphericalHarmonicCoefficients { get { return MarshalCoefficients(sphericalHarmonicCoefficientsPtr); } } 

		float [] MarshalCoefficients(IntPtr ptrFloats)
		{
			int numFloats = 27;
			float [] workCoefficients = new float[numFloats];
			Marshal.Copy (ptrFloats, workCoefficients, 0, (int)(numFloats)); 

			return workCoefficients;
		}

	}

	public class UnityARDirectionalLightEstimate
	{
		public Vector3 primaryLightDirection;
		public float primaryLightIntensity;
		public float [] sphericalHarmonicsCoefficients;

		public UnityARDirectionalLightEstimate (float [] SHC, Vector3 direction, float intensity)
		{
			sphericalHarmonicsCoefficients = SHC;
			primaryLightDirection = direction;
			primaryLightIntensity = intensity;
		}
	};

	public enum LightDataType
	{
		LightEstimate,
		DirectionalLightEstimate
	}

	public struct UnityMarshalLightData
	{
		public LightDataType arLightingType;
		public UnityARLightEstimate arLightEstimate;
		public MarshalDirectionalLightEstimate arDirectonalLightEstimate;

		public UnityMarshalLightData(LightDataType ldt, UnityARLightEstimate ule, MarshalDirectionalLightEstimate mdle)
		{
			arLightingType = ldt;
			arLightEstimate = ule;
			arDirectonalLightEstimate = mdle;
		}

		public static implicit operator UnityARLightData(UnityMarshalLightData rValue)
		{
			UnityARDirectionalLightEstimate udle = null;
			if (rValue.arLightingType == LightDataType.DirectionalLightEstimate) {
				Vector4 lightDirAndIntensity = rValue.arDirectonalLightEstimate.primaryDirAndIntensity;
				Vector3 lightDir = new Vector3 (lightDirAndIntensity.x, lightDirAndIntensity.y, lightDirAndIntensity.z);
				float[] shc = rValue.arDirectonalLightEstimate.SphericalHarmonicCoefficients;
				udle = new UnityARDirectionalLightEstimate (shc, lightDir, lightDirAndIntensity.w);
			}
			return new UnityARLightData(rValue.arLightingType, rValue.arLightEstimate, udle);
		}

	}

	public struct UnityARLightData
	{
		public LightDataType arLightingType;
		public UnityARLightEstimate arLightEstimate;
		public UnityARDirectionalLightEstimate arDirectonalLightEstimate;

		public UnityARLightData(LightDataType ldt, UnityARLightEstimate ule, UnityARDirectionalLightEstimate udle)
		{
			arLightingType = ldt;
			arLightEstimate = ule;
			arDirectonalLightEstimate = udle;
		}


	}


}

