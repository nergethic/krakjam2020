Shader "Unlit/laser"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1, 0, 0, 0)
        _Speed ("Speed", float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _Mask;
            
            float4 _MainTex_ST;
            float4 _Color;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float multipliedVal = _Time * _Speed;
                float2 movedUV = i.uv + multipliedVal;
                fixed4 mainTextSample = tex2D(_MainTex, movedUV);
                fixed4 maskSample = tex2D(_Mask, i.uv);
                fixed3 col = mainTextSample.rgb * maskSample.rgb * _Color.rgb;
                col *= i.color.rgb;
                
                float alpha = mainTextSample.a * maskSample.r * _Color.a * i.color.a;
                
                return fixed4(col, alpha);
            }
            ENDCG
        }
    }
}
