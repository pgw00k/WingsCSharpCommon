/*
* 2022_11_22
* 使用 URP 12.1.6 版本的 Lit 作为 Base 进行修改
* 只采用 Metallic 的 workflow
* 删除了低版本的 subshader
* 
*/

Shader "Wings/RGBAMaskLit"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)

        _BumpScale("BumpScale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _MetallicGlossMap("Metallic", 2D) = "white" {}
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        _OcclusionStrength("OcclusionStrength", Range(0.0, 1.0)) = 1.0

        _RGBAMaskMap("Mask Map", 2D) = "black" {}
        _RColor("Color 1 (R)", Color) = (1,0,0,1)
        _GColor("Color 2 (R)", Color) = (0,1,0,1)
        _BColor("Color 3 (R)", Color) = (0,0,1,1)

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0

        // Blending state
        [HideInInspector] _Cutoff("__cutoff", Float) = 0.0
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0

        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }

        SubShader
    {
        // Universal Pipeline tag is required. If Universal render pipeline is not set in the graphics settings
        // this Subshader will fail. One can add a subshader below or fallback to Standard built-in to make this
        // material work with both Universal Render Pipeline and Builtin Unity Pipeline
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True" "ShaderModel" = "4.5"}
        LOD 300

        // ------------------------------------------------------------------
        //  Forward pass. Shades all light in a single pass. GI + emission + Fog
        Pass
        {
            // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
            // no LightMode tag are also rendered by Universal Render Pipeline
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

        // -------------------------------------
        // Material Keywords
        #pragma shader_feature_local _NORMALMAP
        //#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
        #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
        #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
        #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF

        // -------------------------------------
        // Universal Pipeline keywords
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile_fragment _ _LIGHT_COOKIES
        #pragma multi_compile _ _CLUSTERED_RENDERING

        // -------------------------------------
        // Unity defined keywords
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile_fog
        #pragma multi_compile_fragment _ DEBUG_DISPLAY

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON

        #pragma vertex LitPassVertex
        #pragma fragment LitPassFragment
        //#pragma fragment RGBAMaskLitPassFragment

        #include "Library/RGBAMaskLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"
        //#include "Library/RGBAMaskLitPass.hlsl"
        ENDHLSL
    }

    Pass
    {
        Name "ShadowCaster"
        Tags{"LightMode" = "ShadowCaster"}

        ZWrite On
        ZTest LEqual
        ColorMask 0
        Cull[_Cull]

        HLSLPROGRAM
        #pragma exclude_renderers gles gles3 glcore
        #pragma target 4.5

        // -------------------------------------
        // Material Keywords
        //#pragma shader_feature_local_fragment _ALPHATEST_ON
        //#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON

        // -------------------------------------
        // Universal Pipeline keywords

        // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

        #pragma vertex ShadowPassVertex
        #pragma fragment ShadowPassFragment

        #include "Library/RGBAMaskLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
        ENDHLSL
    }

    Pass
    {
            // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
            // no LightMode tag are also rendered by Universal Render Pipeline
            Name "GBuffer"
            Tags{"LightMode" = "UniversalGBuffer"}

            ZWrite[_ZWrite]
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

        // -------------------------------------
        // Material Keywords
        #pragma shader_feature_local _NORMALMAP
        //#pragma shader_feature_local_fragment _ALPHATEST_ON
        //#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
        //#pragma shader_feature_local_fragment _EMISSION
        #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
        //#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        //#pragma shader_feature_local_fragment _OCCLUSIONMAP
        //#pragma shader_feature_local _PARALLAXMAP
        //#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

        //#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
        #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
        //#pragma shader_feature_local_fragment _SPECULAR_SETUP
        //#pragma shader_feature_local _RECEIVE_SHADOWS_OFF

        // -------------------------------------
        // Universal Pipeline keywords
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED

        // -------------------------------------
        // Unity defined keywords
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON

        #pragma vertex LitGBufferPassVertex
        #pragma fragment LitGBufferPassFragment

        #include "Library/RGBAMaskLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/LitGBufferPass.hlsl"
        ENDHLSL
    }

    Pass
    {
        Name "DepthOnly"
        Tags{"LightMode" = "DepthOnly"}

        ZWrite On
        ColorMask 0
        Cull[_Cull]

        HLSLPROGRAM
        #pragma exclude_renderers gles gles3 glcore
        #pragma target 4.5

        #pragma vertex DepthOnlyVertex
        #pragma fragment DepthOnlyFragment

        // -------------------------------------
        // Material Keywords
        //#pragma shader_feature_local_fragment _ALPHATEST_ON
        //#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON

        #include "Library/RGBAMaskLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
        ENDHLSL
    }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

        // -------------------------------------
        // Material Keywords
        #pragma shader_feature_local _NORMALMAP
        //#pragma shader_feature_local _PARALLAXMAP
        //#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
        //#pragma shader_feature_local_fragment _ALPHATEST_ON
        //#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON

        #include "Library/RGBAMaskLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"
        ENDHLSL
    }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaLit

            #pragma shader_feature EDITOR_VISUALIZATION
            //#pragma shader_feature_local_fragment _SPECULAR_SETUP
            //#pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            //#pragma shader_feature_local_fragment _ALPHATEST_ON
            //#pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            //#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

            //#pragma shader_feature_local_fragment _SPECGLOSSMAP

            #include "Library/RGBAMaskLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"

            ENDHLSL
        }
    }
        FallBack "Hidden/Universal Render Pipeline/FallbackError"
        //CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.LitShader"
}
