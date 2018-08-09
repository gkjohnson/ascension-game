Shader "Hidden/Posterize" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
			Pass{
			Lighting Off ZTest Always Cull Off ZWrite Off Fog { Mode off }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			
			struct v2f {
			    float4 pos : POSITION;
			    float3 color : COLOR0;
			   	float2 uv	: TEXCOORD0;
			};
			
			//float2 TexToPix(float2 uv, float2 dim){
			//	return uv*dim;
			//}
			
			//float2 PixToTex(float2 pix, float2 dim){
			//	return float2( pix.x / dim.x, pix.y / dim.y);
			//}
			
			float _steps = 1;
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord);				
				
			    return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{			
				
				fixed4 col = tex2D(_MainTex, i.uv);
				
				return col*col*_steps + col*1f;
				
				float x = _steps;
				int y = 2;
				
				col.r=(int((col.r*x)/y)*y)/x;
				col.g=(int((col.g*x)/y)*y)/x;
				col.b=(int((col.b*x)/y)*y)/x;
				
				
				
				return col;
			}
			ENDCG
		}
	} 
	FallBack Off
}
