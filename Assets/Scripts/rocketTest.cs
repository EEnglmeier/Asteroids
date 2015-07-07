using UnityEngine;
using System.Collections;

public class rocketTest : MonoBehaviour {

	private float rocketSpeed = 2.0f;
	private Vector3 targetPos; 
	public GameObject target;
	public Rigidbody rig;
	private GameController gamecontroller;

	void Start () {
		rig = GetComponent<Rigidbody> ();
		target = GameObject.FindGameObjectWithTag ("Player");
		targetPos = target.transform.position;
		GameObject go = GameObject.FindGameObjectWithTag("GameController");
		gamecontroller = (GameController) go.GetComponent(typeof(GameController));
	}

	void Update () {
		if (Vector3.Distance (targetPos, transform.position) > 100) {
			targetPos = new Vector3(target.transform.position.x,target.transform.position.y,target.transform.position.z+100);
		}
		transform.position = Vector3.MoveTowards (transform.position, targetPos, rocketSpeed);
		if (transform.position.z > 95) {	
			Destroy(this.gameObject);
		}
		if (gamecontroller.getState () == States.Final) {
			Destroy(this.gameObject);
		}
	}


}







