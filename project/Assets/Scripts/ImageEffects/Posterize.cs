using UnityEngine;
using System.Collections;

//post image effect that rounds each component of a color to the nearest value using a discrete number of steps
public class Posterize : MonoBehaviour {
	
	public Shader shaderMat;
	
	//find the necessary shader
	void Start(){
		//tex = new RenderTexture(Screen.width, Screen.height, 1024);
		//this.camera.targetTexture = tex;
		
		shaderMat = Shader.Find("Hidden/Posterize");
	}
	//number of steps in the colors
	public float val = 1;
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		//create the material
		Material mat = new Material(shaderMat);
		//pass the number of steps to the shader
		mat.SetFloat("_steps", val);// Mathf.Sin(Time.time/val)*100);
		//run it on the image
		Graphics.Blit(source,destination,mat);
	}
}
