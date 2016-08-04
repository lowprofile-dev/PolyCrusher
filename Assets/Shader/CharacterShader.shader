﻿Shader "POLYCRUSHER/CharacterShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Occlusion ("Occlusion Map", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_FillColor ("FillColor", Color) = (1,1,1,1)
		_FillEmission ("FillEmission", Color) = (1,1,1,1)
	}
	SubShader {
		ZWrite Off
        ZTest Off
        Lighting Off
		Cull Back

		CGPROGRAM
         #pragma surface surf Standard //alpha
 
         sampler2D _MainTex;
		 fixed4 _FillColor;
		 fixed4 _FillEmission;

         struct Input {
             float2 uv_MainTex;
         };
 
         void surf (Input IN, inout SurfaceOutputStandard o) {
             //half4 c = tex2D (_MainTex, IN.uv_MainTex);
             o.Albedo = _FillColor.rgb;
             o.Alpha = _FillColor.a;
			 o.Emission = _FillEmission;
         }
         ENDCG

		Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On
        ZTest LEqual
		Lighting On
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard //alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Occlusion;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Occlusion = tex2D (_Occlusion, IN.uv_MainTex);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Color.a;
		}

		ENDCG
    }

	FallBack "Diffuse"
}