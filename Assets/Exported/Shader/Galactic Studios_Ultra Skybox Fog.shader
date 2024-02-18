Shader "Galactic Studios/Ultra Skybox Fog" {
	Properties {
		_Tint ("Tint Colour", Vector) = (0.5,0.5,0.5,0.5)
		[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1
		_Rotation ("Rotation", Range(0, 360)) = 0
		[NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "grey" {}
		_FogCol ("Fog Colour", Vector) = (0.5,0.5,0.5,0.5)
		_FogStart ("Fog Begin", Range(0, 1)) = 0
		_FogEnd ("Fog End", Range(0, 1)) = 0.4
		_FogIntens ("Fog Density", Range(0, 1)) = 1
		_MieIntens ("Mie Intensity", Range(0, 2)) = 0
		_MieTint ("Mie Tint", Vector) = (0.5,0.5,0.5,0.5)
		_MieSize ("Mie Size", Range(0, 1)) = 0.8
		_SunDir ("Sun Direction", Range(0, 360)) = 0
		[Toggle] _FogBottom ("Apply Fog To The Bottom Of The Sky?", Float) = 0
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