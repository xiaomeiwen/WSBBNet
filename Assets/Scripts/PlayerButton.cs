using UnityEngine;
using System.Collections;

public class PlayerButton : MonoBehaviour {

	public GameObject Team;
	public GameObject Card;
	public GameObject Player;
	public string path;
	public AssemblyCSharp.PlayerInformationContainer playersInformation;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnClick()
	{
		Team.SetActive (false);
		Card.SetActive (false);
		Player.SetActive (true);

		playersInformation = AssemblyCSharp.PlayerInformationContainer.Load (path);

		Debug.Log("Current player number in Xml File:" + playersInformation.players.Count);
		Debug.Log("Player1 name:" + playersInformation.players[0].name);
		Debug.Log("Player2 name:" + playersInformation.players[1].name);
		Debug.Log("Player3 name:" + playersInformation.players[2].name);
	}
}
