using UnityEngine;
using System.Collections;


//this component hold how many souls are within a unit and allow them to merge and ascend, etc

public class Soul : MonoBehaviour {
	
	public int Souls=1;
	public string Realm="";
	
	public const int LOW_ASCEND=10;
	public const int MID_ASCEND=20;
	
	//max souls indicate teh max number of souls allowed in that unit - negative 1 means no limit
	public int max_souls=-1;
	
	//disable the object
	void Start(){ enabled = false; }
	
	//set the amount of souls on the object
	public void SetSouls(int num){		
		//if the amount of souls is the same as whats already there, do nothing
		if(Souls==num)return;
		//otherwise, set the amount of souls to num and propegate up and down to set the attached souls to the same
		Souls=num;
		
		//if there are no souls left, remove the object
		if(Souls<=0
		   ||(Realm=="HighPlane"&&Souls<MID_ASCEND)
		   ||(Realm=="MidPlane"&&Souls<LOW_ASCEND)){
			

			//the unit is destroyed if and only if there is no host
			//((Unit)GetComponent("Unit"))._unitManager.Deselect();
			if(((Unit)GetComponent("Unit")).Naked){
				if(Souls<=0)Destroy(this.gameObject);
			}else if(tag!="WorshipSite"){
				//((Unit)GetComponent("Unit")).Player=-1;
				//Souls=0;
			}
		}
	}
	//remove num souls from the entity
	public void RemoveSouls(int num){
		SetSouls(Souls-num);
	}
	//add num souls to the entity
	public void AddSouls(int num){
		SetSouls(Souls+num);
	}
	
	//Merge a certain num of souls from this soul to soul s
	public bool MergeTo(Soul s,int num){		
		if(!s)return false;
		
		//Unit u=(Unit)s.GetComponent("Unit");
	
		s.AddSouls(num);
		this.RemoveSouls(num);
		
		return true;

	}
	//Merges ALL souls from this soul to soul s
	public bool MergeTo(Soul s){
		return MergeTo(s, Souls);
	}
	
	//if necessary, display the amount of souls in the unit
	public bool displayCount=true;
	void OnGUI(){
		if(!((Unit)GetComponent("Unit")).Hover || !displayCount)return;
		GUIStyle s= GUI.skin.GetStyle("Label");
		s.alignment=TextAnchor.UpperCenter;
		
		Vector3 p=Camera.main.WorldToScreenPoint(transform.position);
		GUI.Label(new Rect(p.x-25,Screen.height-p.y-30,50,20),((Soul)GetComponent("Soul")).Souls.ToString(),s);
	}
		
	
}
