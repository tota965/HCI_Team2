using UnityEngine;
using System.Collections;

public class PersonController : MonoBehaviour {
    public float walkSpeed;
    public float turnSpeed;
    // Use this for initialization
    public bool moveForward;
    public bool turnLeft;
    public bool turnRight;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {    
	    if(turnLeft)
        {
            //Turn left (rotate in -y)
            transform.Rotate(-Vector3.up * Time.deltaTime * turnSpeed);

        } else if(turnRight)
        {
            //Turn right (rotate in +y)
            transform.Rotate(Vector3.up * Time.deltaTime * turnSpeed);
        }
        if((moveForward))
        {
            //Move forward (translate along local x-axis)
            transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed );
        }
        else if ((Input.GetAxis("Vertical") < 0))
        {
            //Move forward (translate along local x-axis)
            transform.Translate(-Vector3.forward * Time.deltaTime * walkSpeed);
        }
    }
}
