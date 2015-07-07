using UnityEngine;
using System.Collections;

public class Earthscaling : MonoBehaviour {
	public float scaleWait;
	public float scalePerStep;
	public float xMovePerStep;

	IEnumerator ScaleEarth ()
	{
		while (transform.localScale.x < 12.0f)
		{
			//Debug.Log(transform.localScale);
			transform.localScale += new Vector3(scalePerStep, scalePerStep, scalePerStep);
			if(transform.position.x > 0){
			transform.position -=  new Vector3(xMovePerStep,0, 0);
			}
				yield return new WaitForSeconds (scaleWait);
		}
	}
	
	
	// Use this for initialization
	void Start () {
		StartCoroutine (ScaleEarth());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
