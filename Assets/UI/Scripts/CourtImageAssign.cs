using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class CourtImageAssign : MonoBehaviour {
	// Use this for initialization
	private Button button;
	private Text text;
	public static int courtIdx;

	void Start () {
		button = GetComponent<Button> ();
	}

	// Update is called once per frame
	void Update () {

	}

	public void OnPress(){
		Image selectedCourtImg = GameObject.Find ("selectedCourtImg").GetComponent<Image>();
		selectedCourtImg.sprite = button.image.sprite;

		GameObject courtName = GameObject.Find ("courtName");
		courtName.GetComponent<Text> ().text = this.GetComponent<Image> ().sprite.name;

		courtIdx = int.Parse(this.name.Substring (5)) - 1;
		Debug.Log (courtIdx);
	}
}
