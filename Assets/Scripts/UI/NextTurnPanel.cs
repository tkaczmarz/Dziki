using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextTurnPanel : MonoBehaviour 
{
	public Image colorBackground;
    public Text text;

    public void Show(Team team, float activeTime)
    {
        gameObject.SetActive(true);
        colorBackground.color = team.color;
        text.text = "Turn of team " + team.nr;
        StartCoroutine(ActivateForTime(activeTime));
    }

    public void Show(string msg, Team team, float activeTime)
    {
        gameObject.SetActive(true);
        colorBackground.color = team.color;
        text.text = msg;
        StartCoroutine(ActivateForTime(activeTime));
    }

    private IEnumerator ActivateForTime(float activeTime)
    {
        float timer = 0.0f;
        while (timer < activeTime)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        gameObject.SetActive(false);
    }
}
