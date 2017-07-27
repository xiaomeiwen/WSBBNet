using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChooseBling : MonoBehaviour {
	public Mesh[] blingMeshes;
	public Material blingMat;
	private string selectedBtnName;
//	Transform front;
//	Transform left;
//	Transform right;
//	Transform activeFront;
//	Transform activeLeft;
//	Transform activeRight;
	public GameObject[] headObj;
	public GameObject[] hairObj;
	int size;

	void Start() {
//		front = GameObject.Find ("Front").transform;
//		left = GameObject.Find ("Left").transform;
//		right = GameObject.Find ("Right").transform;
		size = headObj.Length;
	}

	// child0 is head, child1 is body, child2 is hair, child3 is bling head
	public void changeBling() {
		selectedBtnName = EventSystem.current.currentSelectedGameObject.name;
		Debug.Log (selectedBtnName);

//		for (int i = 0; i < front.childCount; i++) {
//			if (front.GetChild (i).gameObject.activeSelf) {  // if i = 0, female is active, i = 1, male is active
//				activeFront = front.GetChild (i);
//			}
//		}
//		for (int i = 0; i < left.childCount; i++) {
//			if (left.GetChild (i).gameObject.activeSelf) {
//				activeLeft = left.GetChild (i);
//			}
//		}
//		for (int i = 0; i < right.childCount; i++) {
//			if (right.GetChild (i).gameObject.activeSelf) {
//				activeRight = right.GetChild (i);
//			}
//		}

		for (int i = 0; i < size; i++) {
			for (int j = 0; j < gameObject.transform.childCount; j++) {
				if (selectedBtnName == "Bling" + (j + 1)) {
					// disable hair and change head
					Debug.Log (j);
					hairObj [i].SetActive (false);
					headObj [i].GetComponent<MeshFilter> ().mesh = blingMeshes [j];
					headObj [i].GetComponent<MeshRenderer> ().material = blingMat;

//					activeFront.GetChild (0).GetComponent<MeshFilter> ().mesh = blingMeshes [j];
//					activeFront.GetChild (0).GetComponent<MeshRenderer> ().material = blingMat;
//
//					activeLeft.GetChild (0).GetComponent<MeshFilter> ().mesh = blingMeshes [j];
//					activeLeft.GetChild (0).GetComponent<MeshRenderer> ().material = blingMat;
//
//					activeRight.GetChild (0).GetComponent<MeshFilter> ().mesh = blingMeshes [j];
//					activeRight.GetChild (0).GetComponent<MeshRenderer> ().material = blingMat;
				}
			}
		}
	}

}
