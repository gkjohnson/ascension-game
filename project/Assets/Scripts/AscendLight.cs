using UnityEngine;
using System.Collections;

public class AscendLight : MonoBehaviour {
	
	//attached shrines mean that they can be used to change the highlight of the light
	public Shrine [] _shrines = new Shrine[6];
	public bool del=false;
	
	public bool independent = false;
	public float timeMod=1.0f;
	
	public AscendLight Above = null;
	// Update is called once per frame
	void Update () {
		//do not manually update color if there is an ascend light below this.
		if(Above==null && !independent)return;
		
		ParticleSystem ps =GetComponent<ParticleSystem>();

		Color c = renderer.sharedMaterial.GetColor("_TintColor");
		
		//if it's not going to be deleted
		if(!del){
			//oscillate the color of the tile
			float mod = (1+Mathf.Sin(Time.timeSinceLevelLoad*timeMod*1.2f))*.5f;
			c.a = mod*.1f + .15f;
						
			// Decides when to highlight the ascend lights
			Tile t = ((Tile)this.transform.parent.GetComponent("Tile"));
			if(t.Resident){
				//if theres a unit on the tile, check if it is next to a relted shrine or temple
				//if it is, highlight it
				for(int i=0;i<t._adjacentTiles.Length;i++){
					if(t._adjacentTiles[i] && t._adjacentTiles[i].Resident){
						Shrine s = (Shrine)t._adjacentTiles[i].Resident.GetComponent("Shrine");
						if(isAttached(s) && 
						t.Resident.Player == t._adjacentTiles[i].Resident.Player && 
						t.Resident.GetComponent("Soul") && 
						((Soul)t.Resident.GetComponent("Soul")).Souls >= s.SOULS_TO_ASCEND){
							c.a=.55f;
						}
					}
				}
			}
		}else{
			//if it is ready to be deleted, stop the particles, and fade
			c.a = 0;
			ps.enableEmission=false;
			ps.loop=false;
			
		}
		//"chase" or ease to the destination alpha
		c = renderer.sharedMaterial.GetColor("_TintColor") - (renderer.sharedMaterial.GetColor("_TintColor") - c)/10;
		
		//once it has faded enough, delete
		if(c.a <= .025f && del && (!ps.IsAlive() || independent) )Destroy(this.gameObject);
				
		this.renderer.sharedMaterial.SetColor("_TintColor",c);
	}
	
	//set the tile to stick to
	public void setTile(Tile t){
		transform.parent = t.transform;
		transform.position = t.transform.position;
	}
	
	//check if shrine s is attached
	bool isAttached(Shrine s){
		if(s==null)return false;
		for(int i=0; i < _shrines.Length;i++){
			if(_shrines[i] == s)return true;
		}
		return false;
	}
	
	//attach shrine s to the light
	public void attachShrine(Shrine s){
		ParticleSystem ps = (ParticleSystem)GetComponent("ParticleSystem");
		for(int i=0;i<_shrines.Length;i++){
			if(_shrines[i]==null){
				_shrines[i]=s;
				ps.enableEmission=true;
				ps.loop=true;
				del=false;
				ps.Play();
				break;
			}
		}
	}
	//remove shrine s from attached shrines. The light is removed if no shrines are attached anymore
	public void removeShrine(Shrine s){
		int count = 0;
		for(int i=0; i<_shrines.Length; i ++){
			if(_shrines[i] == s)_shrines[i] = null;
			else if(_shrines[i])count++;
		}
		if(count<=0)del=true;
	}
	
	//set independent, meaning that it is not affected by surrounding shrines or temples
	public void setIndependent(){
		independent = true;
	}
	
	//if one shot is set, it will start, then fade after spawning
	public void OneShot(){
		setIndependent();
		renderer.material.SetColor("_TintColor", new Color(1,1,1,1));
		del=true;
	}
	
}
