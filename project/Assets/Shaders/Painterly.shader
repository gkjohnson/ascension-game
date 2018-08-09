Shader "Hidden/Painterly" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_StrokeTex ("Base (RGB)", 2D) = "gray" {}
	}
	SubShader {
			Pass{
			Lighting Off ZTest Always Cull Off ZWrite Off Fog { Mode off } Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform sampler2D _StrokeTex;
			uniform float4 _StrokeOffsetScale;
			
			struct v2f {
			    float4 pos : POSITION;
			    float3 color : COLOR0;
			   	float2 uv	: TEXCOORD0;
			   	float2 suv  : TEXCOORD1;
			   	float2 uv2 : TEXCOORD2;
			};
			
			//float2 TexToPix(float2 uv, float2 dim){
			//	return uv*dim;
			//}
			
			//float2 PixToTex(float2 pix, float2 dim){
			//	return float2( pix.x / dim.x, pix.y / dim.y);
			//}
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord);				
				o.suv = o.uv * _StrokeOffsetScale.zw + _StrokeOffsetScale.xy;
			    return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{			
				i.uv.y = int(i.suv.y)- _StrokeOffsetScale.y +.5;
				i.uv.x = int(i.suv.x)- _StrokeOffsetScale.x +.5;
				
				i.uv /= _StrokeOffsetScale.zw ;
				
				fixed4 col = tex2D(_MainTex, i.uv) * tex2D(_StrokeTex, i.suv);
				col.a = tex2D(_StrokeTex, i.suv).a;
				
				if(length(tex2D(_MainTex, i.uv2)) > length(col)){
					col = tex2D(_MainTex, i.uv2);
				}
				//if(col.a != 0)col = fixed4(1,0,0,1);
				//col = fixed4(1,0,0,1);
				
				
				return col;
			}
			ENDCG
		}
	} 
	FallBack Off
}
