using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Speedup")]
public class SpeedUp : PowerupEffect
{
    public float amount;

    public override void Apply(GameObject target)
    {
        target.GetComponent<Movement>().moveSpeed += amount;
        if (target.GetComponent<Movement>().moveSpeed < 0)
        {
            target.GetComponent<Movement>().moveSpeed = 1;
        }
    }

}
