using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class FixationManager : MonoBehaviour
{
    public float fixationTimer = 2.0f; // 5seconds....
    private void Start()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(!Session.instance.hasInitialised)
        {
            return;
        }
        if(gameObject.activeSelf) 
        {
            fixationTimer -= Time.deltaTime;

            if (fixationTimer <= 0.0f)
            {
                timerEnded();
            }
        }
        void timerEnded()
        {
            fixationTimer = 2.0f; // 5seconds....
            gameObject.SetActive(false);
        }
    }
}
