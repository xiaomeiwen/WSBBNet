

using UnityEngine;
using System.Collections;

public class ButtonBackMainSelect : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPress()
	{
		UnityEngine.SceneManagement.Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		string curSceneName = currentScene.name;

			

		//if currentScene, then load Currents calling scene
		if (curSceneName == "MainSelect") {
			BackFromMainSelect ();
			//UnityEngine.SceneManagement.SceneManager.LoadScene ("SplashScreen");
		}
		else if (curSceneName == "CardShop") {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("MainSelect");
		}
		else if (curSceneName == "CourtSelect") {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("MainSelect");
		} 
		else {
			Debug.Log ("Error: ButtonBack does not have navigation mapped");
		}

	}

	private void BackFromMainSelect ()
	{
		GameObject CanvasPlayOptions = GameObject.Find("CanvasPlayOptions");
		Canvas canvasOfPlayOptions = CanvasPlayOptions.GetComponent<Canvas> ();

		if (canvasOfPlayOptions.enabled == true) {
			canvasOfPlayOptions.enabled = false;
		} else {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("SplashScreen");
		}

	}
}
