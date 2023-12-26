using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersusOrange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<VersusPlayer>(out VersusPlayer versusPlayer))
        {
            versusPlayer.Heal(5);

            Destroy(gameObject);
        }
    }
}
