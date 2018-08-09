using UnityEngine;
using System.Collections;

//A tree that can burn and yields a piece of wood when harvested. Grows with every turn and spawns
//red berries when at max growth
public class TreeFragment : SurfaceFragment {
	public const int MAX_GROWTH = 5;
	public int Growth = MAX_GROWTH;
	
	public bool PrevBurning = false;
	public bool Burning=false;
	
	virtual public void Update(){
		//the amount that it should scale by based on its growth
		const float gFact = .39f;
		Vector3 v = new Vector3(Growth*gFact,Growth*gFact,Growth*gFact);
		//ease to the new size
		v-= transform.localScale;
		v/=5;
		transform.localScale = transform.localScale + v;
		
		//play particles if burning
		if(Burning){
			PrevBurning=true;
			particleSystem.enableEmission=true;
		}else{
			particleSystem.enableEmission=false;
		}
		
		//stop burning if shriveled
		if(Growth==0){
			Burning=false;
			PrevBurning=false;	
		}
		
		
		Vector3 goal = new Vector3(0,0,0);
		if(Growth==MAX_GROWTH){
			goal = new Vector3(.87f,.87f,.87f);
			goal = new Vector3(.9f,.9f,.9f);
		}
		
		//MeshFilter c = (MeshFilter)gameObject.GetComponentInChildren(typeof(MeshFilter)).GetComponentInChildren(typeof(MeshFilter));
		//c.transform.localScale += (goal - c.transform.localScale)/5;
		//c.localScale= goal;
		//print (c.mesh);
		
		//referring to child model berry object
		Transform c = transform.GetChild(0);
		c.localScale += (goal - c.transform.localScale)/5;
		
	}
	
	//on harvest of tree, add wood to current user's resources
	public override bool Harvest ()
	{
		if(Growth == MAX_GROWTH){
			Growth = 0;
			GM.FragMan.Collect(FragmentManager.CreateFragment(FragType.WOOD));
			return true;
		}else{
			return false;
		}
	}
	
	//increase the size of the tree
	public void Grow(){
		if(Random.Range (0.0f, 1.0f)<.333f) Growth= Mathf.Min(Growth+1, MAX_GROWTH);;
	}
	
}
