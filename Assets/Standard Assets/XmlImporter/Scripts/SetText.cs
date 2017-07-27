using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetText : MonoBehaviour {

    Button buttonParent;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        buttonParent = gameObject.GetComponentInParent<Button>();
        gameObject.GetComponent<Text>().text = buttonParent.name;
	}
}
