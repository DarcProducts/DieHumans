Shader "Custom/VR/VR With 2 Color"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color 1", Color) = (1, 1, 1, 1)
		_ColorAlt("Color 2", Color) = (0, 0, 0, 1)
	}
		SubShader
		{
			Tags
			{
				"RenderType" = "Opaque"
			}

				Cull Off
				//Blend One One
				//ZWrite Off

				Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					fixed4 vertex : POSITION;
					fixed2 uv : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					fixed2 uv : TEXCOORD0;
					fixed4 vertex : SV_POSITION;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				sampler2D _MainTex;
				fixed4 _MainTex_ST;
				fixed4 _Color;
				fixed4 _ColorAlt;

				v2f vert(appdata v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_OUTPUT(v2f, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv);
				
					if (col.x + col.y + col.z < .1f)
						return _ColorAlt;
					return col * _Color;
				}
				ENDCG
			}
		}
}