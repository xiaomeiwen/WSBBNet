using UnityEngine;
using System.Collections;

public class Turntable : MonoBehaviour {

    public GameObject left;
    public GameObject center;
    public GameObject right;
    //public GameObject rotation;

    public void Start()
    {
        Animator anim;
        anim = center.GetComponent<Animator>();
        anim.SetBool("Front", true);
    }

    public void Move(bool direction)
    {
        Animator anim;
        Animator animLeft;
        Animator animRight;
        Vector3 leftPos = left.transform.position;
        Vector3 centPos = center.transform.position;
        Vector3 rightPos = right.transform.position;

        GameObject tempGO;

        if (direction)
        {
            //move right
            //left.transform.position = centPos;
            //center.transform.position = rightPos;
            //right.transform.position = leftPos;
            tempGO = center;
            center = left;
            left = right;
            right = tempGO;
        }
        else
        {
            //move left
            //left.transform.position = rightPos;
            //center.transform.position = leftPos;
            //right.transform.position = centPos;
            tempGO = center;
            center = right;
            right = left;
            left = tempGO;
            
        }
        //rotation.GetComponent<RotateTowards>().targetAngle = 120;
        anim = center.GetComponent<Animator>();
        anim.SetBool("Front", true);
        animRight = right.GetComponent<Animator>();
        animRight.SetBool("Front", false);
        animLeft = left.GetComponent<Animator>();
        animLeft.SetBool("Front", false);
        left.transform.rotation = Quaternion.identity;
        left.transform.Rotate(Vector3.up, 180);
        center.transform.rotation = Quaternion.identity;
        center.transform.Rotate(Vector3.up, 180);
        right.transform.rotation = Quaternion.identity;
        right.transform.Rotate(Vector3.up, 190);
        
    }
}
