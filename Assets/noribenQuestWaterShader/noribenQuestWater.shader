Shader "Noriben/noribenQuestWater"
{
	Properties
	{
		_Color ("MainColor", Color) = (0.1, 0.1, 0.1, 1)
		_MainTex ("WaveTexture", 2D) = "white" {}
		_HeightTex ("HeightTexture", 2D) = "white" {}
		_NormalPower ("NormalPower", Range(0, 1)) = 0.5
		_Diffuse ("Diffuse", Range(0, 1)) = 0.1
		_Specular ("Specular", Range(0, 100)) = 1
		_Reflection ("Reflection", Range(0, 2)) = 1
		_Transparency ("Transparency", Range(0, 1)) = 0.8
		_WaveHeight ("WaveHeight", Range(0, 10)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
		LOD 100
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 pos : TEXCOORD2;
				half3 normal : NORMAL;
				half4 tangent : TANGENT;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 pos : TEXCOORD2; //頂点のワールド座標
				half3 normal : TEXCOORD3;
				half3 lightDir : TEXCOORD4;
			};

			fixed4 _Color;
			float4 _LightColor0;
			sampler2D _MainTex;
			sampler2D _HeightTex;
			float4 _MainTex_ST;
			fixed _Specular;
			fixed _Reflection;
			fixed _Transparency;
			fixed _NormalPower;
			fixed _WaveHeight;
			fixed _Diffuse;
			
			v2f vert (appdata v)
			{
				v2f o;

				float4 heighttex = tex2Dlod(_HeightTex, float4(v.uv + _Time.y * 0.01, 0, 0));
				v.vertex.y = v.vertex.y + heighttex.y * _WaveHeight;
				;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.pos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.normal = UnityObjectToWorldNormal(v.normal);

				TANGENT_SPACE_ROTATION;
				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half3 normalmap = UnpackNormal(tex2D(_MainTex, i.uv + _Time.y * 0.01));
				normalmap = normalize(normalmap);
				normalmap = lerp(float3(0,0,1), normalmap, _NormalPower);
				//環境マップ
				i.normal = normalize(i.normal);
                half3 viewDir = normalize(_WorldSpaceCameraPos - i.pos);
                half3 refDir = reflect(-viewDir, normalmap);
				//キューブマップの空の色を使う場合
				refDir.y *= refDir.y < 0 ? -1 : 1;
                // キューブマップと反射方向のベクトルから反射先の色を取得する
                half4 refColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, refDir, 0);
				refColor = saturate(refColor);
				
				//ライティング
				half3 halfDir = normalize(i.lightDir + viewDir);
				half3 diffuse = max(0, dot(normalmap, i.lightDir)) * _LightColor0.rgb;
				half3 specular = pow(max(0, dot(normalmap, halfDir)), 128.0) * _LightColor0.rgb;


				//mix
				fixed4 col = _Color;
				col.rgb = col.rgb * diffuse * _Diffuse + refColor.rgb * _Reflection + specular * _Specular;
				col = saturate(col);
				col = fixed4(fixed3(col.xyz), _Transparency);

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
