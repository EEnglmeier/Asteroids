using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {
	public float speed;
	private Rigidbody rig;
	public GameObject explosion_1;
	public GameObject explosion_2;
	private float scale = 1f;
	private GameController gamecontroller;
	private bool resetPos;

	// Use this for initialization
	void Start () {
		resetPos = false;
		rig = GetComponent<Rigidbody> ();
		GameObject go = GameObject.FindGameObjectWithTag("GameController");
		gamecontroller = (GameController) go.GetComponent(typeof(GameController));
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (Input.GetKeyDown (KeyCode.A))
			rig.AddForce (new Vector3 (speed, 0, 0));
			transform.position += new Vector3 (speed * Time.deltaTime,  Mathf.Clamp (transform.position.y, 0f, 0f), 0.0f);
		if (Input.GetKeyDown(KeyCode.D))
			rig.AddForce (new Vector3 (-speed, 0, 0));
			
		transform.position -= new Vector3 (speed * Time.deltaTime, Mathf.Clamp (transform.position.y, 0f, 0f), 0.0f);

		transform.Translate (-Input.acceleration.x*speed*Time.deltaTime,0 , 0);*/
		if (gamecontroller.getState () == States.Playing) {
			transform.position = new Vector3 (Mathf.Clamp (transform.position.x, -30, 30), Mathf.Clamp (transform.position.y, -10, 25), Mathf.Clamp (transform.position.z, 0f, 0f));
		}
		if (gamecontroller.getState () == States.Final && !resetPos) {
			resetPos = true;
			transform.position = Vector3.zero;
		}
	}
	void OnCollisionEnter (Collision col){
		if (col.gameObject.tag == "Rocket") {
			GameObject expl_1 = Instantiate(explosion_1,col.transform.position,Quaternion.identity) as GameObject;
			GameObject expl_2 = Instantiate(explosion_2,col.transform.position,Quaternion.identity) as GameObject;
			Destroy(col.gameObject);
			Destroy(expl_1,3);
			Destroy(expl_2,3);
			scale -= 0.05f;
			transform.localScale = new Vector3 (scale,scale,scale);
			GameObject go = GameObject.FindGameObjectWithTag("GameController");
			GameController gamecontroller = (GameController) go.GetComponent(typeof(GameController));
			gamecontroller.updateMass();
		}
	}
}


