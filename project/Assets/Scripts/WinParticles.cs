using UnityEngine;
using System.Collections;

//particles that get attached to the soul vessel when the game is won
public class WinParticles : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//ParticleSystem ps = (ParticleSystem)GetComponent("ParticleSystem");
	}
	
	int speed =-1;
	// Update is called once per frame
	void Update () {
		this.transform.Rotate(new Vector3(0,0,1),speed);
	}
}
