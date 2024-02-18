Shader "StylizedWater/Mobile" {
	Properties {
		[HDR] _WaterColor ("Water Color", Vector) = (0.1176471,0.6348885,1,0)
		[HDR] _WaterShallowColor ("WaterShallowColor", Vector) = (0.4191176,0.7596349,1,0)
		_Wavetint ("Wave tint", Range(-1, 1)) = 0
		[HDR] _RimColor ("Rim Color", Vector) = (1,1,1,0.5019608)
		_NormalStrength ("NormalStrength", Range(0, 1)) = 0.25
		_Transparency ("Transparency", Range(0, 1)) = 0.75
		_Glossiness ("Glossiness", Range(0, 1)) = 0.85
		[Toggle] _Worldspacetiling ("Worldspace tiling", Float) = 1
		_NormalTiling ("NormalTiling", Range(0, 1)) = 0.9
		_EdgeFade ("EdgeFade", Range(0.01, 3)) = 0.2448298
		_RimSize ("Rim Size", Range(0, 20)) = 5
		_Rimfalloff ("Rim falloff", Range(0.1, 50)) = 3
		_Rimtiling ("Rim tiling", Float) = 0.5
		_FoamOpacity ("FoamOpacity", Range(-1, 1)) = 0.05
		_FoamSpeed ("FoamSpeed", Range(0, 1)) = 0.1
		_FoamSize ("FoamSize", Float) = 0
		_FoamTiling ("FoamTiling", Float) = 0.05
		_Depth ("Depth", Range(0, 100)) = 30
		_Wavesspeed ("Waves speed", Range(0, 10)) = 0.75
		_WaveHeight ("Wave Height", Range(0, 1)) = 0.5366272
		_WaveFoam ("Wave Foam", Range(0, 10)) = 0
		_WaveSize ("Wave Size", Range(0, 10)) = 0.1
		_WaveDirection ("WaveDirection", Vector) = (1,0,0,0)
		[NoScaleOffset] [Normal] _Normals ("Normals", 2D) = "bump" {}
		[NoScaleOffset] _Shadermap ("Shadermap", 2D) = "black" {}
		[Toggle(_USEINTERSECTIONFOAM_ON)] _UseIntersectionFoam ("UseIntersectionFoam", Float) = 0
		[Toggle(_LIGHTING_ON)] _LIGHTING ("LIGHTING", Float) = 0
		[Toggle] _ENABLE_VC ("ENABLE_VC", Float) = 0
		[Toggle] _Unlit ("Unlit", Float) = 0
		[Toggle(_NORMAL_MAP_ON)] _NORMAL_MAP ("NORMAL_MAP", Float) = 0
		_Metallicness ("Metallicness", Range(0, 1)) = 0
		[Toggle] _USE_VC_INTERSECTION ("USE_VC_INTERSECTION", Float) = 0
		[Toggle] _EnableDepthTexture ("EnableDepthTexture", Float) = 1
		[HideInInspector] __dirty ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}