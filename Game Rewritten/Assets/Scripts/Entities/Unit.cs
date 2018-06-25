using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : Entity {
	[Space(10)]
	[Header("Unit Prefabs")]
	public Ability[] abilities;
	public GameObject abilityUIPrefab;
	public bool castSpellAtEndOfTurn;

	private Ability preparedAbility;
	private AbilityHandler activeAbilityHandler;
	private List<Ability> abilityQueue = new List<Ability>();
	private bool finishedMoving = false;

	public void Start(){
		for(int i=0;i<abilities.Length;i++){
			abilities[i] = Instantiate(abilities[i].gameObject).GetComponent<Ability>();
			abilities[i].transform.parent = transform;
			abilities[i].transform.localScale = Vector3.one;
			ParticleSystem ps = abilities[i].GetComponent<ParticleSystem>();
			if(ps != null)
				ps.Stop();
		}

		base.Start();
		fadeOnHover = true;
	}

	public override void Update(){

		if(preparedAbility == null){
			base.Update();
		}

		if(activeAbilityHandler != null){
			activeAbilityHandler.update();

			if(activeAbilityHandler.isAbilityFinishedExecuting()) {
				preparedAbility = null;
				unselect(false);
				activeAbilityHandler = null;

				if(abilityQueue.Count > 0){
					prepareAbility(abilityQueue[abilityQueue.Count-1]);
					abilityQueue.RemoveAt(abilityQueue.Count-1);
				}
			}
		}

		if(preparedAbility == null || (preparedAbility.uses <= 0)) return;

		if(Input.GetMouseButtonDown(Mouse.LEFT_CLICK) && Mouse.mouseInAir()){
			unselect(false);
			return;
		}

		Territory[] pattern = preparedAbility.markTerritories(territory, GridMover.instance.getTerritory().gameObject, Map.instance);

		if(preparedAbility.autocast){
			pattern = preparedAbility.markTerritories(territory, territory.gameObject, Map.instance);
		}
			
		if(pattern != null && preparedAbility.getCanCast()){
			if((Input.GetMouseButtonDown(Mouse.LEFT_CLICK) && pattern.Length > 0 && !Mouse.isOverUI()) || preparedAbility.autocast){
				activeAbilityHandler.execute(pattern);
			}
		}
	}

	public override void select(){
		if(!isActivated() || statsUIPrefab == null) return;
		base.select();
		GameObject ability = abilityUIPrefab;
		RectTransform prefabRect = abilityUIPrefab.GetComponent<RectTransform>();
		for(int i=0;i<abilities.Length;i++){
			ability = Instantiate(abilityUIPrefab);
			ability.transform.SetParent(statsUIInstance.transform);
			ability.transform.GetComponent<UIAbility>().initilize(this, abilities[i]);
			ability.transform.SetParent(statsUIInstance.transform);
			RectTransform rect = ability.GetComponent<RectTransform>();
			rect.anchoredPosition = new Vector2(prefabRect.anchoredPosition.x + (rect.sizeDelta.x + 5) * i, prefabRect.anchoredPosition.y);
		}
		statsUIInstance.GetComponent<RectTransform>().sizeDelta += new Vector2((abilities.Length * prefabRect.sizeDelta.x) + 24, 0);
	}

	public override void unselect(bool resetCamera){
		base.unselect(resetCamera);
		preparedAbility = null;
	}

	public AbilityHandler prepareAbility(Ability ability){
		if(preparedAbility != null){
			abilityQueue.Add(ability);
			return null;
		}
		return swapAbilities(ability);
	}

	public AbilityHandler swapAbilities(Ability ability){
		this.preparedAbility = ability;
		this.activeAbilityHandler = new AbilityHandler(this, ability);
		return activeAbilityHandler;
	}

	public void prepareAbility(Ability ability, bool autocast){
		ability.autocast = autocast;
		ability.castDelay(0.5f);
		prepareAbility(ability);
	}

	public Ability getPreparedAbility(){
		return preparedAbility;
	}

	public void onAbilityUse(){
		foreach(Ability a in abilities){
			if(a.uses > 0){
				return;
			}
		}
		deactivate();
	}

	public void reset(){
		for(int i=0;i<abilities.Length;i++){
			abilities[i].uses = abilities[i].startUses;
		}
		resetColors();
	}

	public override void setTerritory(Territory t){
		bool b = territory == null;
		base.setTerritory(t);
		if(b) return;
		switch(moveType){
		case MoveType.FLOAT:	
			StartCoroutine(moveTowardsTerritory(territory, moveSpeed));
			break;
		case MoveType.JUMP:
			StartCoroutine(moveJumpTransition(territory, moveSpeed));
			break;
		}
	}

	public void move(moveDelegate method, Territory territory, float movespeed){
		StartCoroutine(method.Invoke(territory, movespeed));
	}

	public IEnumerator moveTowardsTerritory(Territory territory, float movespeed){
		finishedMoving = false;
		Vector3 vec = new Vector3(territory.transform.position.x, transform.position.y, territory.transform.position.z);
		while(Vector3.Distance(transform.position, vec) > 0.1f){
			transform.position = Vector3.Lerp(transform.position, vec, moveSpeed);
			yield return null;
		}
		finishedMoving = true;
	}

	public IEnumerator meleeAttackTerritory(Territory territory, float movespeed){
		Vector3 start = transform.position;
		StartCoroutine(moveTowardsTerritory(territory, 0.8f));
		while(!finishedMoving)
			yield return null;

		StartCoroutine(moveTowardsTerritory(Map.instance.findTerritory((int)start.x, (int)start.z), 0.8f));
		while(!finishedMoving)
			yield return null;
		transform.position = start;
	}

	public IEnumerator moveJumpTransition(Territory territory, float movespeed){
		float yOriginal = transform.position.y;
		Vector3 vec = new Vector3(territory.transform.position.x, transform.position.y+1, territory.transform.position.z);
		while(Vector3.Distance(transform.position, vec) > 0.1f){
			transform.position = Vector3.Lerp(transform.position, vec, moveSpeed);
			yield return null;
		}
		transform.position = vec;
		vec = new Vector3(territory.transform.position.x, yOriginal, territory.transform.position.z);
		while(Vector3.Distance(transform.position, vec) > 0.1f){
			transform.position = Vector3.Lerp(transform.position, vec, moveSpeed);
			yield return null;
		}
		transform.position = vec;
	}

	public void rotateTowards(int snapTo, Vector3 worldPosition){
		Vector3 dir = (worldPosition - transform.position).normalized;
		dir.y = 0;
		transform.rotation = Quaternion.LookRotation(dir);
	}

	public Ability getAbility(AbilityList ability){
		foreach(Ability a in abilities){
			if(a.ability == ability)
				return a;
		}
		return null;
	}

	public Ability getAbility(AbilityType type){
		foreach(Ability a in abilities){
			if(a.abilityType == type && a.uses > 0)
				return a;
		}
		return null;
	}
}
