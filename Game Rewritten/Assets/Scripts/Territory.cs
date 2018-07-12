using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory : MonoBehaviour {
	private Renderer renderer;
	private Entity entity;
	private int markType = MarkType.NONE;
	private Player player;
	private float density;

	void Start(){
		renderer = transform.GetChild(0).GetComponent<Renderer>();
	}

	public void capture(Player player){
		changeColor(player.color);
		player.capturedTerritories.Add(this);

		if(this.player != null)
			this.player.capturedTerritories.Remove(this);
		
		this.player = player;
	}

	public void captureNeighbours(Player player){
		List<Territory> territories = getNeighbours(1);
		territories.ForEach(t => {
			if(t != null)
				t.capture(player);
		});
	}

	public List<Territory> getNeighbours(int range){
		Map m = Map.instance;
		int x = (int)transform.position.x;
		int z = (int)transform.position.z;
		List<Territory> territories = new List<Territory>();
		territories.Add(m.findTerritory(x-range,z));
		territories.Add(m.findTerritory(x+range,z));
		territories.Add(m.findTerritory(x+range,z+range));
		territories.Add(m.findTerritory(x-range,z+range));
		territories.Add(m.findTerritory(x+range,z-range));
		territories.Add(m.findTerritory(x,z-range));
		territories.Add(m.findTerritory(x,z+range));
		territories.Add(m.findTerritory(x-range,z-range));

		/*foreach(Territory t in territories){
			if(t == null) territories.Remove(t);
		}*/

		return territories;
	}

	public static Territory getNearestTerritory(Territory[] pool, Territory territory, bool entityRequirement){
		Territory nearest = null;
		float smallestDistance = 999;

		foreach(Territory t in pool){
			if(entityRequirement){
				if(!t.hasEntity()) continue;
			}
			float d = distance(t, territory);
			if(d <= smallestDistance){
				smallestDistance = d;
				nearest = t;
			}
		}
		return nearest;
	}

	public Territory getNearestNeighbourFrom(Territory territory, bool entityRequirement){
		return getNearestTerritory(getNeighbours(1).ToArray(), territory, entityRequirement);
	}

	public static float distance(Territory t1, Territory t2){
		if(t1 == null || t2 == null) return -1;
		return (int)Vector3.Distance(t1.transform.position, t2.transform.position);
	}

	public void put(Entity e, Player player){
		capture(player);
		e.setTerritory(this);
		this.entity = e;
	}

	public void select(){
		entity.select();
	}

	public void unselect(bool resetCamera){
		entity.unselect(resetCamera);
	}

	/*public void swapSelection(Territory territory){
		if(entity != null)
			entity.swapSelection(territory.getEntity());
	}*/

	public bool selected(){
		if(entity != null)
			return entity.isSelected();
		return false;
	}

	public void changeColor(Color c){
		if(renderer == null) renderer = transform.GetChild(0).GetComponent<Renderer>();
		renderer.material.color = c;
	}

	public Color getCurrentColor(){
		return renderer.material.color;
	}

	public bool hasEntity(){
		return entity != null;
	}

	public Entity getEntity(){
		return entity;
	}

	public void mark(int type){
		Color c = MarkType.parseType(type);
		Color curr = getCurrentColor();
		changeColor(new Color(curr.r*c.r, curr.g*c.g, curr.b*c.b));
		
		markType = type;
	}

	public void removeMark(){
		if(player != null)
			changeColor(player.color);
		else
			changeColor(MarkType.parseType(MarkType.NONE));
	}

	public bool marked(){
		return markType != MarkType.NONE;
	}

	public bool compareMark(int markType){
		return this.markType == markType;
	}

	public Player getPlayer(){
		return player;
	}

	public bool hasPlayer(){
		return player != null;
	}

	public void removeEntity(){
		entity = null;
	}

	public void setDensity(float density, bool applyToNeighbours = true){
		this.density = density;

		if(!applyToNeighbours) return;

		List<Territory> neighbours = getNeighbours(1);
		foreach(Territory t in neighbours){
			if(t == null) continue;
			Debug.Log(Territory.distance(this, t));
			t.setDensity(density / Territory.distance(this, t), false);
		}
	}

	public float getDensity(){
		return density;
	}

	void Update(){
		/*if(density > 0){
			Debug.Log(density);
			//Debug.Log(Color.HSVToRGB(0, 1, 1));
			float colorDensity = MathUtility.remapValue(density, 0, 1, 0, 0.5f);
			Debug.Log("DENSITY: " + colorDensity + ", " + density);
			changeColor(Color.HSVToRGB(colorDensity, 1, 1));
		}*/
	}
}

public enum MarkEnum {
	NONE = -1, NEUTRAL, ATTACK, FRIENDLY
}

public static class MarkType {
	public const int NONE = -1;
	public const int NEUTRAL = 0;
	public const int ATTACK = 1;
	public const int FRIENDLY = 2;

	public static Color parseType(int type){
		switch(type) {
		case NEUTRAL:
			return new Color(0.5f, 0.5f, 0.5f);
		case ATTACK:
			return new Color(2f, 0.5f, 0.5f);
		case FRIENDLY:
			return Color.green;
		}
		return Color.white;
	}
}
