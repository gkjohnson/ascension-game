using UnityEngine;
using System.Collections;

//Shrine that allows units to ascend from tiles around it
public class Shrine : Building {
	Unit u;
	public int SOULS_TO_ASCEND = 5;
	
	public Light glowLight;
	
	//add a light to the object with the team color
	void Start(){
		u= (Unit)GetComponent("Unit");
		return;
		
		GameObject go = new GameObject();
		
		go.AddComponent("Light");
		
		glowLight = (Light) go.GetComponent("Light");

		glowLight.type = LightType.Point;
		
		glowLight.transform.parent = this.transform;
		glowLight.transform.position = this.transform.position + new Vector3(0,6,0);
		glowLight.range=77.15f;
		glowLight.intensity=1f;
				
		glowLight.color = PlayerManager.PM.getColor(u.Player)*1f ;

	}
	
	bool run = false;
	override public void Update(){
		//oscillate the glow intensity
		if(glowLight) glowLight.intensity += (-glowLight.intensity + .45f)/15;

		if(run)return;
		
		//if ascendlights have not been created yet, create them on the surrounding tiles
		Tile t = u.CurrentTile;
		for(int i=0; i < t._adjacentTiles.Length; i ++){
			if(!t._adjacentTiles[i] || !t._adjacentTiles[i].Above)continue;
			
			AscendLight al = (AscendLight)t._adjacentTiles[i].GetComponentInChildren(typeof(AscendLight));
			
			if(al){
				al.attachShrine(this);
				continue;
			}
			
			//create it on the tiles around the shrine
			al = (AscendLight)((GameObject)Instantiate(Resources.Load("Prefabs/AscendLight"))).GetComponent("AscendLight");
			al.setTile(t._adjacentTiles[i]);
			al.attachShrine(this);
			
			//create it on the tiles above, as well, if applicable
			if(t._adjacentTiles[i].Above){
				AscendLight al2 = (AscendLight)((GameObject)Instantiate(Resources.Load("Prefabs/AscendLight"))).GetComponent("AscendLight");
				al2.setTile(t._adjacentTiles[i].Above);
				al2.attachShrine(this);	
				
				al.Above=al2;
			}
			
		}
		run = true;
	}
	
	//once a shrine has been exhausted, destroy it and dissasociate it from all ascend lights
	public override void Exhausted ()
	{
		Tile[] t = u.CurrentTile._adjacentTiles;
		
		for(int i=0;i<t.Length;i++){
			if(!t[i])continue;
			AscendLight al = (AscendLight)t[i].GetComponentInChildren(typeof(AscendLight));
			al.removeShrine(this);
			
			if(t[i]){
				al = (AscendLight)t[i].Above.GetComponentInChildren(typeof(AscendLight));
				if(al)al.removeShrine(this);
			}
		}
		
		u.CurrentTile.Resident=null;
		
		Destroy(this.gameObject);
	}
}
