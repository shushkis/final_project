using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UXF;

public class SessionMonitor : MonoBehaviour
{
    public GameObject fixation;
    public GameObject target;
    public bool trailBegin = false;
    public bool fixation_check = false;
    public bool target_check = false;
    public bool targetPresent = false;
    public bool clutter_triggered = false;

    public GameObject[] clutter_objs;
    public List<string> posList = new List<string>();
    public int current_block_number = -1;
    public int current_block_number_2 = 0;
    public enum State
    {
        Begin,
        Fixation,
        Target
    }
    public State cur_state = State.Begin;

    private void Start()
    {
        //gameObject.SetActive(false);
        clutter_objs = GameObject.FindGameObjectsWithTag("clutter");
        foreach (var clutter_obj in clutter_objs)
        {
            clutter_obj.SetActive(false);
        }
        //GameObject exGen = GameObject.GetComponent<"ExperimentGenerator">
        //posList = exGen.posList;

    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Session.instance.currentTrialNum" + Session.instance.currentTrialNum);
        
        if (!Session.instance.hasInitialised) {
            return;
        }
        else
        {
            if (Session.instance.currentBlockNum > current_block_number_2)
            {
                clutter_triggered = false;
            }
            current_block_number_2 = Session.instance.currentBlockNum;  

            if (Session.instance.currentBlockNum > current_block_number)
            {
                posList = Session.instance.settings.GetStringList("posList");
                for(int i = 0; i < Session.instance.settings.GetInt("times_to_repeat"); i++)
                {
                    posList.AddRange(posList);
                }
                current_block_number ++;
            }

            
            State next_state = decide_next_step(cur_state);

            switch (next_state)
            {
                case State.Begin:
                    begin_trial();
                    break;
                case State.Fixation:
                    fixation_stage();
                    break;
                case State.Target:
                    target_stage();
                    break;
            }
            cur_state = next_state;
        }
        if (Session.instance.currentBlockNum % 2 == 0) {
            if (clutter_triggered)
            {
                return;
            }
            clutter_triggered = true;
            foreach(var clutter_obj in clutter_objs)
            {
                clutter_obj.SetActive(true);
            }
        }
        else
        {
            if (clutter_triggered)
            {
                return;
            }
            clutter_triggered = true;
            foreach (var clutter_obj in clutter_objs)
            {
                clutter_obj.SetActive(false);
            }
        }
    }

    private void begin_trial()
    {
        trailBegin = true;
        Session.instance.BeginNextTrial();    
    }
    private void fixation_stage()
    {
        if (fixation.activeSelf) return;
        target.SetActive(true);
        targetPresent = true;
    }
    private void target_stage()
    {
        if (!target.activeSelf && trailBegin && !targetPresent) // let's finish 
        {
            trailBegin = false;
            return;
        }
        if (targetPresent)
        {
            targetPresent = false;
            Debug.Log(" posList.Count: " +  posList.Count);
            string curr_exp_pos = posList[Random.Range(0, posList.Count)];
            posList.Remove(curr_exp_pos);
            Session.instance.settings.SetValue("currPos", curr_exp_pos);
            //string side = sides[Random.Range(0, sides.Count)];
            //string depth = depthsList[Random.Range(0, depthsList.Count)];
            target.SendMessage("setExpValues", curr_exp_pos);
        }
        
    }
    State decide_next_step(State cur_state)
    {
        switch (cur_state){
            case State.Begin:
                cur_state = move_to_fixation()? State.Fixation : State.Begin;
                Debug.Log("State.Begin cur_state: " + cur_state);
                break;
            case State.Fixation:
                Debug.Log("State.Fixation cur_state: " + cur_state);
                cur_state = move_to_target()? State.Target : State.Fixation;
                break;
            case State.Target:
                Debug.Log("State.Target cur_state: " + cur_state);
                cur_state = move_to_begin()? State.Begin : State.Target; 
                break;
        }
        return cur_state;
    }

    bool move_to_begin()
    {
        Debug.Log("move_to_begin" + !target.activeSelf);
        return !trailBegin; // target object is off task is done.
    }

    bool move_to_fixation()
    {
        Debug.Log("move_to_fixation");
        return trailBegin;
    }
    bool move_to_target()
    {
        Debug.Log("move_to_target" + !fixation.activeSelf);
        return targetPresent;
    }

}
