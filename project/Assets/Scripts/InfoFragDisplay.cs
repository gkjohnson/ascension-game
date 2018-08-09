using UnityEngine;
using System.Collections;

//icon that displays the fragment model when an item is selected
public class InfoFragDisplay : MonoBehaviour {
	
	void Start(){
		transform.RotateAround(new Vector3(0,0,1), 45.0f);
		transform.localScale = new Vector3(3,3,3);
	}
	
	// animate the model and position it in front of the camera
	void Update () {
		Camera c = (Camera)transform.parent.GetComponentInChildren(typeof(Camera));
		transform.position = c.ViewportToWorldPoint(new Vector3 ( (Screen.width - 115.0f)/Screen.width, 1 - (220.0f)/Screen.height, 50));
		
		transform.RotateAround(new Vector3(0,1,0), .025f);
		
	}
}
