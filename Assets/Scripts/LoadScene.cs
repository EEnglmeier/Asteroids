using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour {

	public void loadStandardScene(string sceneName ){
		Application.LoadLevel (sceneName);
	}
	public void quitApp(){
		Application.Quit ();
	}

}
