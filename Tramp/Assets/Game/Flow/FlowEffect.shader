Shader "Custom/FlowEffect" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	    _Volume("volume", Float) = 0.0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200
			Lighting off
			Cull[_Cull]

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Lambert  noshadow noforwardadd

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
			};

			fixed4 _Color;
			float _Volume;

			float mod(float a, float b)
			{
				return a - floor(a / b) * b;
			}

			void surf(Input IN, inout SurfaceOutput o) {
				// Albedo comes from a texture tinted by color
				//IN.uv_MainTex;

				float2 UV = float2(IN.uv_MainTex.x,mod(_Time.y,1.0f));

				float4 col = tex2D(_MainTex, UV);
				
				float speed = 5;
				float LineNumber = 5;
				float time = mod((_Time.y+(((col.a*_Volume)/30.0f)-0.5f))*speed,1.0f);
				float texy = mod(IN.uv_MainTex.y*LineNumber,1.0);
				if (!(texy <  time&& texy>time - 0.1))
					clip(-1.0);
				

				fixed4 c =  _Color;

				o.Albedo = c.rgb;
				o.Alpha = c.a;
				o.Emission =c.rgb;
			}


			ENDCG
	}
			FallBack Off
}
