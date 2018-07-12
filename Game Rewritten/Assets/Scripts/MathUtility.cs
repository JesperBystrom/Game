using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtility {
	public static float remapValue(float value, float minExpected, float maxExpected, float minTarget, float maxTarget){
		return minTarget + (value - minExpected) * (maxTarget - minTarget) / (maxExpected - minExpected);
	}

	public static float percentage(float value, float maxValue){
		return (value / maxValue);
	}

	public static int percentage(int value, int maxValue){
		return (value / maxValue);
	}
}
