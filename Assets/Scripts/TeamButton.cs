using UnityEngine;
using System.Collections;

public class TeamButton : MonoBehaviour {

	public GameObject Team;
	public GameObject Card;
	public GameObject Player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick()
	{
		Team.SetActive (true);
		Card.SetActive (false);
		Player.SetActive (false);
	}
}
