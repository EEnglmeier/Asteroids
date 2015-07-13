using UnityEngine;
using System.Collections;

public class EarthCollision : MonoBehaviour {
	public GameObject hugeExpl;
	public GameObject mediumExpl;
	public GameObject smallExpl;
	public float massNeedForHugeExpl;
	public float massNeedForMedExpl;
	public float massNeedForsmallExpl;
	private GameController gamecontroller;

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.FindGameObjectWithTag("GameController");
		gamecontroller = (GameController) go.GetComponent(typeof(GameController));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter (Collision col){
		if (col.gameObject.tag == "Player") {
			if(gamecontroller.getCurrentMass() >= massNeedForHugeExpl){
				hugeExpl = Instantiate(hugeExpl,col.transform.position,Quaternion.identity) as GameObject;
				Destroy(this.gameObject);
			}
			if(gamecontroller.getCurrentMass() < massNeedForHugeExpl && gamecontroller.getCurrentMass() >= massNeedForMedExpl){
				mediumExpl =  Instantiate(mediumExpl,col.transform.position,new Quaternion(45,45,45,45)) as GameObject;

			}
			if(gamecontroller.getCurrentMass() < massNeedForMedExpl && gamecontroller.getCurrentMass() >= massNeedForsmallExpl){
				smallExpl =  Instantiate(smallExpl,col.transform.position,new Quaternion(45,45,45,45)) as GameObject;

			}
			gamecontroller.startEndGame();
			Destroy(col.gameObject);

		}
	}
}
