using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]

//add to camera - creates an outline around any gameobject in the set layermask (test)
public class Outline : MonoBehaviour {
	
	//materials to blit to teh screen
	Material unlitMat;
	Material transMat;
	
	//variables for the outlines
	public Color OutlineColor = new Color(1,0,0,1);
	public int Thickness = 1;
	public LayerMask CullingMask;
	
	//camera for the outlines only
	public Camera cam;
	
	void Start(){
		
		//create a camera and parent to this camera
		if(!cam){
			cam = (Camera)Instantiate(camera);
		
			cam.depth= -1;
			cam.transform.parent=transform;
		}
		
		//get rid of all components except for camera and transform
		Component[] comp = cam.GetComponents(typeof(Component));
		for(int i = 0 ; i < comp.Length ; i++){
			if(
			comp[i].GetType() == typeof(Camera) || 
			comp[i].GetType() == typeof(Transform)
			){
				continue;
			}
			
			Destroy(comp[i]);
		}
		
		//set up the new camera for the outline
		cam.gameObject.AddComponent("OutlineAux");

		cam.clearFlags = CameraClearFlags.SolidColor;
		
		cam.backgroundColor = new Color(0,0,0,0);

		//set up the materials for the transmat and unlitmat
		unlitMat = new Material(Shader.Find("Unlit/Texture"));
		transMat = new Material(Shader.Find("Unlit/Transparent"));
		transMat.color= new Color(1,1,1,1);
		unlitMat.color = new Color(1,1,1,0);
	}
	
	//match the values of the outline camera to match those of this camera
	void LateUpdate(){
		cam.fieldOfView = camera.fieldOfView;
		cam.orthographicSize = camera.orthographicSize;
		cam.cullingMask = CullingMask;
		
		OutlineAux o = (OutlineAux)Component.FindObjectOfType(typeof(OutlineAux));
		o._Color = OutlineColor;
		o._Thickness = Thickness;
				
	}
	
	void OnRenderImage (RenderTexture source, RenderTexture destination){
		
		 
		/*Graphics.Blit(source,destination,unlitMat);

		OutlineAux o = (OutlineAux)Component.FindObjectOfType(typeof(OutlineAux));
		Graphics.Blit(o.tex,destination,transMat);
		*/
		
		//cullingmask is reset to avoid layer issues
		cam.cullingMask = camera.cullingMask;
		
		OutlineAux o = (OutlineAux)Component.FindObjectOfType(typeof(OutlineAux));
		
		
		//add the rendertexture from the outline component onto what ws rendered by this camer
		Graphics.Blit(o.tex,source,transMat);
		
		//set the destination to the source
		Graphics.Blit(source,destination,unlitMat);
		
	}
}
