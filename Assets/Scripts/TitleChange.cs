using UnityEngine;
using System.Collections;

public class TitleChange : MonoBehaviour 
{


    public GameObject otherDisable1;

    public GameObject otherDisable2;

    public GameObject OPEnable;

    public GameObject SPEnable;

    public GameObject OPDisable;

    public GameObject SPDisable;

    public void Enable()
    {
        otherDisable1.SetActive(false);
        otherDisable2.SetActive(false);
        OPDisable.SetActive(false);
        SPDisable.SetActive(false);
        OPEnable.SetActive(true);
        SPEnable.SetActive(true);
    }
}
