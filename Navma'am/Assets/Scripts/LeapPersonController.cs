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
            if (h.IsRight)
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
            if (h.IsLeft)
            {
                leftFist = true;
                foreach (var f in h.Fingers)
                {
                    if (f.IsExtended)
                    {
                        Debug.Log(string.Format("FINGER: {0}", f.Type));
                        if ((f.Type != Finger.FingerType.TYPE_THUMB) && (f.Type != Finger.FingerType.TYPE_INDEX))
                        {
                            leftFist = false;
                            Debug.Log("No Fist");
                        }
                        
                    }
                }
                if (!leftFist)
                {
                    if (h.Direction.Roll > 0)
                    {
                        pc.turnRight = true;
                        pc.turnLeft = false;
                    }
                    else if (h.Direction.Roll < 0)
                    {
                        pc.turnLeft = true;
                        pc.turnRight = false;
                    }
                } else
                {
                    pc.turnLeft = false;
                    pc.turnRight = false;
                }
            }
        }
    }
}