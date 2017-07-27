using UnityEngine;
using System.Collections;

public class CardButton : MonoBehaviour {

	public GameObject Team;
	public GameObject Card;
	public GameObject Player;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnClick() {
		Team.SetActive (false);
		Card.SetActive (true);
		Player.SetActive (false);
	}
}
