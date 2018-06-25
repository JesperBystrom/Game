using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour {

	public float health;
	public RectTransform rectTransform;
	public Text healthText;
	private Vector2 size;
	public float maxHealth;

	void Start () {
		size = rectTransform.sizeDelta;
	}

	void Update () {
		rectTransform.sizeDelta = new Vector2(size.x*(health/maxHealth), size.y);
	}

	public void setHealth(float health, float maxHealth){
		this.health = health;
		this.maxHealth = maxHealth;
		healthText.text = health + " / " + maxHealth;
	}
}
