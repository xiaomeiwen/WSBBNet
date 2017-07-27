using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCourtInfo : MonoBehaviour {
	GameObject courtObj;
	GameObject dataRecorder;
	Material[] courtMats;
	int idx;

	// Use this for initialization
	void Awake () {
		dataRecorder = GameObject.Find ("DataRecorder");
		idx = dataRecorder.GetComponent<DataFromUI> ().selectedCourtIdx;
		courtObj = GameObject.Find ("Court");
//		Debug.Log (idx + " " + courtObj.transform.GetChild (idx).name);
		courtObj.transform.GetChild(idx).gameObject.SetActive(true);

//		courtMats = new Material[]{dataRecorder.GetComponent<DataFromUI> ().selectedCourtMat, dataRecorder.GetComponent<DataFromUI> ().selectedCourtFence};
//		this.GetComponent<MeshFilter>().sharedMesh = dataRecorder.GetComponent<DataFromUI> ().selectedCourtMesh;
//		this.GetComponent<MeshRenderer> ().sharedMaterials = courtMats;
	}
}
