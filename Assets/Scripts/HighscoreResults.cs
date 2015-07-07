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
			//IEnumerable<ParseObject> highscores = t.Result;
			highscores = t.Result;
			gotThem = true;
			/*
			highscoreTableText.text = "";
			foreach (var highscore in highscores) {
				highscoreTableText.text = highscoreTableText.text + highscore["name"] + "\t|\t" + highscore["mass"] + "\n";
			}
			*/
		});
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gotThem) {
			highscoreTableText.text = "";
			foreach (var highscore in highscores) {
				highscoreTableText.text = highscoreTableText.text + highscore ["name"] + "\t|\t" + highscore ["mass"] + "\n";
			}
			gotThem = false;
		}
	}
}