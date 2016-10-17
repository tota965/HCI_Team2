using UnityEngine;
using System.Collections.Generic;
using Leap;
using Leap.Unity;

public class LeapPersonController : MonoBehaviour
{
    PersonController pc;
    LeapProvider provider;
    public LeapHandController hc;
    private HandModel hm;
    private bool leftFist;

    void Start()
    {
        provider = FindObjectOfType<LeapProvider>() as LeapProvider;
        pc = FindObjectOfType<PersonController>() as PersonController;
    }

    void Update()
    {
        foreach (var h in provider.CurrentFrame.Hands)
        {
            if (h.IsLeft)
            {
                foreach (var f in h.Fingers)
                {
                    if (f.Type == Finger.FingerType.TYPE_INDEX)
                    {
                        if (f.IsExtended)
                        {
                            Debug.Log("Go");
                            pc.moveForward = true;
                        } else
                        {
                            Debug.Log("Stop");
                            pc.moveForward = false;
                        }
                    }
                }
            }
            if (h.IsRight)
            {
                Debug.Log(h.PalmNormal.Roll);
                Vector3 palmVector = h.PalmNormal.ToVector3();
                Debug.Log(string.Format("ANGLE BETWEEN: {0}",Vector3.Angle(palmVector, Vector3.down)));
                leftFist = true;
                foreach (var f in h.Fingers)
                {
                    if (f.IsExtended)
                    { 
                        if ((f.Type != Finger.FingerType.TYPE_THUMB) && (f.Type != Finger.FingerType.TYPE_INDEX))
                        {
                            leftFist = false;
                        }
                        
                    }
                }
                pc.turnLeft = false;
                pc.turnRight = false;
                if (!leftFist)
                {
                    if (Vector3.Angle(palmVector, Vector3.down) > 120)
                    {
                        pc.turnRight = true;
                        pc.turnLeft = false;
                    }
                    else if (Vector3.Angle(palmVector, Vector3.down) < 60)
                    {
                        pc.turnLeft = true;
                        pc.turnRight = false;
                    }
                }
            }
        }
    }
}