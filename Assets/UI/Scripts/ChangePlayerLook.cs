using UnityEngine;
using System.Collections;

public class ChangePlayerLook : MonoBehaviour {
	public GameObject bballerFloor;

	// size of each array is 3, from left(0) to front(1), right(2)
	public GameObject[] femaleHeadObj;
	public GameObject[] femaleBodyObj;
	public GameObject[] femaleHairObj;
	public GameObject[] maleHeadObj;
	public GameObject[] maleBodyObj;
	public GameObject[] maleHairObj;

	// size of each array is 3, from thin(0) to normal(1), fat(2)
	int size;
	public Mesh[] femaleHead;
	public Mesh[] femaleBody;
	public Mesh[] maleHead;
	public Mesh[] maleBody;

	int hairSize;
	public Mesh[] hair;

	// size of each material array is 3, from left(0) to front(1), right(2)
	public Material[] fFaceMat;
	public Material[] mFaceMat;
	public Material[] mSkinMat;
	public Material[] fSkinMat;
	public Material[] jerseyMat;
	public Material[] shoesMat;
	public Material[] propsMat;

	// Use this for initialization
	void Start () {
		size = maleBody.Length;
		hairSize = hair.Length;
//		bballerFloor = GameObject.Find ("BBallerFloor");

		// set default meshes and the texture offset of materials
		for (int i = 0; i < size; i++) {
			femaleHeadObj[i].GetComponent<MeshFilter> ().mesh = femaleHead [1];
			femaleBodyObj[i].GetComponent<SkinnedMeshRenderer> ().sharedMesh = femaleBody [1];
			femaleHairObj [i].GetComponent<MeshFilter> ().mesh = hair [0];

			maleHeadObj[i].GetComponent<MeshFilter> ().mesh = maleHead [1];
			maleBodyObj[i].GetComponent<SkinnedMeshRenderer> ().sharedMesh = maleBody [1];
			maleHairObj [i].GetComponent<MeshFilter> ().mesh = hair [0];

			fFaceMat [i].mainTextureOffset = new Vector2 (0, 0);
			mFaceMat [i].mainTextureOffset = new Vector2 (0, 0);
			mSkinMat [i].mainTextureOffset = new Vector2 (0, 0);
			fSkinMat [i].mainTextureOffset = new Vector2 (0, 0.333f);
			jerseyMat [i].mainTextureOffset = new Vector2 (0, 0);
			shoesMat [i].mainTextureOffset = new Vector2 (0, 0);
		}
	}

	public void changeGender() {
		// child0(idx = 0) is left, child1(idx = 1) is front, child2(idx = 2) is right
		int idx = 0;
		for (int i = 0; i < size; i++) {
			if (bballerFloor.transform.GetChild (i).gameObject.activeSelf) {
				idx = i;
			}
		}

		// GetChild(j) represents gender, j = 0 means female, j = 1 means male
		for (int j = 0; j < 2; j++) {
			bballerFloor.transform.GetChild (idx).GetChild (j).gameObject.SetActive(!bballerFloor.transform.GetChild (idx).GetChild (j).gameObject.activeSelf);
		}
	}

	public void changeRace() {
		// child0(idx = 0) is left, child1(idx = 1) is front, child2(idx = 2) is right
		int idx = 0;
		for (int i = 0; i < size; i++) {
			if (bballerFloor.transform.GetChild (i).gameObject.activeSelf) {
				idx = i;
			}
		}

		// change skin material offset
		mSkinMat [idx].mainTextureOffset = new Vector2(mSkinMat [idx].mainTextureOffset.x + 0.2f, mSkinMat [idx].mainTextureOffset.y);
		fSkinMat [idx].mainTextureOffset = new Vector2(fSkinMat [idx].mainTextureOffset.x + 0.2f, fSkinMat [idx].mainTextureOffset.y);
		// change female face material offset
		fFaceMat [idx].mainTextureOffset = new Vector2(fFaceMat [idx].mainTextureOffset.x + 0.2f, fFaceMat [idx].mainTextureOffset.y);
		// change male face material offset
		mFaceMat [idx].mainTextureOffset = new Vector2(mFaceMat [idx].mainTextureOffset.x + 0.2f, mFaceMat [idx].mainTextureOffset.y);

		// change skin material texture based on textures
		if (mSkinMat [idx].mainTextureOffset.x == 1.0f) {
			mSkinMat [idx].mainTextureOffset = new Vector2(0.0f, mSkinMat [idx].mainTextureOffset.y + 0.333f);
			fSkinMat [idx].mainTextureOffset = new Vector2(0.0f, fSkinMat [idx].mainTextureOffset.y + 0.333f);
			fFaceMat [idx].mainTextureOffset = new Vector2(0.0f, fFaceMat [idx].mainTextureOffset.y + 0.333f);
			mFaceMat [idx].mainTextureOffset = new Vector2(0.0f, mFaceMat [idx].mainTextureOffset.y + 0.333f);
		}
		if (mSkinMat [idx].mainTextureOffset.y >= 0.99f) { // first y is 0, then 0.333f, then 0.666f, then 0.999f => >= 0.99f
			mSkinMat [idx].mainTextureOffset = new Vector2(0.0f, 0.0f);
			fSkinMat [idx].mainTextureOffset = new Vector2(0.0f, 0.333f);
			fFaceMat [idx].mainTextureOffset = new Vector2(0.0f, 0.0f);
			mFaceMat [idx].mainTextureOffset = new Vector2(0.0f, 0.0f);
		}
	}

