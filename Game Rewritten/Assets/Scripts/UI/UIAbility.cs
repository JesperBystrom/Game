using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAbility : MonoBehaviour {

	public Ability ability;
	public Text usesText;
	private Unit unit;

	public void initilize(Unit unit, Ability ability){
		this.ability = ability;
		this.unit = unit;
		transform.GetChild(0).GetComponent<Image>().sprite = ability.icon;
	}

	private void Update(){
		usesText.text = ability.uses.ToString();
	}

	public void selectAbility(){
		if(unit.getPreparedAbility() != ability)
			unit.swapAbilities(ability);
		else
			unit.unselect(false);
	}
}
