using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	public Territory[] territories;
	public static Map instance;
	[HideInInspector] public PathFinder2D pathFinder;

	void Awake(){
		instance = this;
	}

	void Start () {
		pathFinder = new PathFinder2D(15,15);
		territories = new Territory[transform.childCount];
		for(int i=0;i<transform.childCount;i++){
			territories[i] = transform.GetChild(i).GetComponent<Territory>();
		}
	}

	void Update () {
	}

	public void clearMarks(){
		foreach(Territory t in territories){
			t.removeMark();
		}
	}

	public Territory findTerritory(int x, int z){
		foreach(Territory t in territories){
			if(t.transform.position.Equals(new Vector3(x, (int)t.transform.position.y, z))){
				return t;
			}
		}
		return null;
	}

	public Territory chooseRandomTerritory(Vector3 vec, float range){
		return null;
	}

	public Territory chooseRandomTerritory(){
		return territories[Random.Range(0,territories.Length)];
	}

	public void markTerritories(Territory[] territories, int markType){
		foreach(Territory t in territories) {
			t.mark(markType);
		}
	}

	public float getMaxDistance(){
		return Vector3.Distance(territories[0].transform.position, territories[territories.Length-1].transform.position);
	}

	public void updateMapDensity(Player player){
		foreach(Territory t in territories){
			if(t.getPlayer() == player){
				t.setDensity(1);
			}
		}
	}
}