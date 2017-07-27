using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlayersToGameScreen : MonoBehaviour {
	Transform bballer;
	GameObject[] activePlayers = new GameObject[3];
	public GameObject[] players;

	// Use this for initialization
	void Awake () {
		bballer = GameObject.Find ("PlayerGroup").transform.GetChild(0);
//		Debug.Log (bballer.GetChild (1).GetChild (0).gameObject.activeSelf);
//		Debug.Log (bballer.GetChild (2).GetChild (0).gameObject.activeSelf);
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 2; j++) {
//				Debug.Log ("i: " + i + " j:" + j);
				if (bballer.GetChild(i).GetChild (j).gameObject.activeSelf) {
//					Debug.Log ("i: " + i + " j:" + j);
					activePlayers [i] = bballer.GetChild (i).GetChild (j).gameObject;
//					Debug.Log (activePlayers [i].name);
					activePlayers [i].transform.SetParent (players[i].transform);
					activePlayers [i].name = "Basketballer";
					activePlayers [i].GetComponent<Transform> ().localPosition = new Vector3 (0f, 0f, 0f);
					activePlayers [i].GetComponent<Transform> ().Rotate (new Vector3 (0f, 180f, 0f));
					activePlayers [i].GetComponent<Transform> ().localScale = new Vector3 (3.3f, 3.3f, 3.3f);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
