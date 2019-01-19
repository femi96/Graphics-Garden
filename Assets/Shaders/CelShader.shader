Shader "Custom/CelShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
    _Treshold ("Cel treshold", Range(1.0, 20.0)) = 5.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

    CGPROGRAM
    #pragma surface surf CelShadingForward
    #pragma target 3.0

    float _Treshold;

		half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten) {
      half NdotL = dot(s.Normal, lightDir);
      // if (NdotL <= 0.0) NdotL = 0;
      // else NdotL = 1;
      NdotL = floor(NdotL * _Treshold) / (_Treshold - 0.5);

      half4 c;
      c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
      // c.r = floor(c.r * _Treshold) / (_Treshold - 0.5);
      // c.g = floor(c.g * _Treshold) / (_Treshold - 0.5);
      // c.b = floor(c.b * _Treshold) / (_Treshold - 0.5);
      // c.r = 0;
      // c.g = 0;
      // c.b = 0;
      c.a = s.Alpha;
      return c;
    }

    sampler2D _MainTex;
    fixed4 _Color;

    struct Input {
      float2 uv_MainTex;
    };

    void surf(Input IN, inout SurfaceOutput o) {
      // Albedo comes from a texture tinted by color
      fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
      o.Albedo = c.rgb;
      o.Alpha = c.a;
    }
    ENDCG
	}
	FallBack "Diffuse"
}
