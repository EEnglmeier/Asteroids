using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;

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
	public GameObject gameOverText;
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
	private float distanceTravlled;
	private Vector3 oldPosition;
	public Font myFont;
	public Text distanceText;
	bool waitIsOver;
	IEnumerable<ParseObject> myHighscoreObjects;
	public Canvas inputCanvas;
	public Canvas highscoreCanvas;
	bool hasSaved;

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
		if (GetLocation.longi != 0 && GetLocation.lat != 0) {
			longi = GetLocation.longi;
			lat = GetLocation.lat;
		} 
		else {
			setDummyCoords ();
		}
		oldPosition = Vector3.zero;
		resetAsteroidChild = true;
		earthRadius = new Vector3 (0, 0, 20);
		timePlayed = 0;
		state = States.Playing;
		mainMenu = "Asteroids Menu";
		currentMass = initialMass;
		Text text = textgameobject.GetComponent<Text>();
		text.text = "Mass: "+currentMass;
		this.distanceText.text = "Distance: 0";
		transformPos(longi, lat);
		earthParent.transform.Rotate (new Vector3 (-13,4,0));
		earthParent.transform.position = new Vector3 (450,-135,-600);
		StartCoroutine (GameLoop());
		this.waitIsOver = false;
		this.hasSaved = false;
		inputCanvas.gameObject.SetActive (false);
		highscoreCanvas.gameObject.SetActive (false);
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
			StartCoroutine(EndGameEarly());
		}
		if (state == States.Playing) {
			earthParent.transform.Rotate (new Vector3 (0, Time.deltaTime * -earthRotationSpeed, 0));
			distanceTravlled += Vector3.Distance(asteroidParent.transform.position,oldPosition);
			oldPosition = asteroidParent.transform.position;
			this.distanceText.text = "Distance: " + distanceTravlled.ToString("0");
		}
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


	IEnumerator EndGame()
	{
		// wait for some seconds (explosion woohoo!) before...
		yield return new WaitForSeconds (5.0f);
		// ...presenting the canvas with the input fields
		inputCanvas.gameObject.SetActive (true);
		/*
		while (true) {
<<<<<<< HEAD
			//Debug.Log("endgame true");

			// get the highscores from parse
			var query = ParseObject.GetQuery ("Highscore")
				.OrderByDescending ("mass")
					.ThenBy ("distance")
					.Limit (10);
			query.FindAsync ().ContinueWith (t => {
				myHighscoreObjects = t.Result;
				Debug.Log ("highscores downloaded");
				this.waitIsOver = true;
			});

			// wait for the collision animation to end
=======
>>>>>>> 7e54facb60aa59d28b78f853b1f81bec7ecbdc45
			yield return new WaitForSeconds(10.0f);
			//this.waitIsOver = true;
			//presentHighscoreTable();

			loadMainMenu();
		}
		*/

	}
	IEnumerator EndGameEarly()
	{
		while (true) {
			Text text = gameOverText.GetComponent<Text>();
			text.text = "GAME OVER";
			yield return new WaitForSeconds(5.0f);
			loadMainMenu();
		}
	}
	public void startEndGame() {
		StartCoroutine (EndGame ());
	}
	public float getDistanceTravlled(){
		return distanceTravlled;
	}

	IEnumerator waitAgain()
	{
		yield return new WaitForSeconds (0.5f);
		presentHighscoreTable ();
	}

	public void presentHighscoreTable() {
		inputCanvas.gameObject.SetActive (false);
		highscoreCanvas.gameObject.SetActive (true);
		if (hasSaved) {
			// get the highscores from parse
			var query = ParseObject.GetQuery ("Highscore")
				.OrderByDescending ("mass")
					.ThenBy ("distance")
					.Limit (10);
			query.FindAsync ().ContinueWith (t => {
				myHighscoreObjects = t.Result;
				Debug.Log ("highscores downloaded");
				this.waitIsOver = true;
			});
			
			presentTheHighscores ();
		} else {
			StartCoroutine (waitAgain ());
		}
		
	}

	public void sendInputToParse() {

		Debug.Log ("input to parse");
		GameObject myInputObject = GameObject.Find("NameText");
		GameObject myPlaceholderObject = GameObject.Find ("Placeholder");
		Text myInputText, myPlaceholder;
		if (myInputObject != null) {
			Debug.Log("input text not null");
			myInputText = myInputObject.GetComponent<Text>();
			string myInput = myInputText.text;
			if (myInput != "") {
				//Debug.Log("input string not null: " + myInput);
				ParseObject newHighscore = new ParseObject("Highscore");
				newHighscore["mass"] = this.currentMass;
				newHighscore["distance"] = this.distanceTravlled;
				newHighscore["name"] = myInput;
				
				// set the access control list of the new object to read-only
				newHighscore.ACL = new ParseACL()
				{
					PublicReadAccess = true
				};
				
				/*Task saveTask = */newHighscore.SaveAsync().ContinueWith(t => {
					this.hasSaved = true;
				});
				
				presentHighscoreTable();
			} else {
				myPlaceholder = myPlaceholderObject.GetComponent<Text>();
				myPlaceholder.color = Color.red;
			}
		}
	}

	IEnumerator waitForHighscores()
	{
		yield return new WaitForSeconds (0.5f);
		presentTheHighscores ();
	}

	public void presentTheHighscores() {

		if (this.waitIsOver) {
			// present a "table" with the highscores and the own result (max. 10 highscores)

			GameObject myTextObject = GameObject.Find("HighscoreText");
			Text myText;
			if (myTextObject != null) {
				myText = myTextObject.GetComponent<Text>();
				myText.text = "Player" + "        \t| \t" + "Mass" + "\t | \t" + "Distance" + "\n" + "--------------------------------------------------" + "\n";
				foreach (var myObject in myHighscoreObjects) {
					string playerName = myObject["name"].ToString();
					playerName = playerName.PadRight(15);
					string subDistance = myObject["distance"].ToString();
					subDistance = subDistance.Split('.')[0];
					myText.text = myText.text + /*myObject ["name"]*/playerName + "  \t" + myObject ["mass"] + "\t   \t" + /*myObject["distance"]*/ subDistance + "\n";
				}
			}

			Debug.Log("highscores: " + myHighscoreObjects);
		} else {
			Debug.Log ("wait not yet over");
			StartCoroutine(waitForHighscores());
		}
	}
}

