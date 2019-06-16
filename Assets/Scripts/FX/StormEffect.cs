using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormEffect : MonoBehaviour
{

    public float minTime = 5f;
    public float maxTime = 13f;
    private float time;
    private float targetTime = -1;

    private void Update()
    {
        if (targetTime == -1)
        {
            targetTime = Random.Range(minTime, maxTime);
        }
        time += Time.deltaTime;
        if (time >= targetTime)
        {
            time = 0;
            targetTime = -1;

            StartCoroutine(StartFlash());
        }
    }

    private IEnumerator StartFlash()
    {
        Color startingColor = Camera.main.backgroundColor;
        Camera.main.backgroundColor = Color.white;
        yield return null;
        yield return null;
        Camera.main.backgroundColor = startingColor;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        Camera.main.backgroundColor = Color.white;
        yield return null;
        yield return null;
        Camera.main.backgroundColor = startingColor;

    }

}
