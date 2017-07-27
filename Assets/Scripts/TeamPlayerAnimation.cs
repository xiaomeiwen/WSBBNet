using UnityEngine;
using System.Collections;

public class TeamPlayerAnimation : MonoBehaviour {

	public GameObject playerModel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		playerModel.GetComponent<Animation>().Play("Action1");
	}
}
