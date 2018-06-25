using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPatternData {
	public List<Vector2> pattern = new List<Vector2>();

	public AbilityPatternData(List<Vector2> pattern){
		this.pattern = pattern;
	}

	public Territory[] parsePattern(Map map, int requiredMark){
		List<Territory> territories = new List<Territory>();
		foreach(Vector2 v in pattern){
			Territory t = map.findTerritory((int)v.x, (int)v.y);
			if(t != null){
				if(t.compareMark(requiredMark) || requiredMark == MarkType.NONE){
					territories.Add(t);
				}
			}
		}
		return ArrayUtils<Territory>.listToArray(territories);
	}
}

public static class AbilityPattern {

	public static AbilityPatternData getSquarePattern(Vector3 origin, int range){
		List<Vector2> pattern = new List<Vector2>();
		//pattern.Add(new Vector2(center.x, center.z));
		for(int z=(int)origin.z-range;z<=(int)origin.z+range;z++){
			for(int x=(int)origin.x-range;x<=(int)origin.x+range;x++){
				pattern.Add(new Vector2(x,z));
			}
		}
		return new AbilityPatternData(pattern);
	}

	public static AbilityPatternData getFourDirectionalPattern(Vector3 origin, int range){
		List<Vector2> pattern = new List<Vector2>();
		for(int x=(int)origin.x-range;x<=(int)origin.x+range;x++){
			pattern.Add(new Vector2(x, origin.z));
		}

		for(int z=(int)origin.z-range;z<=(int)origin.z+range;z++){
			pattern.Add(new Vector2(origin.x, z));
		}
		return new AbilityPatternData(pattern);
	}

	public static AbilityPatternData getPathPattern(Vector3 origin, Vector3 target, Map map, int range, bool includeBlockage){
		List<Vector2> pattern = new List<Vector2>();
		Node2D[] path = map.pathFinder.findPath((int)origin.x, (int)origin.z, (int)target.x, (int)target.z, range, includeBlockage);
		if(path != null){
			foreach(Node2D n in path){
				pattern.Add(new Vector2(n.x, n.y));
			}
		}
		return new AbilityPatternData(pattern);
	}
}

public enum PatternType {
	SQUARE, FOUR_DIRECTIONAL, PATH
}

public enum AbilityType {
	MOVE, ATTACK, PARTICLE, DAMAGE
}

public enum AbilityList {
	ATTACK,
	DOUBLE_MOVE,
	FIRE,
	FIRE_DIRECTIONAL,
	MOVE
}

public class AbilityHandler {
	private Ability ability;
	private Ability abilityInstance;
	private Territory[] pattern;
	private int currentTerritory;
	private int timer;
	private int startTimer;
	private Unit entity;
	private bool executed;
	private bool finished;

	public AbilityHandler(Unit u, Ability ability){
		this.ability = ability;
		this.timer = ability.timer;
		this.startTimer = this.timer;
		this.entity = u;

		ability.finished = false;
		ability.setCanCast(true);
	}

	public void execute(Territory[] pattern){
		this.pattern = pattern;
		entity.rotateTowards(90, pattern[0].transform.position);

		ParticleSystem ps = ability.GetComponent<ParticleSystem>();
		if(ps != null)
			ps.Play();
		decreaseUses();
	}

	private void execute(Territory territory){

		switch(ability.abilityType){
		case AbilityType.ATTACK:
			handleAttack(territory);
			damage(territory);
			break;
		case AbilityType.MOVE:
			handleMove(territory);
			break;
		case AbilityType.PARTICLE:
			handleSpell(territory);
			damage(territory);
			break;
		}
	}

	public void handleMove(Territory to){
		entity.getTerritory().removeEntity();
		to.put(entity, Game.instance.getCurrentPlayer());
	}

	public void handleSpell(Territory to){
		GameObject o = Object.Instantiate(ability.gameObject);
		o.GetComponent<ParticleDeath>().enabled = true;

		ParticleSystem ps = o.GetComponent<ParticleSystem>();
		if(ps != null)
			ps.Play();
		
		o.transform.position = new Vector3(to.transform.position.x, to.transform.position.y + 1.5f, to.transform.position.z);
	}

	public void handleAttack(Territory to){
		entity.move(entity.meleeAttackTerritory, to, 0.65f);
	}

	public void damage(Territory to){
		if(to.hasEntity() && !to.getPlayer().Equals(Game.instance.getCurrentPlayer())){
			to.getEntity().hurt(ability.damage);
		}
	}

	public void update(){
		if(pattern == null) return;

		timer--;
		if(timer <= 0){
			if(currentTerritory >= pattern.Length){
				end();
				return;
			}
			execute(pattern[currentTerritory]);
			currentTerritory++;
			timer = startTimer;
		}
	}

	public bool isAbilityFinishedExecuting(){
		return finished;
	}

	private void end(){
		ability.finished = true;
		pattern = null;
		currentTerritory = 0;
		finished = true;
		Map.instance.clearMarks();
		ability.setCanCast(ability.uses > 0);
	}

	public void decreaseUses(){
		ability.uses--;
		//ability.setCanCast(ability.uses > 0);
		if(ability.autocast) return;
		entity.onAbilityUse();
	}

	public bool hasExecuted(){
		return pattern != null;
	}
}

public class Ability : MonoBehaviour {
	public PatternType patternType;
	public AbilityType abilityType;
	public AbilityList ability;
	public int range;
	public int patternRange;
	public int timer;
	public int damage;
	[Range(-1,2)] public int markType;
	public Sprite icon;
	public int uses;
	[HideInInspector] public bool finished = false;
	[HideInInspector] public bool autocast;

	public bool canCast = true;

	public int startUses;

	private void Start(){
		startUses = uses;
	}

	public Territory[] markTerritories(Territory territory, GameObject target, Map map){
		map.clearMarks();

		Territory[] square = AbilityPattern.getSquarePattern(territory.transform.position, range).parsePattern(map, -1);
		foreach(Territory t in square){
			t.mark(MarkType.NEUTRAL);
		}

		if(target == null)
			return null;

		Territory[] pattern = null;
		switch(patternType){
		case PatternType.SQUARE:
			pattern = AbilityPattern.getSquarePattern(target.transform.position, patternRange).parsePattern(map, MarkType.NEUTRAL);
			break;
		case PatternType.PATH:
			pattern = AbilityPattern.getPathPattern(territory.transform.position, target.transform.position, map, patternRange, markType==MarkType.FRIENDLY).parsePattern(map, MarkType.NEUTRAL);
			break;
		case PatternType.FOUR_DIRECTIONAL:
			pattern = AbilityPattern.getFourDirectionalPattern(target.transform.position, patternRange).parsePattern(map, MarkType.NEUTRAL);
			break;
		}

		Map.instance.markTerritories(pattern, markType);
		return pattern;
	}

	public void castDelay(float timer){
		canCast = false;
		StartCoroutine(wait(timer));
	}

	public IEnumerator wait(float timer){
		yield return new WaitForSeconds(timer);
		canCast = true;
	}

	public void setCanCast(bool b){
		canCast = b;
	}

	public bool getCanCast(){
		return canCast;
	}

	public float getRange(){
		return range * uses;
	}
}
