using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Valve.VR;

using static Valve.VR.SteamVR;
using UXF;
using ViveSR.anipal.Eye;

public class TargetManager : MonoBehaviour
{

    public Transform cam;
    public float maxFocusDistance = 100f;
    public LayerMask focusMask;
    private readonly GazeIndex[] idxPriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };


    public float fixationTimer = 2.0f; // 10 seconds....
    public bool receivedMsg = false;
    //public string depth = "";

    public Vector3 pos_near = new Vector3 (-0.079f, 0.783f, - 2.356918f);
    public Vector3 pos_far  = new Vector3(-1.079f, 0.809f, 5.19f);
    public Vector3 cur_pos = new Vector3();

    public IDictionary<string, int> sides = new Dictionary<string, int>() { { "near_left", 1 }, { "far_left", 2 }, { "near_right", -1 }, { "far_right", -2 } };

    public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;//which controller
    public SteamVR_Action_Boolean clickAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");



    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!receivedMsg)
        {
            return;
        }
        
        fixationTimer -= Time.deltaTime;
        bool isPressed = clickAction.GetState(inputSource);
        if (fixationTimer <= 0.0f || isPressed)
        {
            Session.instance.CurrentTrial.result["timeout"] = (fixationTimer <= 0.0f);
            Session.instance.CurrentTrial.result["pressed"] = isPressed;
            timerEnded();
        } 

    }


    

    void timerEnded()
    {
        fixationTimer = 2.0f;
        receivedMsg = false;
        gameObject.SetActive(false);
    }

    void setExpValues(string currPos)//, string side)
    {
        Debug.Log("currPos: " + currPos);
        string depth = currPos.Split("$")[0];
        string side  = currPos.Split("$")[1];
        //List<string> side_keys = Enumerable.ToList(sides.Keys);
        //string side = side_keys[Random.Range(0, side_keys.Count)];
        var side_val = sides[side];
        Session.instance.CurrentTrial.result["depth"] = depth;
        Session.instance.CurrentTrial.result["side"] = side;
        Debug.Log("side: " + side + "val: " + side_val);
        gameObject.transform.position = depth == "near"? pos_near : pos_far;
        gameObject.transform.position += new Vector3(side_val, 0, 0);
        Session.instance.CurrentTrial.result["curr_pos"] = gameObject.transform.position;

        receivedMsg = true;
    }
}

