using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour 
{
    public float speed = 5;

	public void Launch(SelectableObject target, float damage)
    {
        transform.LookAt(target.transform);
        Vector3 rot = transform.rotation.eulerAngles;
        rot.x = 90;
        transform.rotation = Quaternion.Euler(rot);
        StartCoroutine(Fly(target, damage));
    }

    public IEnumerator Fly(SelectableObject target, float damage)
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
            yield return null;
        }

        target.TakeDamage(damage);
        Destroy(gameObject);
    }
}
