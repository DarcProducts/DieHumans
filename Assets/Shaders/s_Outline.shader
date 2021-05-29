Shader "Custom/s_Outline"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _DistanceFromEdge("Distance From Edge", float) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"="Transparent"}

        Cull Off
        Blend One One
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;

            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float2 uv0 : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv0;
                return o;
            }

            fixed4 frag(Interpolators i) : SV_Target
            {
                
                // sample the texture
                return float4(i.normal, 1);
            }
            ENDCG
        }
    }
}
