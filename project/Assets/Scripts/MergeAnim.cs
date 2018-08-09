using UnityEngine;
using System.Collections;

//Particle animation for when two souls merg into each other
public class MergeAnim : MonoBehaviour {
	
	//the sart point and target
	GameObject target;
	Vector3 start;
	
	Vector3 currPos;
	
	bool finished = false;
	
	// set the color of the particles
	void Start () {
		Color c = GeneralManager.GenMan.PlayerMan.PlayerColors[GeneralManager.GenMan.PlayerMan.CurrTurn];
		c.a=1;
		
		c*=.35f;
		c+= new Color(1,1,1) *.65f;
		c.a=1;
		
		particleSystem.startColor = c;// GeneralManager.GenMan.PlayerMan.PlayerColors[GeneralManager.GenMan.PlayerMan.CurrTurn];
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//destroy if the target is gone or it has finished the animation
		if(finished || target ==null){
			particleSystem.enableEmission=false;
			particleSystem.Stop();
			if(!particleSystem.isPlaying){
				Destroy(this.gameObject);
			}
			return;	
		}
		
		//model a parabola over the vector between the target and the start
		Vector3 dir = target.transform.position - start;
		Vector3 distTo = currPos - target.transform.position;
			
		currPos-=distTo/50 + distTo.normalized*.375f;
		
		float delta = (dir.magnitude/2 - distTo.magnitude) ;
		delta*=delta;
	
		delta = (dir.magnitude/2)*(dir.magnitude/2) - delta;
		delta/=(dir.magnitude/2)*(dir.magnitude/2);
		delta *=30.0f;
		//print (delta + " " +(dir.magnitude/2)*(dir.magnitude/2));
		
		Vector3 transPos = currPos;
		transPos.y += delta/5;
		
		this.transform.position = transPos;
		
		//end onces it gets close enough
		if(distTo.sqrMagnitude<.5){
			finished=true;

			particleSystem.startSize=2;
			particleSystem.startLifetime=1.5f;
			particleSystem.emissionRate=2000;
			particleSystem.gravityModifier = 0.25f;
			particleSystem.startSpeed = 18;
		}
	}
	
	//set the objects that act as the start and end points
	public void setTarget(GameObject s, GameObject g){
		target=g;
		start=s.transform.position + new Vector3(0,2,0);
		currPos=start;
	}
	
}
