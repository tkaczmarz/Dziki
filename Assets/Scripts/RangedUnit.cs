using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : Unit
{
	public GameObject projectilePrefab;

    private Projectile projectile;

    protected override void Awake() 
    {
        base.Awake();

        if (!projectilePrefab)
            Debug.LogError(name + " has no projectile attached!");
    }

    public override void Attack(SelectableObject target)
    {
        // prevent attacking self or teammate
		if (target == this || target.team == team || target.IsDead)
			return;
		
		// can't attack if too far from target
		if (Vector3.Distance(transform.position, target.transform.position) > attackRange)
			return;

        float dmg = (health / maxHealth) * maxDamage;
        GameObject projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(90, 0, 0));
        projectile = projectileObject.GetComponent<Projectile>();
        if (projectile)
        {
            projectile.Launch(target, dmg);
            FinishMove();
        }
        else
            Debug.LogError(name + ": projectile not found");
    }
}
