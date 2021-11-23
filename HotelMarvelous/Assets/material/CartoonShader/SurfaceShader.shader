// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SurfaceShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _StairNum ("Stair Num", float) = 2
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull off

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _StairNum;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz;   //월드에서의 노말 벡터값
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col;
                fixed4 MainTex = tex2D(_MainTex, i.uv);
                clip(MainTex.a - 0.5);

                float3 worldSpaceLightDir = normalize(_WorldSpaceLightPos0);

                float ndotl = dot(worldSpaceLightDir, i.normal);
                float halfLambert = ndotl * 0.5 + 0.5;

                float floorToon = floor(halfLambert * _StairNum) * (1/_StairNum);
                
                col = MainTex * floorToon;

                return col;
            }
            ENDCG
        }

        Pass
        {
            Tags { "LightMode" = "ShadowCaster" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct v2f
            {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
            };

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.uv = v.texcoord;
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D( _MainTex, i.uv );
				clip( c.a - 0.5 );
				SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
          
    }
}
