using UnityEngine;
using System.Collections;

public class CanvasPlayers : MonoBehaviour {
	public GameObject playerGround;
	public GameObject bballerFloor;
	GameObject leftPlayer;
	GameObject frontPlayer;
	GameObject rightPlayer;
	// Use this for initialization
	void Start () {
		//default values
		//default values
		Canvas thisCanvas = GetComponentInParent<Canvas>();
		thisCanvas.enabled = false;
		leftPlayer = bballerFloor.transform.GetChild (0).gameObject;
		frontPlayer = bballerFloor.transform.GetChild (1).gameObject;
		rightPlayer = bballerFloor.transform.GetChild (2).gameObject;
	}

	// show three player models
	public void displayThreeModel() {
		for (int i = 0; i < bballerFloor.transform.childCount; i++) {
			bballerFloor.transform.GetChild (i).gameObject.SetActive (true);
		}

		Vector3 newPos = new Vector3 (-400f, -210f, 0f);
		playerGround.GetComponent<RectTransform> ().localPosition = newPos;
		bballerFloor.GetComponent<MeshRenderer> ().enabled = false;
		bballerFloor.GetComponent<RectTransform> ().Rotate (new Vector3(10f, 0f, 0f));

		Vector3 frontPos = new Vector3 (0f, 300f, -0.3f);
		Vector3 leftPos = new Vector3 (-0.3f, 300f, 0.15f);
		Vector3 rightPos = new Vector3 (0.3f, 300f, 0.15f);
		Vector3 rotation = new Vector3 (0f, 10f, 0f);
		leftPlayer.GetComponent<Transform> ().localPosition = leftPos;
		leftPlayer.GetComponent<Transform> ().Rotate (rotation);

		frontPlayer.GetComponent<Transform> ().localPosition = frontPos;
		frontPlayer.GetComponent<Transform> ().Rotate (rotation);

		rightPlayer.GetComponent<Transform> ().localPosition = rightPos;
		rightPlayer.GetComponent<Transform> ().Rotate (rotation);

		for (int i = 0; i < bballerFloor.transform.childCount; i++) {
			for (int j = 0; j < bballerFloor.transform.GetChild (i).childCount; j++) {
				bballerFloor.transform.GetChild (i).GetChild (j).localScale = new Vector3 (5f, 0.9f, 10f);
			}
		}
	}
}
