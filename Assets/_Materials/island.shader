// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/island"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _ColorTwo ("Color2", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Amount ("Extrusion Amount", Range(-1,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 4.6

        sampler2D _MainTex;
        sampler2D _NoiseTex;

        struct Input {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 uv_MainTex : TEXCOORD0;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _ColorTwo;
        
        float _Amount;
        void vert(inout appdata_full v) {
            //v.vertex.xyz += v.normal * _Amount;
            
            float d = tex2Dlod(_MainTex, float4(2*_Time+v.texcoord.xy,0,0)).r * _Amount;
            v.vertex.xyz += v.normal * d;
            //v.vertexx = mul(unity_ObjectToWorld, v.vertex).xy;
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 t = tex2D(_NoiseTex, IN.uv_MainTex); // - fixed4(1,1,1,1)
            
            fixed4 c = lerp(_Color, _ColorTwo, saturate((t.r))); //tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
