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


public class EyeTracker1 : Tracker
{
    public Transform cam;

    public float maxFocusDistance = 100f;
    public LayerMask focusMask;
    public bool debug = true;

    private readonly GazeIndex[] idxPriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };

    protected override UXFDataRow GetCurrentValues()
    {
        var row = new UXFDataRow();

        RecordGaze(row);
        RecordEyeData(row);
        RecordEyeFocus(row);

        return row;
    }

    void RecordGaze(UXFDataRow row)
    {
        Vector3 gazeOriginCombinedLocal = new Vector3(float.NaN, float.NaN, float.NaN);
        Vector3 gazeDirectionCombinedLocal = new Vector3(float.NaN, float.NaN, float.NaN); ;

        bool gaze = false;
        foreach (var idx in idxPriority)
        {
            gaze = SRanipal_Eye_v2.GetGazeRay(idx, out gazeOriginCombinedLocal, out gazeDirectionCombinedLocal);
            if (gaze) break;
        }

        if (gaze)
        {
            Vector3 gazeOriginCombined = cam.TransformPoint(gazeOriginCombinedLocal);
            Vector3 gazeDirectionCombined = cam.TransformDirection(gazeDirectionCombinedLocal);

            if (debug)
            {
                Debug.DrawRay(gazeOriginCombined, gazeDirectionCombined, Color.red);
            }

            row.Add(("gaze_origin_x", gazeOriginCombined.x));
            row.Add(("gaze_origin_y", gazeOriginCombined.y));
            row.Add(("gaze_origin_z", gazeOriginCombined.z));
            row.Add(("gaze_direction_x", gazeDirectionCombined.x));
            row.Add(("gaze_direction_y", gazeDirectionCombined.y));
            row.Add(("gaze_direction_z", gazeDirectionCombined.z));
        }
        else
        {
            row.Add(("gaze_origin_x", "NA"));
            row.Add(("gaze_origin_y", "NA"));
            row.Add(("gaze_origin_z", "NA"));
            row.Add(("gaze_direction_x", "NA"));
            row.Add(("gaze_direction_y", "NA"));
            row.Add(("gaze_direction_z", "NA"));
        }
    }


    void RecordEyeData(UXFDataRow row)
    {
        VerboseData vEyeData = new VerboseData();
        bool success = SRanipal_Eye_v2.GetVerboseData(out vEyeData);

        row.Add(("eye_openness_left", vEyeData.left.eye_openness));
        row.Add(("eye_openness_right", vEyeData.right.eye_openness));
        row.Add(("pupil_diameter_left", vEyeData.left.pupil_diameter_mm));
        row.Add(("pupil_diameter_right", vEyeData.right.pupil_diameter_mm));
    }

    void RecordEyeFocus(UXFDataRow row)
    {
        Ray gazeRay = new Ray();
        FocusInfo focusInfo = new FocusInfo();
        bool focus = false;
        foreach (var idx in idxPriority)
        {
            focus = SRanipal_Eye_v2.Focus(idx, out gazeRay, out focusInfo, 0.1f, maxFocusDistance, focusMask);
            if (focus) break;
        }

        if (focus)
        {
            if (debug)
            {
                Debug.LogFormat("Focus object: {0}", focusInfo.transform.name);
            }

            row.Add(("focus_object_raw", focusInfo.transform.name));
            row.Add(("focus_point_x", focusInfo.point.x));
            row.Add(("focus_point_y", focusInfo.point.y));
            row.Add(("focus_point_z", focusInfo.point.z));
            row.Add(("focus_distance", focusInfo.distance));
        }
        else
        {
            row.Add(("focus_object_raw", "NA"));
            row.Add(("focus_point_x", "NA"));
            row.Add(("focus_point_y", "NA"));
            row.Add(("focus_point_z", "NA"));
            row.Add(("focus_distance", "NA"));
        }
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
            "pupil_diameter_left",
            "pupil_diameter_right",
            "focus_object_raw",
            "focus_point_x",
            "focus_point_y",
            "focus_point_z",
            "focus_distance"
        };
}