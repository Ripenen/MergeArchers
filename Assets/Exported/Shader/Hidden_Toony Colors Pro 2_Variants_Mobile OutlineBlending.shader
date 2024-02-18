Shader "Hidden/Toony Colors Pro 2/Variants/Mobile OutlineBlending" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_HColor ("Highlight Color", Vector) = (0.785,0.785,0.785,1)
		_SColor ("Shadow Color", Vector) = (0.195,0.195,0.195,1)
		_MainTex ("Main Texture (RGB) Spec/MatCap Mask (A) ", 2D) = "white" {}
		[TCP2Gradient] _Ramp ("#RAMPT# Toon Ramp (RGB)", 2D) = "gray" {}
		_RampThreshold ("#RAMPF# Ramp Threshold", Range(0, 1)) = 0.5
		_RampSmooth ("#RAMPF# Ramp Smoothing", Range(0.01, 1)) = 0.1
		_BumpMap ("#NORM# Normal map (RGB)", 2D) = "bump" {}
		_OutlineColor ("#OUTLINE# Outline Color", Vector) = (0.2,0.2,0.2,1)
		_Outline ("#OUTLINE# Outline Width", Float) = 1
		_TexLod ("#OUTLINETEX# Texture LOD", Range(0, 10)) = 5
		_ZSmooth ("#OUTLINEZ# Z Correction", Range(-3, 3)) = -0.5
		_Offset1 ("#OUTLINEZ# Z Offset 1", Float) = 0
		_Offset2 ("#OUTLINEZ# Z Offset 2", Float) = 0
		_SrcBlendOutline ("#BLEND# Blending Source", Float) = 5
		_DstBlendOutline ("#BLEND# Blending Dest", Float) = 10
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
	//CustomEditor "TCP2_MaterialInspector"
}