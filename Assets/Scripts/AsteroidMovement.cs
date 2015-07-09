using UnityEngine;
using System.Collections;

public class AsteroidMovement : MonoBehaviour {

	public float speedX;
	public float speedY;
	private GameController gamecontroller;

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.FindGameObjectWithTag("GameController");
		gamecontroller = (GameController) go.GetComponent(typeof(GameController));
	}
	
	// Update is called once per frame
	void Update () {
		if (gamecontroller.getState () == States.Playing) {
			transform.position = new Vector3 (Mathf.Clamp (transform.position.x, -30, 30), Mathf.Clamp (transform.position.y, -10, 25), Mathf.Clamp (transform.position.z, 0f, 0f));
			transform.Translate (-Input.acceleration.x * speedX * Time.deltaTime, Input.acceleration.y + 0.4f * speedY * Time.deltaTime, 0);
		}
	}
}
