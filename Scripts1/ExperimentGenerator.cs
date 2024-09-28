using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UXF;

public class ExperimentGenerator : MonoBehaviour
{

    //List<string> depthsList = new List<string>(new string[] { "near", "far" });
    //List<string> sideList = new List<string>(new string[] { "near_right", "near_left", "far_left", "far_right"});
    public void Generate(Session session)
    {
        int num_of_blocks = session.settings.GetInt("num_of_blocks");
        int time_to_repeat = session.settings.GetInt("times_to_repeat");
        int num_of_trials = session.settings.GetInt("num_of_trials"); // near - far times 4 places 2 from left and 2 from right size of posList

        for (int i = 0; i < num_of_blocks; i++)
        {
            session.CreateBlock(num_of_trials * time_to_repeat);
        }
        
        
    }
}