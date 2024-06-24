Shader "fabx/VR_SkyShader"
{
    Properties
    {
        _SkyColor("Sky Color", Color) = (0.5, 0.7, 1, 1)
        _HorizonColor("Horizon Color", Color) = (1, 0.7, 0.5, 1)
        _SunDirection("Sun Direction", Vector) = (0, 1, 0)
        _SunColor("Sun Color", Color) = (1, 1, 0.5, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _SkyColor;
            float4 _HorizonColor;
            float4 _SunColor;
            float3 _SunDirection;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 dir = normalize(i.worldPos - _WorldSpaceCameraPos);
                float t = dot(dir, _SunDirection);
                fixed4 col = lerp(_HorizonColor, _SkyColor, dir.y * 0.5 + 0.5);
                col += _SunColor * pow(max(t, 0), 128);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
