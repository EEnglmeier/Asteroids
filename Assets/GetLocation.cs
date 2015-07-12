using UnityEngine;
using System.Collections;

public class GetLocation : MonoBehaviour {

	public static float longi;
	public static float lat;

	// Use this for initialization
	void Start () {
		StartCoroutine (getLocation ());
		longi = 0;
		lat = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator getLocation()
	{
		if (!Input.location.isEnabledByUser) {
			yield break;
		}
		
		Input.location.Start();
		
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
		}
		if (maxWait < 1)
		{
			yield break;
		}
		if (Input.location.status == LocationServiceStatus.Failed)
		{
			yield break;
		}
		else
		{
			longi = Input.location.lastData.longitude;
			lat = Input.location.lastData.latitude;
		}
		
		Input.location.Stop();
	}
}