	public void changeFace() {
		// child0(idx = 0) is left, child1(idx = 1) is front, child2(idx = 2) is right
		int idx = 0;
		for (int i = 0; i < size; i++) {
			if (bballerFloor.transform.GetChild (i).gameObject.activeSelf) {
				idx = i;
			}
		}

		for (int j = 0; j < size; j++) {
			if (femaleHeadObj [idx].GetComponent<MeshFilter> ().sharedMesh == femaleHead [j]) {
				femaleHeadObj[idx].GetComponent<MeshFilter> ().mesh = femaleHead [(j + 1) % size];
				maleHeadObj[idx].GetComponent<MeshFilter> ().mesh = maleHead [(j + 1) % size];
				break;
			}
		}
	}

	public void changeBody() {
		// child0(idx = 0) is left, child1(idx = 1) is front, child2(idx = 2) is right
		int idx = 0;
		for (int i = 0; i < size; i++) {
			if (bballerFloor.transform.GetChild (i).gameObject.activeSelf) {
				idx = i;
			}
		}

		for (int j = 0; j < size; j++) {
			if (femaleBodyObj [idx].GetComponent<SkinnedMeshRenderer> ().sharedMesh == femaleBody [j]) {
				femaleBodyObj[idx].GetComponent<SkinnedMeshRenderer> ().sharedMesh = femaleBody [(j + 1) % size];
				maleBodyObj[idx].GetComponent<SkinnedMeshRenderer> ().sharedMesh = maleBody [(j + 1) % size];
				break;
			}
		}
	}

	public void changeJersey() {
		Vector2 jerseyOffset;
		// child0(idx = 0) is left, child1(idx = 1) is front, child2(idx = 2) is right
		int idx = 0;
		for (int i = 0; i < size; i++) {
			if (bballerFloor.transform.GetChild (i).gameObject.activeSelf) {
				idx = i;
			}
		}

		jerseyOffset = new Vector2(jerseyMat [idx].mainTextureOffset.x + 0.2f, jerseyMat [idx].mainTextureOffset.y);
		jerseyMat [idx].mainTextureOffset = jerseyOffset;
	}

	public void changeShoes() {
		Vector2 shoesOffset;
		// child0(idx = 0) is left, child1(idx = 1) is front, child2(idx = 2) is right
		int idx = 0;
		for (int i = 0; i < size; i++) {
			if (bballerFloor.transform.GetChild (i).gameObject.activeSelf) {
				idx = i;
			}
		}

		shoesOffset = new Vector2(shoesMat [idx].mainTextureOffset.x + 0.2f, shoesMat [idx].mainTextureOffset.y);
		shoesMat [idx].mainTextureOffset = shoesOffset;
	}

	public void changeHair() {
		// child0(idx = 0) is left, child1(idx = 1) is front, child2(idx = 2) is right
		int idx = 0;
		for (int i = 0; i < size; i++) {
			if (bballerFloor.transform.GetChild (i).gameObject.activeSelf) {
				idx = i;
			}
		}

		for (int j = 0; j < hairSize; j++) {
			if (maleHairObj [idx].GetComponent<MeshFilter> ().sharedMesh == hair [j]) {
				femaleHairObj[idx].GetComponent<MeshFilter> ().mesh = hair [(j + 1) % hairSize];
				maleHairObj[idx].GetComponent<MeshFilter> ().mesh = hair [(j + 1) % hairSize];
				break;
			}
		}
	}
}
