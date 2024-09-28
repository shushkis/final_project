using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UXF;
using ViveSR.anipal.Eye;


//
// Script for supporting Eye Tracking from the Vive Pro Eye with UXF. Inherits from Tracker.
// Attach this script to an object and then reference it in your Session component (UXF_Rig) under tracked objects.
// This is the same method you would attach a PositionRotationTracker.
//credit:
//https://gist.github.com/jackbrookes/774af439d580cd9a9d3a8611b1167e5d
//


public class EyeTracker : Tracker
{
    public Transform cam;

    public float maxFocusDistance = 100f;
    public LayerMask focusMask;
    public bool debug = true;

    private readonly GazeIndex[] idxPriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };

    public SaccadeDetection saccadeDetection;


    // DATA
    int timestamp;
    float speed;
    float acceleration;
    bool groundTruth;
    float eyeOpenessLeft;
    float eyeOpenessRight;
    Vector3 eyeDirectionLocal;
    Vector3 eyeOriginLocal;
    Vector3 eyeDirectionGlobal;
    Vector3 eyeOriginGlobal;
    Vector3 eyeDirectionLeftLocal;
    Vector3 eyeOriginLeftLocal;
    Vector3 eyeDirectionRightLocal;
    Vector3 eyeOriginRightLocal;
    Vector3 eyeDirectionLeftGlobal;
    Vector3 eyeDirectionRightGlobal;
    Vector3 eyeOriginLeftGlobal;
    Vector3 eyeOriginRightGlobal;
    RaycastHit hit;                             // collision information from combined eye gaze

    public GameObject toolBox;

    private void Start()
    {
        saccadeDetection.SaccadeDetectionDataAvailable.AddListener(getSaccadeDetectionData);
    }


    void getSaccadeDetectionData()
    {
        saccadeDetection.SendLoggingData(out timestamp, out speed, out acceleration, out eyeOpenessLeft,
                                     out eyeOpenessRight, out eyeDirectionLocal, out eyeOriginLocal, out eyeDirectionLeftLocal,
                                     out eyeOriginLeftLocal, out eyeDirectionRightLocal, out eyeOriginRightLocal);
    }

    protected override UXFDataRow GetCurrentValues()
    {
        var row = new UXFDataRow();

        
        row.Add(("gaze_origin_x", eyeOriginLocal.x));
        row.Add(("gaze_origin_y", eyeOriginLocal.y));
        row.Add(("gaze_origin_z", eyeOriginLocal.z));
        row.Add(("gaze_direction_x", eyeDirectionLocal.x));
        row.Add(("gaze_direction_y", eyeDirectionLocal.y));
        row.Add(("gaze_direction_z", eyeDirectionLocal.z));

        row.Add(("eye_openness_left", eyeOpenessLeft));
        row.Add(("eye_openness_right", eyeOpenessRight));
        row.Add(("speed", speed));
        row.Add(("acceleration", acceleration));
        
        bool saccade = saccadeDetection.saccade;
        row.Add(("saccade", saccade));
        eyeDirectionGlobal = Camera.main.transform.TransformDirection(eyeDirectionLocal);
        eyeOriginGlobal = Camera.main.transform.TransformPoint(eyeOriginLocal);

        bool gazeCollision = checkGazeCollision();
        row.Add(("gazeCollision", gazeCollision));

        row.Add(("gaze_origin_global_x", eyeOriginGlobal.x));
        row.Add(("gaze_origin_global_y", eyeOriginGlobal.y));
        row.Add(("gaze_origin_global_z", eyeOriginGlobal.z));
        row.Add(("gaze_direction_global_x", eyeDirectionGlobal.x));
        row.Add(("gaze_direction_global_y", eyeDirectionGlobal.y));
        row.Add(("gaze_direction_global_z", eyeDirectionGlobal.z));

        

        return row;
    }

    private bool checkGazeCollision()
    {
        if (Physics.Raycast(eyeOriginGlobal, eyeDirectionGlobal, out hit))
        {
            Debug.Log("Basic yes");
            return hit.transform.name.Equals(toolBox.name);
        }
        Debug.Log("Basic No");
        return false;
    }

    public override string MeasurementDescriptor => "eye_tracking";
    public override IEnumerable<string> CustomHeader => new string[] {
            "gaze_origin_x",
            "gaze_origin_y",
            "gaze_origin_z",
            "gaze_direction_x",
            "gaze_direction_y",
            "gaze_direction_z",
            "eye_openness_left",
            "eye_openness_right",
            "speed",
            "acceleration",
            "saccade",
            "gazeCollision",
            "gaze_origin_global_x",
            "gaze_origin_global_y",
            "gaze_origin_global_z",
            "gaze_direction_global_x",
            "gaze_direction_global_y",
            "gaze_direction_global_z"
        };
}