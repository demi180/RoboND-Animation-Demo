Shader "Custom/Sphere"
{
	Properties
	{
		 _Color ("Main Color", Color) = (1,1,1,1)
		 _MainTex ("Main Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100
		zwrite off
		ztest always

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            // color from the material
            fixed4 _Color;

            v2f vert (appdata v)
//			float4 vert (float4 vertex : POSITION) : SV_POSITION
            {
            	v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
//                return UnityObjectToClipPos(vertex);
            }

            // pixel shader, no inputs needed
            fixed4 frag (v2f i) : SV_Target
//            fixed4 frag () : SV_Target
            {
            	// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				col.a = _Color.a;
				return col;
//                return _Color; // just return it
            }
			ENDCG
		}
	}
}
