using UnityEngine;
using System.Collections;

public class RightButton : MonoBehaviour 
{

    public GameObject left;
    public GameObject center;
    public GameObject right;

    public void Move()
    {
        Vector3 leftPos = left.transform.position;
        Vector3 centPos = center.transform.position;
        Vector3 rightPos = right.transform.position;

        GameObject tempGO;

        left.transform.position = centPos;
        center.transform.position = rightPos;
        right.transform.position = leftPos;
        tempGO = center;
        center = left;
        GetComponent<LeftButton>().center = left;
        left = right;
        GetComponent<LeftButton>().left = right;
        right = tempGO;
        GetComponent<LeftButton>().right = tempGO;

    }
}
