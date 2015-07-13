using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Parse;

public class HighscoreResults : MonoBehaviour {

	//public GameObject myGameObject;
	public Text highscoreTableText;
	IEnumerable<ParseObject> highscores;
	bool gotThem;

	// Use this for initialization
	void Start () {
		gotThem = false;
		//Text[] textObjects = myGameObject.GetComponents<Text>();
		//highscoreTableText = textObjects[3];
		var query = ParseObject.GetQuery ("Highscore")
			.OrderByDescending ("mass")
				.ThenBy ("distance")
				.Limit (10);
		query.FindAsync ().ContinueWith (t => {
			highscores = t.Result;
			gotThem = true;

		});
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gotThem) {
			//highscoreTableText.text = "";
			highscoreTableText.text = "Player" + "        \t| \t" + "Mass" + "\t | \t" + "Distance" + "\n" + "----------------------------------------------" + "\n";
			foreach (var highscore in highscores) {
				string playerName = highscore["name"].ToString();
				playerName = playerName.PadRight(15);
				string subDistance = highscore["distance"].ToString();
				subDistance = subDistance.Split('.')[0];
				highscoreTableText.text = highscoreTableText.text + playerName + "  \t" + highscore ["mass"] + "\t \t" + subDistance + "\n";
			}
			gotThem = false;
		}
	}
}