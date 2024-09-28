using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneToggle : MonoBehaviour
{

    public float targetTime = 60.0f;

    void Update()
    {

        targetTime -= Time.deltaTime;

        if (targetTime <= 0.0f)
        {
            timerEnded();
        }

    }

    void timerEnded()
    {
        Debug.Log("switching scene");
        SceneManager.LoadScene("Garage");
    }


}