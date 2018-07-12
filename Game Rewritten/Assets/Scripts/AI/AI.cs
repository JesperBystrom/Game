using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Strategy {
	NEUTRAL, HIT_AND_RUN, DEFEND, OFFENSIVE
}


public class AIPriority {

	private AI ai;


	public AIPriority(AI ai){
		this.ai = ai;
	}

	public Territory getMostExpandableTerritory(Territory territory, Ability ability){

		Territory mostLikely = null;
		float highestValue = 0;

		List<Territory> avaibleTerritores = new List<Territory>();

		foreach(Territory t in Map.instance.territories){
			if(t == null || t == territory) continue;
			avaibleTerritores.Add(t);
		}


		foreach(Territory t in avaibleTerritores){

			float originalDist = Vector3.Distance(t.transform.position, territory.transform.position);
			float dist = Mathf.Abs(originalDist - Map.instance.getMaxDistance());
			float value = 0;

			if(ability.getRange() - originalDist <= 0.5f){
				value += 4;
			}

			value += t.getDensity() * 0.01f;
			value += dist;

			if(t.getPlayer() == null){
				value += 3;
				/*List<Territory> neighbours = t.getNeighbours(4);
				foreach(Territory n in neighbours){
					if(n == null) continue;

					if(n.getPlayer() == null) {
						value += 1;
					}
				}*/
			}

			if(t.getPlayer() == ai){
				value++;
			}

			if(t.getPlayer() != null){
				value += 0.5f;
			}

			foreach(Player p in Game.instance.getPlayers()){
				if(p == ai) continue;

				float distToBase = Territory.distance(t, p.getBase().getTerritory());
				value += ((distToBase/50) * ai.getStrength()) - p.getStrength(); //desto närme basen, desto större chans att man attackerar.
			}

			if(value > highestValue){
				highestValue = value;
				mostLikely = t;
			}
		}

		return mostLikely;
	}
}

public class AIAction {

	private AI ai;
	private bool waited = false;

	public AIAction(AI ai){
		this.ai = ai;
	}
	/*
	public void castAbility(Unit unit, Ability ability){
		ai.StartCoroutine(castAbilityCoroutine(1, unit, ability, ai.getMostValueableTerritory(unit, Map.instance.territories, ai.agressiveDensity)));
	}*/

	public void castAbility(Unit unit, Ability ability, Territory towards){
		ai.unitInUse = unit;
		ai.StartCoroutine(castAbilityCoroutine(1, unit, ability, towards));
	}

	private IEnumerator castAbilityCoroutine(float time, Unit unit, Ability ability, Territory towards){
		yield return new WaitForSeconds(time);
		Territory[] pattern = ability.markTerritories(unit.getTerritory(), towards.gameObject, Map.instance);
		AbilityHandler handler = unit.swapAbilities(ability);
		handler.execute(pattern);
	}

	public Territory canAttack(Unit unit, Ability ability){
		foreach(Player p in Game.instance.getPlayers()){
			if(p == ai) continue;
			foreach(Territory t in p.capturedTerritories){
				if(t.hasEntity()){
					Unit u = null;

					if(t.getEntity().GetType() == typeof(Unit))
						u = (Unit)t.getEntity();

					if(u == null || u == unit) continue;

					if(Vector3.Distance(unit.getTerritory().transform.position, t.transform.position) <= ability.patternRange+0.5f){
						return t;
					}
				}
			}
		}
		return null;
	}
}

public class AI : Player {
	[Header("Prefabs")]
	public ShopManager unitShop;
	public ShopManager structureShop;

	[Header("Stats")]
	public float agressiveDensity = 1;
	public float defensiveDensity = 0;

	public delegate void performAction();
	public performAction actionDelegate;

	public Unit unitInUse;

	private Strategy strategy = Strategy.NEUTRAL;
	private AIAction action;
	private AIPriority priority;

	private void Start(){
		unitShop = GameObject.Find("UnitShop").GetComponent<ShopManager>();
		action = new AIAction(this);
		priority = new AIPriority(this);
	}

	public override void startTurn(){
		StartCoroutine(turnOrder());
	}

	private IEnumerator turnOrder(){
		List<Entity> avaibleEntities = startOfTurn();
		yield return new WaitForSeconds(1);
		while(!duringTurn(avaibleEntities)) {
			yield return new WaitForSeconds(1.5f);
		}
		yield return new WaitForSeconds(1);
		endOfTurn();
	}

	private List<Entity> startOfTurn(){
		List<Entity> avaibleEntities = new List<Entity>();

		strategy = Strategy.NEUTRAL;

		Debug.Log("Stronger: " + MathUtility.percentage(getAmountOfPlayersStrongerThanMe(), Game.instance.players.Length));

		if(MathUtility.percentage(getAmountOfPlayersStrongerThanMe(), Game.instance.players.Length) > 0.5f){
			strategy = Strategy.OFFENSIVE;
		}

		foreach(Territory t in capturedTerritories){
			if(t.getEntity() == null) continue;
			avaibleEntities.Add(t.getEntity());
		}

		foreach(Entity e in avaibleEntities){
			if(Territory.distance(getNearestEnemyTerritory(e.getTerritory(), true), e.getTerritory()) <= 5){
				strategy = Strategy.HIT_AND_RUN;
			}
		}

		Item optimalPurchase = AIBuyStrategy.getMostOptimalPurchase(this, strategy, unitShop);
		Entity purchased = unitShop.purchaseItem(unitShop.getItem(optimalPurchase));

		if(purchased != null) {
			getSafestTerritory(capturedTerritories.ToArray(), true).put(purchased, this);
			avaibleEntities.Add(purchased);
		}

		return avaibleEntities;
	}

