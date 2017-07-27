using UnityEngine;
using System.Collections;

public class CanvasCards : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//default values
		Canvas thisCanvas = GetComponentInParent<Canvas>();
		thisCanvas.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
