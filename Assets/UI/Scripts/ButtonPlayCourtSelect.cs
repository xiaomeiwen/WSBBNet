using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonPlayCourtSelect : MonoBehaviour {

	public Mesh[] courtMesh;
	public Material[] courtMat;
	public Material[] courtFence;

	GameObject dataRecorder;

	void Start() {
		dataRecorder = GameObject.Find ("DataRecorder");
	}

	public void OnPress(){
		int idx = CourtImageAssign.courtIdx;
		dataRecorder.GetComponent<DataFromUI> ().selectedCourtIdx = idx;
//		dataRecorder.GetComponent<DataFromUI> ().selectedCourtMesh = courtMesh [idx];
//		dataRecorder.GetComponent<DataFromUI> ().selectedCourtMat = courtMat [idx];
//		dataRecorder.GetComponent<DataFromUI> ().selectedCourtFence = courtFence [idx];
		SceneManager.LoadScene ("GameScreen");
	}
}
