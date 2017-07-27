using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ImageAssignButtonScript : MonoBehaviour {
	// Use this for initialization
	private Button button;
	private Text text;
	void Start () {
		button = GetComponent<Button> ();
		text = GameObject.Find ("TeamAbout").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void OnPress(){
		GameObject LargeTeamImage = GameObject.Find ("TeamIcon");
		Image imageOfLargeTeamImage = LargeTeamImage.GetComponent<Image>();
		imageOfLargeTeamImage.sprite = button.image.sprite;

		GameObject teamName = GameObject.Find ("TeamName");
		teamName.GetComponent<Text> ().text = button.name;
	}
}
