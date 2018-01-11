using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogBase : MonoBehaviour 
{
	protected UnityAction action = null;

    protected void OnDisable() 
    {
        if (action != null)
        {
            action.Invoke();
            action = null;
        }
    }

    protected IEnumerator ActivateForTime(float activeTime)
    {
        float timer = 0.0f;
        while (timer < activeTime)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
