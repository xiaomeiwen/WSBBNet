using UnityEngine;
using System.Collections;

public class ButtonBackCourtSelect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPress()
	{
		Application.LoadLevel ("MainSelect");
	}
}
