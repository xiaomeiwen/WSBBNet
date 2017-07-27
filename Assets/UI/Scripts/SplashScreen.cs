using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {
	GameObject playerModel;
	GameObject canvasTeam;
	// Use this for initialization

	void Start () {
//		Canvas thisCanvas = GetComponentInParent<Canvas>();
//		thisCanvas.enabled = true;
		canvasTeam = GameObject.Find("CanvasTeam");
		canvasTeam.SetActive (false);
		playerModel = GameObject.Find ("PlayerModel");
		playerModel.SetActive (false);
	}

	public void play(string sceneName){
		canvasTeam.SetActive (true);
		playerModel.SetActive (true);
		this.gameObject.SetActive (false);
	}
}
