#ifndef RGBAMASK_LIT_INPUT_INCLUDED
#define RGBAMASK_LIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"

// NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
half _BumpScale;
half _Smoothness;
half _Metallic;
half _OcclusionStrength;
half _Surface;
half _Cutoff;
CBUFFER_END

// NOTE: Do not ifdef the properties for dots instancing, but ifdef the actual usage.
// Otherwise you might break CPU-side as property constant-buffer offsets change per variant.
// NOTE: Dots instancing is orthogonal to the constant buffer above.
#ifdef UNITY_DOTS_INSTANCING_ENABLED
UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)
UNITY_DOTS_INSTANCED_PROP(float, _Smoothness)
UNITY_DOTS_INSTANCED_PROP(float, _Metallic)
UNITY_DOTS_INSTANCED_PROP(float, _BumpScale)
UNITY_DOTS_INSTANCED_PROP(float, _OcclusionStrength)
UNITY_DOTS_INSTANCED_PROP(float, _Surface)
UNITY_DOTS_INSTANCED_PROP(float, _Cutoff)
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

#define _BaseColor              UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata_BaseColor)
#define _Smoothness             UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Smoothness)
#define _Metallic               UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Metallic)
#define _BumpScale              UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_BumpScale)
#define _OcclusionStrength      UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_OcclusionStrength)
#define _Surface                UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Surface)
#define _Cutoff                 UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Cutoff)
#endif

TEXTURE2D(_MetallicGlossMap);   SAMPLER(sampler_MetallicGlossMap);
TEXTURE2D(_RGBAMaskMap);
half4 _RColor;
half4 _GColor;
half4 _BColor;

inline void InitializeStandardLitSurfaceData(float2 uv, out SurfaceData outSurfaceData)
{
	half4 albedo = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
	half4 mask = SAMPLE_TEXTURE2D(_RGBAMaskMap, sampler_BaseMap, uv);
	half3 mixcolor = _RColor.rgb * mask.r + _GColor.rgb * mask.g + _BColor.rgb * mask.b + albedo * (1 - mask.r - mask.g - mask.b);

	outSurfaceData.alpha = albedo.a;
	outSurfaceData.albedo = mixcolor.rgb * _BaseColor.rgb;

	half4 specGloss = SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, uv);
	outSurfaceData.metallic = specGloss.r * _Metallic;
	outSurfaceData.smoothness = specGloss.a * _Smoothness;
	//outSurfaceData.occlusion = specGloss.g * _OcclusionStrength;
	outSurfaceData.occlusion = _OcclusionStrength;
	outSurfaceData.specular = half3(0.0, 0.0, 0.0);

	outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);

	outSurfaceData.emission = 0;
	outSurfaceData.clearCoatMask = half(0.0);
	outSurfaceData.clearCoatSmoothness = half(0.0);
}

#endif // RGBAMASK_LIT_INPUT_INCLUDED
