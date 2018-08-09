using UnityEngine;
using System.Collections;

//soul vessel represents the final buildign to merge into.
public class SoulVessel : MonoBehaviour {
	
	public Light glowLight;
	Unit u;
	
	//add a light with the team color to the object
	void Start () {
		u= (Unit)GetComponent("Unit");

		
		GameObject go = new GameObject();
		
		go.AddComponent("Light");
		
		glowLight = (Light) go.GetComponent("Light");

		glowLight.type = LightType.Point;
		
		glowLight.transform.parent = this.transform;
		glowLight.transform.position = this.transform.position + new Vector3(0,6,0);
		glowLight.range=77.15f;
		glowLight.intensity=1f;
				
		glowLight.color = PlayerManager.PM.getColor(u.Player)*1f;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		glowLight.intensity = (Mathf.Sin(Time.time/2) * .5f + .5f)*.4f + .1f;
		
		//float mod = (1+Mathf.Sin(Time.timeSinceLevelLoad*timeMod*1.2f))*.5f;
		//c.a = mod*.1f + .15f;
		
		transform.position = u.CurrentTile.transform.position + new Vector3(0,1,0)*Mathf.Sin(Time.timeSinceLevelLoad );
		transform.RotateAroundLocal(Vector3.up, 2*Mathf.PI*Time.deltaTime/100);
	}
	
	
	bool winner = false;
	
	int timer = 10;
	
	//set this soul vessel as having won
	public void setWin(){
		
		timer--;
		
		//when the timer runs out, play a dig animation around the unit
		if(timer==0){
			Tile[] t = u.CurrentTile._adjacentTiles;
			for(int i=0;i<t.Length;i++){
				if(!t[i])continue;
				t[i].Dig(1,false);
			}
			u.CurrentTile.incHeight(1);
		}
		if(winner)return;
		winner=true;
		
		//notify the plane manager that it has won so it can zoom in and play animation
		PlaneManager.PM.SetWinner(this);
		
		//add particles
		GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/WinParticles"));
		go.transform.position = this.transform.position;
		go.transform.parent = this.transform;
	}
}
