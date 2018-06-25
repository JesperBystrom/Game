using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimatedPanel : UI {

	public Animator animator;

	public override void open(){
		if(opened) return;
		animator.SetTrigger("open");
		opened = true;
	}

	public override void close(){
		Debug.Log("closing....");
		if(!opened) return;
		animator.SetTrigger("close");
		opened = false;
	}

	public override void forceOpen(){
		open();
		opened = true;
	}

	public override void toggle(){
		if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || animator.IsInTransition(0)) return;
		if(!opened)
			open();
		else
			close();
	}
}
