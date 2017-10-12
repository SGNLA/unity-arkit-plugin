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


	public class UnityARDirectionalLightEstimate
	{
		public float [] sphericalHarmonicsCoefficients;
		public Vector3 primaryLightDirection;
		public float primaryLightIntensity;

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

