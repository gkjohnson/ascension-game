Shader "Hidden/OutlineShader" {
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
			
			uniform float4 _dimensions;
			uniform float _thickness;
			uniform fixed4 _color;
			
			struct v2f {
			    float4 pos : POSITION;
			    float3 color : COLOR0;
			   	float2 uv	: TEXCOORD0;
			};
			
			float2 TexToPix(float2 uv, float2 dim){
				return uv*dim;
			}
			
			float2 PixToTex(float2 pix, float2 dim){
				return float2( pix.x / dim.x, pix.y / dim.y);
			}
			
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
				if(col.w != 0.0f){
					return fixed4(0,0,0,0);
				}else{							
					float2 dim = _dimensions.xy;
					float2 pix = TexToPix(i.uv,dim);
					
					fixed4[4] dirCol;
					dirCol[0] = tex2D(_MainTex, PixToTex(pix + float2(_thickness,0),dim));
					dirCol[1] = tex2D(_MainTex, PixToTex(pix + float2(-_thickness,0),dim));
					dirCol[2] = tex2D(_MainTex, PixToTex(pix + float2(0,_thickness),dim));
					dirCol[3] = tex2D(_MainTex, PixToTex(pix + float2(0,-_thickness),dim));
					
						
					
					if(col.w == 0.0f && (dirCol[0].w != 0.0f || dirCol[1].w != 0.0f || dirCol[2].w != 0.0f || dirCol[3].w != 0.0f)){
						return _color;
					}else{
						return fixed4(0,0,0,0);
					}
				}
			}
			ENDCG
		}
	} 
	FallBack Off
}
