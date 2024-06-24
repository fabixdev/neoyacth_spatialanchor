Shader "fabx/WaterShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1
        _WaveScale ("Wave Scale", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _NormalMap;
        float _WaveSpeed;
        float _WaveScale;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);

            float wave = sin(_WaveSpeed * _Time.y + IN.worldPos.x * _WaveScale) * 0.1;
            IN.uv_NormalMap.y += wave;

            half4 normal = tex2D (_NormalMap, IN.uv_NormalMap);
            o.Normal = UnpackNormal(normal);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Transparent/Cutout/VertexLit"
}
