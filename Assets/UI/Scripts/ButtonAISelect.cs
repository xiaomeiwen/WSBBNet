using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class ButtonAISelect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPress()
	{
		SceneManager.LoadScene ("CourtSelect");
	}
}
