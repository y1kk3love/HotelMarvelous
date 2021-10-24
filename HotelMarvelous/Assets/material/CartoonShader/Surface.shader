// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Surface"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _StairNum ("Stair Num", float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue" = "AlphaTest" }     
        Blend SrcAlpha OneMinusSrcAlpha
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
                
                float3 worldSpaceLightDir = normalize(_WorldSpaceLightPos0);
                float ndotl = dot(worldSpaceLightDir, i.normal);
                float halfLambert = ndotl * 0.5 + 0.5;
                float floorToon = floor(halfLambert * _StairNum) * (1/_StairNum);
                col = MainTex * floorToon;
                
                if(MainTex.a == 0)
                {
                    col.a = 0;
                }
                else
                {
                    col.a = 1;
                }

                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
                
            sampler2D _MainTex;
            fixed _Cutoff;

            struct v2f 
            { 
                V2F_SHADOW_CASTER; 
                float2 uv : TEXCOORD1;
            }
                
            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos( v.vertex ); 
                o.uv = v.texcoord;
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
                
            float4 frag(v2f IN) : COLOR
            {
                fixed4 c = tex2D( _MainTex, IN.uv );
                clip( c.a - _Cutoff );
                SHADOW_CASTER_FRAGMENT(IN)
            }
            ENDCG
        }  

    }
}
