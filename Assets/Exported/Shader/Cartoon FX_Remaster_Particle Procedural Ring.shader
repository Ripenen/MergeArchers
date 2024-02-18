Shader "Cartoon FX/Remaster/Particle Procedural Ring" {
	Properties {
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Blend Source", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Blend Destination", Float) = 10
		[Toggle(_CFXR_DISSOLVE)] _UseDissolve ("Enable Dissolve", Float) = 0
		[NoScaleOffset] _DissolveTex ("Dissolve Texture", 2D) = "gray" {}
		_DissolveSmooth ("Dissolve Smoothing", Range(0.0001, 0.5)) = 0.1
		[ToggleNoKeyword] _InvertDissolveTex ("Invert Dissolve Texture", Float) = 0
		_MainTex ("Texture", 2D) = "white" {}
		[Toggle(_CFXR_SINGLE_CHANNEL)] _SingleChannel ("Single Channel Texture", Float) = 0
		[Toggle(_CFXR_RADIAL_UV)] _UseRadialUV ("Enable Radial UVs", Float) = 0
		_RingTopOffset ("Ring Offset", Float) = 0.05
		[Toggle(_CFXR_WORLD_SPACE_RING)] _WorldSpaceRing ("World Space", Float) = 0
		[Toggle(_CFXR_HDR_BOOST)] _HdrBoost ("Enable HDR Multiplier", Float) = 0
		_HdrMultiply ("HDR Multiplier", Float) = 2
		[Toggle(_FADING_ON)] _UseSP ("Soft Particles", Float) = 0
		_SoftParticlesFadeDistanceNear ("Near Fade", Float) = 0
		_SoftParticlesFadeDistanceFar ("Far Fade", Float) = 1
		[KeywordEnum(Off,On,CustomTexture)] _CFXR_DITHERED_SHADOWS ("Dithered Shadows", Float) = 0
		_ShadowStrength ("Shadows Strength Max", Range(0, 1)) = 1
		_DitherCustom ("Dithering 3D Texture", 3D) = "black" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	//CustomEditor "CartoonFX.MaterialInspector"
}