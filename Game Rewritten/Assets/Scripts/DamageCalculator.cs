using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageCalculator {

	public static float calculatePercentageHealth(float myDamage, float targetHealth, float targetMaxHealth){
		float healthLeft = targetHealth - myDamage;
		return healthLeft / targetMaxHealth;
	}

}
