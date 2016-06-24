Shader "Muteki"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
	    _MainTex("Texture", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+5" }
		//Cull[_Cull]
		//ZWrite Off
		ZTest Always
		//============================
		// レンダリング用のPass
		CGPROGRAM
#pragma surface surf Lambert 

		half4 _Color;
	sampler3D	_DitherMaskLOD;
	sampler2D _MainTex;

	// 入力構造体
	struct Input {
		float2 uv_MainTex;
		float3 worldPos;
		float4 screenPos;
	};

	float mod(float a, float b)
	{
		return a - floor(a / b) * b;
	}

	void surf(Input IN, inout SurfaceOutput o) {

		//// ディザリングで半透明を表現
		//half alphaRef = tex3D(_DitherMaskLOD, float3(IN.screenPos.xy / IN.screenPos.w * _ScreenParams.xy * 0.25, _Color.a*0.9375)).a;
		//clip(alphaRef - 0.01);

		// 色
		o.Albedo = _Color.rgb;
		o.Alpha = _Color.a;
		o.Emission = _Color.rgb*tex2D(_MainTex,IN.uv_MainTex).rgb;
	}
	ENDCG
	}
	FallBack "Differd"
}