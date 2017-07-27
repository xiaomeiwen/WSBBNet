/*
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ClickCard : MonoBehaviour {

	public GameObject reviewCardWindow;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick()
	{
		//change sprite
		UISprite destinySprite = reviewCardWindow.GetComponent<UISprite>();
		UISprite selfSprite = GetComponent<UISprite>();
		destinySprite.spriteName = selfSprite.spriteName;

		//toggle active
		if (reviewCardWindow.activeSelf == true)
			reviewCardWindow.SetActive (false);
		else 
			reviewCardWindow.SetActive (true);

	}
}
*/
