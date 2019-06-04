﻿

//アウトラインのshader、単色のテキスチャーをoutする

Shader "OutlineShader/OutlinePrePass"
{
	/*Properties{
		
		_OutlineColor("OutlineColor", Color) = (1,1,0,1)
	}*/
	
	SubShader
	{
		
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
				/*fixed4 _OutlineColor;*/

				struct v2f
				{
					float4 pos : SV_POSITION;
				};

				v2f vert(appdata_full v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{

					//単色のテキスチャーをoutする
					return fixed4(1,1,0,1);
				}

				
				#pragma vertex vert
				#pragma fragment frag
			ENDCG
		}
	}
}
