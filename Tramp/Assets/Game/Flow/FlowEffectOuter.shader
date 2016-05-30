Shader "MyTransparent"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}
	}
		SubShader{
		Cull[_Cull]
		//============================
		// レンダリング用のPass
		CGPROGRAM
#pragma surface surf Standard

		half4 _Color;
	sampler3D	_DitherMaskLOD;

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

	void surf(Input IN, inout SurfaceOutputStandard o) {

		// ディザリングで半透明を表現
		half alphaRef = tex3D(_DitherMaskLOD, float3(IN.screenPos.xy / IN.screenPos.w * _ScreenParams.xy * 0.25, _Color.a*0.9375)).a;
		clip(alphaRef - 0.01);

		// 色
		o.Albedo = lerp(_Color.rgb,float3(1,1,1),mod(IN.uv_MainTex.y-_Time.y,1.0));
		o.Alpha = _Color.a;
	}
	ENDCG
	}
	FallBack "Differd"
}