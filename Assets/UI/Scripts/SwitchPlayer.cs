using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwitchPlayer : MonoBehaviour 
{
//	private bool cwRotated = false;  // clockwise rotate
//	private bool ccwRotated = false;  // counter-clockwise rotate
	private bool isRotated = false;
	private float rotateSpeed;
	public static int states;
	private Vector2 startPos;
	public GameObject changeExpObj;
	private ChangeExpValue changeExp;
	private RaycastHit hit;
	private Vector3 target;
//	GameObject frontObj, leftObj, rightObj;
//	Transform frontTrans, leftTrans, rightTrans;
	Transform front, left, right;

	void Start(){
		states = 0;
		rotateSpeed = 4.0f;
		changeExp = changeExpObj.GetComponent<ChangeExpValue> ();
		target = new Vector3(0, 0, 1);
		front = GameObject.Find ("Front").transform;
		left = GameObject.Find ("Left").transform;
		right = GameObject.Find ("Right").transform;
//		// front is female
//		if (front.GetChild (0).gameObject.activeSelf) {
//			frontObj = front.GetChild (0).gameObject;
//			frontTrans = front.GetComponent<Transform> ();
//		} else {  // front is male
//			frontObj = front.GetChild (1).gameObject;
//			frontTrans = frontObj.GetComponent<Transform> ();
//		}
//		// left is female
//		if (left.GetChild (0).gameObject.activeSelf) {
//			leftObj = left.GetChild (0).gameObject;
//			leftTrans = leftObj.GetComponent<Transform> ();
//		} else {  // left is male
//			leftObj = left.GetChild (1).gameObject;
//			leftTrans = leftObj.GetComponent<Transform> ();
//		}
//		// right is female
//		if (right.GetChild (0).gameObject.activeSelf) {
//			rightObj = right.GetChild (0).gameObject;
//			rightTrans = rightObj.GetComponent<Transform> ();
//		} else {  // left is male
//			rightObj = right.GetChild (1).gameObject;
//			rightTrans = rightObj.GetComponent<Transform> ();
//		}
	}

	void Update()
	{
		front.LookAt (target + front.position);
		left.LookAt (target + left.position);
		right.LookAt (target + right.position);

		if (states == 0) {   // front player at front
			if (isRotated) {
				GetComponent<RectTransform> ().Rotate (Vector3.up * rotateSpeed);
				if (GetComponent<RectTransform> ().rotation.eulerAngles.y >= 119) {
					isRotated = false;
					changeExp.player1ToPlayer2 ();
				}
			}
//			Debug.Log ("0---" + GetComponent<RectTransform> ().rotation.eulerAngles.y + " " + isRotated);
		} else if (states == 1) {   // right player at front
			if (isRotated) {
				GetComponent<RectTransform> ().Rotate (Vector3.up * rotateSpeed);
				if (GetComponent<RectTransform> ().rotation.eulerAngles.y >= 239) {
					isRotated = false;
					changeExp.player2ToPlayer3 ();
				}
			}
//			Debug.Log ("1---" + GetComponent<RectTransform> ().rotation.eulerAngles.y + " " + isRotated);
		} else {  // left player at front
			if (isRotated) {
				GetComponent<RectTransform> ().Rotate (Vector3.up * rotateSpeed);
				if (GetComponent<RectTransform> ().rotation.eulerAngles.y <= 1.5f) {
					isRotated = false;
					changeExp.player3ToPlayer1 ();
				}
			}
//			Debug.Log ("2---" + GetComponent<RectTransform> ().rotation.eulerAngles.y + " " + isRotated);
		}

		if (GetComponent<RectTransform> ().rotation.eulerAngles.y >= 0 && GetComponent<RectTransform> ().rotation.eulerAngles.y <= 2f) {
//			Debug.Log("FrontPlayer");
			states = 0;
			// get front player's speed, agility and power - player1
//			changeExp.updateExpVal();
		}
		if (GetComponent<RectTransform> ().rotation.eulerAngles.y >= 119f && GetComponent<RectTransform> ().rotation.eulerAngles.y <= 121f) {
//			Debug.Log("RightPlayer");
			states = 1;
			// get left player's speed, agility and power - player2
//			changeExp.updateExpVal();
		}
		if (GetComponent<RectTransform> ().rotation.eulerAngles.y >= 239f && GetComponent<RectTransform> ().rotation.eulerAngles.y <= 241f) {
//			Debug.Log("LeftPlayer");
			states = -1;
			// get right player's speed, agility and power - player3
//			changeExp.updateExpVal();
		}

		BoxCollider frontCol = front.GetComponent<BoxCollider> ();//GameObject.Find ("BasketballerFront").GetComponent<BoxCollider> ();
		BoxCollider leftCol = left.GetComponent<BoxCollider> ();//GameObject.Find ("BasketballerLeft").GetComponent<BoxCollider> ();
		BoxCollider rightCol = right.GetComponent<BoxCollider> (); //GameObject.Find ("BasketballerRight").GetComponent<BoxCollider> ();

		//#if UNITY_ANDROID touch
		foreach (Touch t in Input.touches) {
			if (t.phase == TouchPhase.Began) {
				if (frontCol.Raycast (Camera.main.ScreenPointToRay (t.position), out hit, Mathf.Infinity) 
					|| leftCol.Raycast (Camera.main.ScreenPointToRay (t.position), out hit, Mathf.Infinity)
					|| rightCol.Raycast (Camera.main.ScreenPointToRay (t.position), out hit, Mathf.Infinity)) {
//					Debug.Log ("touched");
					isRotated = true;
				}
			}
		}
	}
}