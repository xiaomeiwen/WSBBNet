using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BtnMainOption : MonoBehaviour {
	public GameObject moreOptions;
	public GameObject mute;

	void Start() {
		moreOptions.SetActive (false);
		mute.SetActive (false);
	}

	public void pressOptionBtn() {
		if (moreOptions.activeSelf == false) {
			moreOptions.SetActive (true);
		} else {
			moreOptions.SetActive (false);
		}
	}

	public void pressSoundBtn() {
		if (mute.activeSelf == false) {
			mute.SetActive (true);
		} else {
			mute.SetActive (false);
		}
	}
}