	private bool duringTurn(List<Entity> avaibleEntities){

		//if(unitInUse != null) return false; 

		Ability abilityToUse = null;

		Debug.Log(strategy);

		switch(strategy){

		case Strategy.HIT_AND_RUN:

			foreach(Entity e in avaibleEntities){

				if(e == null || e.GetType() != typeof(Unit)) continue;

				Unit u = (Unit)e;

				foreach(Ability a in u.abilities){
					if(a.uses <= 0) continue;

					Territory target = getNearestEnemyTerritory(u.getTerritory(), false);

					if(target != null){

						float dist = Territory.distance(u.getTerritory(), target);

						if(dist <= a.getRange()){
							//Attack if in range
							if(a.abilityType == AbilityType.ATTACK || a.abilityType == AbilityType.PARTICLE){

								Territory t = action.canAttack(u, a);
								if(t != null){
									action.castAbility(u,a,t);
									strategy = Strategy.NEUTRAL;
									return false;
								}
							}
						}

						// Move towards enemy if too far away
						if(a.abilityType == AbilityType.MOVE){
							Territory t = getNearestEnemyTerritory(u.getTerritory(), true);
							if(t != null){
								action.castAbility(u,a,t.getNearestNeighbourFrom(u.getTerritory(), false));
								return false;
							}
						}
					}
				}
			}

			break;
			
		case Strategy.NEUTRAL: //Try to get as many territories as possible
		case Strategy.OFFENSIVE:
			
			foreach(Entity e in avaibleEntities){

				if(e == null || e.GetType() != typeof(Unit)) continue;

				Unit u = (Unit)e;

				abilityToUse = u.getAbility(AbilityType.MOVE);

				if(abilityToUse == null) continue;

				Territory t = priority.getMostExpandableTerritory(u.getTerritory(), abilityToUse);

				if(t != null){
					action.castAbility(u, abilityToUse, t);

					if(abilityToUse.uses > 0)
						return false;
				}
			}
			break;
		}
		return true;
	}

	private void endOfTurn(){
		StartCoroutine(wait());
	}

	public IEnumerator wait(){
		yield return new WaitForSeconds(1);
		Game.instance.endTurn();
		Debug.Log("Ending turn...");
	}

	protected override void Update(){
	}






























	public Territory getSafestTerritory(Territory[] pool, bool friendlyTerritory){
		Territory lowest = null;
		float lowestDistance = 0;
		foreach(Territory t in pool){
			if(t.hasEntity()) continue;

			if(friendlyTerritory){
				if(t.getPlayer() != this) continue;
			} else {
				if(t.getPlayer() == this) continue;
			}

			foreach(Player p in Game.instance.getPlayers()){
				if(p == this) continue;
				Territory recent = p.getRecentCaptured();
				float distance = Vector3.Distance(t.transform.position, recent.transform.position);
				if(distance >= lowestDistance){
					lowest = t;
					lowestDistance = distance;
				}
			}
		}
		return lowest;
	}

	public Territory getNearestEnemyTerritory(Territory territory, bool entityRequirement){

		List<Territory> nearest = new List<Territory>();

		foreach(Player p in Game.instance.getPlayers()){
			if(p == this) continue;
			Territory t = Territory.getNearestTerritory(p.capturedTerritories.ToArray(), territory, entityRequirement);

			Debug.Log(t.getPlayer());

			if(t != null)
				nearest.Add(t);
		}

		return Territory.getNearestTerritory(nearest.ToArray(), territory, entityRequirement);
	}

	/*

	public Territory getMostValueableTerritory(Unit unit, Territory[] pool, float agressiveDensity){
		float highestValue = 0;
		//Territory mostLikely = null;
		List<Territory> mostLikelyPool = new List<Territory>();
		Territory mostLikely = null;
		foreach(Territory t in pool){
			float value = 0;
			float dist = Vector3.Distance(t.transform.position, unit.getTerritory().transform.position);

			if(dist > 4 || t.hasEntity()) continue;

			value = dist * agressiveDensity;

			if(t.getPlayer() == this) value += 0.1f * agressiveDensity;
			if(t.getPlayer() == null) value += 1f / agressiveDensity;


			foreach(Player p in Game.instance.getPlayers()){
				if(p == this) continue;
				value += agressiveDensity;
			}

			if(value > highestValue){
				highestValue = value;
				mostLikelyPool.Add(t);
				mostLikely = t;
			}
		}
		return mostLikely;
	}

	public Territory getNearestEnemyTerritory(Territory territory, bool entityRequirement){

		List<Territory> nearest = new List<Territory>();

		foreach(Player p in Game.instance.getPlayers()){
			if(p == this) continue;
			Territory t = Territory.getNearestTerritory(p.capturedTerritories.ToArray(), territory, entityRequirement);

			Debug.Log(t.getPlayer());

			if(t != null)
				nearest.Add(t);
		}

		return Territory.getNearestTerritory(nearest.ToArray(), territory, entityRequirement);
	}

	public void castAbility(Unit unit, AbilityType type){
		foreach(Ability a in unit.abilities){
			if(a.abilityType == type){
				Territory[] pattern = a.markTerritories(unit.getTerritory(), getSafestTerritory(Map.instance.territories, false).gameObject, Map.instance);
				AbilityHandler handler = unit.prepareAbility(a);
				handler.execute(pattern);
			}
		}
	}*/
}
