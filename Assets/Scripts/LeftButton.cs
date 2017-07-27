using UnityEngine;
using System.Collections;

public class LeftButton : MonoBehaviour {

    public GameObject left;
    public GameObject center;
    public GameObject right;

    public void Move()
    {
        Vector3 leftPos = left.transform.position;
        Vector3 centPos = center.transform.position;
        Vector3 rightPos = right.transform.position;

        GameObject tempGO;

        left.transform.position = rightPos;
        center.transform.position = leftPos;
        right.transform.position = centPos;
        tempGO = center;
        center = right;
        GetComponent<RightButton>().center = right;
        left = tempGO;
        GetComponent<RightButton>().left = tempGO;
        right = left;
        GetComponent<RightButton>().right = left;

    }
}
