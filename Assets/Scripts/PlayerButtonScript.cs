using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerButtonScript : MonoBehaviour {

	public GameObject enablePanel;

	public GameObject disablePanel1;

	public GameObject disablePanel2;

    public GameObject teamPanel;

	public void Enable()
	{
		enablePanel.SetActive (true);
		disablePanel1.SetActive (false);
		disablePanel2.SetActive (false);
        teamPanel.SetActive(false);
	}
}
