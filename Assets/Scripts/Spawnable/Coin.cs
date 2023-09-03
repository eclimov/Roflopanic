using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : AbstractSpawnable
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Border")
        {
            gameObject.SetActive(false);
        }
    }
}
