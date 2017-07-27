using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeLabelText : MonoBehaviour {


    public void ChangeText(string text)
    {

        this.GetComponentInChildren<Text>().text = text;
    }
}
