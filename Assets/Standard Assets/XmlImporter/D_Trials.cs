using UnityEngine;
using System.Collections;
using WSBB;
public class D_Trials : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//XMLSpace.XMLDataParser xdp = new XMLSpace.XMLDataParser ();
		XMLDataParser xdp = new XMLDataParser ();
		xdp.LoadTeamData ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
