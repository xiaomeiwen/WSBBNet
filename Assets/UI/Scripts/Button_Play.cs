using UnityEngine;
using System.Collections;

public class Button_Play : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void goToCardShop()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("CardShop");

		/*
		//get Canvas for CanvasCards
		GameObject CanvasCards = GameObject.Find("Cards");

		Canvas canvasOfCards = CanvasCards.GetComponent<Canvas> ();

		if (CanvasCards.activeSelf) {//if we are on card canvas(it's active go to store
			UnityEngine.SceneManagement.SceneManager.LoadScene("CardShop");
		} else {//else activate CanvasPlayOptions
			UnityEngine.SceneManagement.SceneManager.LoadScene("MainSelect");
			GameObject CanvasPlayOptions = GameObject.Find("CanvasPlayOptions");
			Canvas canvasOfCanvasPlayOptions = CanvasPlayOptions.GetComponent<Canvas> ();

			canvasOfCanvasPlayOptions.enabled = true;
		}
		*/
	}

	public void goToMain()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainSelect");

	}
}
