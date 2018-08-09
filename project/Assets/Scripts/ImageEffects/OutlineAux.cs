using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]

//Created by the outline component on the instantiated camera - renders to a rendertexture
public class OutlineAux : MonoBehaviour {
	
	public Shader shaderMat;
	public RenderTexture tex;
	
	//shader variables
	public int _Thickness;
	public Color _Color;
	
	//create a rendertexture and set it as the target. Also find the appropriate shader
	void Start(){
		tex = new RenderTexture(Screen.width, Screen.height, 1024);
		this.camera.targetTexture = tex;
		
		shaderMat = Shader.Find("Hidden/OutlineShader");
	}
	
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		//camera.cullingMask = 1;//LayerMask.NameToLayer("Default");
		//print(LayerMask.NameToLayer("Default"));
		
		//create a material with the shader from before
		Material mat = new Material(shaderMat);
		
		//set the variables in the shader
		mat.SetVector("_dimensions", new Vector4(source.width, source.height,0,0));
		mat.SetVector("_color", _Color);
		mat.SetFloat("_thickness", _Thickness);
		
		//take the rendered models and create an outline around them
		Graphics.Blit(source,destination,mat);		
		
	}
}
