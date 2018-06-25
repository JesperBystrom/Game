using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour {

	[System.Serializable]
	public struct Angles {
		public Vector3 position;
		public Vector3 rotation;

		public static implicit operator Angles(Vector3 pos){
			return new Angles() { position = pos, rotation = pos };
		}
	}
	public Angles[] angles;
	private int currentAngle;
	private Vector3 originalPosition;
	private Vector3 target;
	private Angles targetAngle;
	private float speed = 8;

	void Start(){
		originalPosition = transform.position;
		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 315.226f, 0);
		targetAngle = Vector3.zero;
	}

	void Update(){

		if(Input.GetKeyDown(KeyCode.Q)){
			currentAngle--;
		}
		if(Input.GetKeyDown(KeyCode.E)){
			currentAngle++;
		}
		if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)){
			currentAngle = (currentAngle > angles.Length-1) ? 0 : currentAngle;
			currentAngle = (currentAngle < 0) ? angles.Length-1 : currentAngle;
			targetAngle = angles[currentAngle];
		}

		if(Input.GetMouseButton(Mouse.RIGHT_CLICK)){
			target = Vector3.zero;
			float x = Input.GetAxis("Mouse X") * speed;
			float y = Input.GetAxis("Mouse Y") * speed;


			transform.position += transform.right * -x * Time.deltaTime;
			transform.position += transform.up * -y * Time.deltaTime;
		}

		if(targetAngle.position != Vector3.zero){

			Quaternion q = Quaternion.Euler(targetAngle.rotation);
			transform.position = Vector3.Lerp(transform.position, targetAngle.position, 8 * Time.deltaTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, q, 0.4f);

			bool b = Vector3.Distance(transform.position, targetAngle.position) < 0.1f && Quaternion.Angle(transform.rotation, q) < 1;

			if(b){
				targetAngle = Vector3.zero;
			}
	
		}

		/*if(target == Vector3.zero) return;
		transform.position = Vector3.Lerp(transform.position, new Vector3(target.x, transform.position.y, target.z), 4 * Time.deltaTime);
		if(Vector3.Distance(transform.position, target) < 0.1f){
			target = Vector3.zero;
		}*/
	}

	public void panTowards(GameObject o){
		target = o.transform.position;
	}

	public void reset(){
		//target = new Vector3(originalPosition.x - transform.position.y/2, originalPosition.y, originalPosition.z + transform.position.y/2);
	}
}
