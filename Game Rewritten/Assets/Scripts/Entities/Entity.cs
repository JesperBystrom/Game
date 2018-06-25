using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MoveType {
	FLOAT, JUMP
}

public class Entity : MonoBehaviour {

	[Header("Entity Prefabs")]
	public GameObject statsUIPrefab;
	public GameObject damageTextPrefab;
	public GameObject healthBarPrefab;
	public Renderer[] renderers;
	[Space(10)]
	[Header("Stats")]
	public int health;
	public int maxHealth;
	[Range(0,1)] public float moveSpeed = 0.5f;
	public MoveType moveType;

	public delegate IEnumerator moveDelegate(Territory territory, float movespeed);
	public delegate void buffDelegate(int amount);

	protected UI statsUIInstance;
	protected Territory territory;
	protected bool fadeOnHover;

	private HealthController healthBarInstance;
	public Color[] originalColors;
	private Animator animator;
	private bool selected;
	private Color currentColor;

	public virtual void Start(){

		maxHealth = health;

		if(statsUIInstance != null){
			statsUIInstance.forceFade(1f);
		}

		animator = GetComponent<Animator>();


		originalColors = new Color[renderers.Length];
		for(int i=0;i<renderers.Length;i++){
			if(renderers[i].material == null) continue;
			originalColors[i] = renderers[i].material.color;
		}
	}

	public virtual void Update(){
		if(!selected){
			GameObject o = Mouse.getGameObject("Territory");
			if(o != null){
				Territory t = o.GetComponent<Territory>();
				if(t != null){
					if(t.getEntity() == this){
						if(healthBarInstance == null){
							createHealthBar();
						}
					} else {
						removeHealthBar();
					}
				}
			}
		}

		if(!selected || statsUIInstance == null) return;

		if(Input.GetMouseButtonDown(Mouse.LEFT_CLICK) && !Mouse.isOverUI() && Mouse.getGameObject("Territory") != territory.gameObject){
			unselect(false);
		}
	}

	public virtual void select(){
		createHealthBar();

		if(statsUIPrefab == null) return;

		GameObject o = Instantiate(statsUIPrefab.gameObject);
		o.transform.SetParent(GameObject.Find(statsUIPrefab.transform.parent.name).transform);
		o.GetComponent<UIToGame>().anchor(this.gameObject, 24);

		selected = true;
		statsUIInstance = o.GetComponent<UI>();
		o.SetActive(true);
		Camera.main.GetComponent<CameraPan>().panTowards(gameObject);
	}

	public virtual void unselect(bool resetCamera){
		if(!selected) return;
		Destroy(statsUIInstance.gameObject);
		selected = false;

		if(resetCamera)
			Camera.main.GetComponent<CameraPan>().reset();

		GameObject o = Mouse.getGameObject("Territory");

		if(o != null){
			if(o != territory.gameObject)
				removeHealthBar();
		}
	}

	public virtual void setTerritory(Territory territory){
		this.territory = territory;
		transform.position = new Vector3(territory.transform.position.x, transform.position.y, territory.transform.position.z);
	}

	public void hurt(int damage){
		StartCoroutine(hurtCoroutine(damage));
	}

	private IEnumerator hurtCoroutine(int damage){
		if(renderers == null) yield return null;
		changeColor(Color.red);
		yield return new WaitForSeconds(0.5f);
		resetColors();

		changeHealth(takeDamage, damage, TextType.LOSS);
		yield return null;
	}

	public void changeHealth(buffDelegate method, int amount, TextType type){
		method.Invoke(amount);

		Vector3 v = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		TextFactory.getInstance().createMovingText(damageTextPrefab, v, amount, type, true);
		updateHealthBar();
	}

	public void takeDamage(int amount){
		health -= amount;
		if(health <= 0)
			die();
	}

	public void heal(int amount){
		health += amount;
	}

	public void die(){
		Destroy(gameObject);
	}

	public void changeColor(Color c){
		this.currentColor = c;
		foreach(Renderer r in renderers){
			if(r == null) continue;
			r.material.color = c;
		}
	}

	public void resetColors(){
		if(renderers.Length <= 0) return;
		currentColor = originalColors[0];
		for(int i=0;i<renderers.Length;i++){
			renderers[i].material.color = originalColors[i];
		}
	}

	private void createHealthBar(){
		removeHealthBar();
		healthBarInstance = UI.createInstance(healthBarPrefab).GetComponent<HealthController>();
		healthBarInstance.setHealth(health, maxHealth);
		healthBarInstance.GetComponent<UIToGame>().anchor(gameObject, -32);
	}

	private void removeHealthBar(){
		if(healthBarInstance != null)
			Destroy(healthBarInstance.gameObject);
	}

	private void updateHealthBar(){
		if(healthBarInstance != null)
			healthBarInstance.setHealth(health, maxHealth);
	}

	public Territory getTerritory(){
		return territory;
	}

	public bool isSelected(){
		return selected;
	}

	public void deactivate(){
		changeColor(Color.gray);
	}

	public bool isActivated(){
		return currentColor != Color.gray;
	}
}
