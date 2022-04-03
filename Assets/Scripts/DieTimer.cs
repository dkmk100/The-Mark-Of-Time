using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieTimer : MonoBehaviour
{
    public float timeUntilDeath = 1.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        timeUntilDeath -= Time.fixedDeltaTime;
        if (timeUntilDeath <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
