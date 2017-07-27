using UnityEngine;
using System.Collections;

public class RotateTowards : MonoBehaviour {

    public float targetAngle = 0;
    public float speed = 120;
    float angle = 0;
    public float testAng = 0;
	
	
	// Update is called once per frame
	void Update () 
    {
        testAng = targetAngle - this.transform.rotation.y;
        if (!((testAng-targetAngle) > -5.0f || (testAng-targetAngle) < 5.0f))
        {
            angle = Mathf.MoveTowardsAngle(this.transform.rotation.y, targetAngle, speed * Time.deltaTime);
            this.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
        
	}
}
