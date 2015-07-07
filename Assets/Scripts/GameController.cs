using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum States{
	Playing,Final
}

public enum TargetPositions{
	Munich,NewYork,Toyko,Marrakesch,Iceland,Kaptown
}

public class GameController : MonoBehaviour {

	public GameObject rocket;
	public Vector3 spawnValues;
	public float spawnWait;
	public float startWait;
	public int rocketCount;
	public float initialMass;
	private float currentMass;
	public GameObject textgameobject;
	private string mainMenu;
	private States state;
	private float timePlayed;
	public float playTime;
	public float cameraTurnSpeed;
	public float cameraMoveSpeed;
	public ParticleSystem stars;
	public GameObject asteroid;
	public GameObject asteroidParent;
	public GameObject earthParent;
	public float asteroidMoveSpeed;
	public float earthRotationSpeed;
	private Vector3 finalTarget;
	private float longi;
	private float lat;
	private Vector3 earthRadius;
	public TargetPositions targetPositions;
	private bool resetAsteroidChild;

	IEnumerator GameLoop ()
	{
		yield return new WaitForSeconds (startWait);
		while (state == States.Playing)
		{
			for (int i = 0; i < rocketCount; i++)
			{
				Vector3 spawnPosition = new Vector3 (Random.Range(spawnValues.x,-spawnValues.x),Random.Range(spawnValues.y,-spawnValues.y),spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (rocket, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait);
			}
		}
	}
		

	// Use this for initialization
	void Start () {
		resetAsteroidChild = true;
		setDummyCoords ();
		earthRadius = new Vector3 (0, 0, 20);
		timePlayed = 0;
		state = States.Playing;
		mainMenu = "Asteroids Menu";
		currentMass = initialMass;
		Text text = textgameobject.GetComponent<Text>();
		text.text = "Mass: "+currentMass;
		transformPos(longi, lat);
		earthParent.transform.Rotate (new Vector3 (-13,4,0));
		earthParent.transform.position = new Vector3 (450,-135,-600);
		StartCoroutine (getLocation ());
		StartCoroutine (GameLoop());
	}
	
	// Update is called once per frame
	void Update () {
		timePlayed += Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.Escape)) {
			loadMainMenu();
		}
		if (timePlayed >= playTime) {
			state = States.Final;
		}
		if (state == States.Final) {
			StopCoroutine(GameLoop());
			stars.Stop();
			Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x,2.3f,-126f), cameraMoveSpeed);
			//Camera.main.transform.eulerAngles = Vector3.Lerp (Camera.main.transform.eulerAngles, new Vector3 (0, 90, 0), Time.deltaTime * cameraTurnSpeed);
			//asteroid.transform.localScale = new Vector3(2,2,2);
			earthParent.transform.position = new Vector3 (0,-135,-600);
			asteroidParent.transform.position = Vector3.MoveTowards(asteroidParent.transform.position, earthParent.transform.position, asteroidMoveSpeed);
			if(resetAsteroidChild){
				asteroidParent.transform.position = Vector3.zero;
				asteroid.transform.position = Vector3.zero;
				asteroid.transform.rotation = new Quaternion(0,0,0,0);
				resetAsteroidChild = false;
			}
		}
		if (currentMass < 500) {
			loadMainMenu();
		}
		if(timePlayed <= playTime)
		earthParent.transform.Rotate(new Vector3(0, Time.deltaTime * -earthRotationSpeed, 0));
	}
	public void updateMass(){
		currentMass -= 500;
		Text text = textgameobject.GetComponent<Text>();
		text.text = "Mass: "+currentMass;
	}
	public States getState(){
		return this.state;
	}
	public float getCurrentMass(){
		return currentMass;
	}
	public void loadMainMenu(){
		Debug.Log ("gamecontroller loadMainMenu");
		Application.LoadLevel(mainMenu);
	}

	private void setDummyCoords(){
		switch(targetPositions){
		case TargetPositions.Iceland:
			lat = 64.963051f;
			longi = -19.020835f;
			break;
		case TargetPositions.Munich:
			longi = 11.581981f;
			lat = 48.135125f;
			break;
		case TargetPositions.NewYork:
			lat = 40.712784f;
			longi = -74.005941f;
			break;
		case TargetPositions.Marrakesch:
			lat = 31.629779f;
			longi = -8.00829f;
			break;
		case TargetPositions.Toyko:
			lat = 35.689487f;
			longi = 139.691706f;
			break;
		case TargetPositions.Kaptown:
			lat = -33.924869f;
			longi = 18.424055f;
			break;
		}
	}

	private void transformPos(float longitude, float latitude){
		Vector3 vec =   Quaternion.AngleAxis(longitude, -Vector3.up) * Quaternion.AngleAxis(latitude, -Vector3.right) * earthRadius;
		/*
		latitude = (latitude * Mathf.PI) / 180;
		longitude = (longitude * Mathf.PI) / 180;
		latitude -= 1.570795765134f;
		float xPos =  Mathf.Sin (latitude) * Mathf.Cos (longitude);
		float zPos =  Mathf.Sin (latitude) * Mathf.Sin (longitude);
		float yPos =  Mathf.Cos (latitude);
		finalTarget = new Vector3 (xPos,zPos,yPos);
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.position = finalTarget;*/
		finalTarget = vec;
		rotatePointOnSurfaceToCamera (earthRadius, finalTarget);
	}

	private void rotatePointOnSurfaceToCamera(Vector3 sphereRadius, Vector3 targetPoint){
		Quaternion rot = Quaternion.FromToRotation (targetPoint, sphereRadius);
		earthParent.transform.rotation *= rot;
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
			Text text = textgameobject.GetComponent<Text>();
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

	IEnumerator EndGame()
	{
		while (true) {
			Debug.Log("endgame true");
			yield return new WaitForSeconds(10.0f);
			loadMainMenu();
		}
	}
	public void startEndGame() {
		StartCoroutine (EndGame ());
	}
}

