Shader "Unlit/Outline"
{
    Properties
    {
        _Outline_Bold ("Outline Bold", Range (0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull front
        Pass
        {
            CGPROGRAM
            #pragma vertex _VertexFuc
            #pragma fragment _FragmentFuc
            #include "UnityCG.cginc"

            struct ST_VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct ST_VertexOutput
            {
                float4 vertex : SV_POSITION;
            };

            float _Outline_Bold;

            ST_VertexOutput _VertexFuc (ST_VertexInput stInput)
            {
                ST_VertexOutput stOutput;

                float3 fNormalized_Normal = normalize(stInput.normal);
                float3 fOutline_Position = stInput.vertex + fNormalized_Normal * (_Outline_Bold * 0.1f);

                stOutput.vertex = UnityObjectToClipPos(fOutline_Position);
                return stOutput;
            }

            fixed4 _FragmentFuc (ST_VertexOutput i) : SV_Target
            {
                return 0.0f;
            }
            ENDCG
        }
    }
}
