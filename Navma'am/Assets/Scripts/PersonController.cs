using UnityEngine;
using System.Collections;

public class PersonController : MonoBehaviour {
    public float walkSpeed;
    public float turnSpeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {    
	    if((Input.GetAxis("Horizontal")) < 0)
        {
            //Turn left (rotate in -y)
            transform.Rotate(-Vector3.up * Time.deltaTime * turnSpeed);

        } else if((Input.GetAxis("Horizontal")) > 0)
        {
            //Turn right (rotate in +y)
            transform.Rotate(Vector3.up * Time.deltaTime * turnSpeed);
        }
        if((Input.GetAxis("Vertical") > 0))
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
