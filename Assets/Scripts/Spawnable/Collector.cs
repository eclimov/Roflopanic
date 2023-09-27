using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            AudioManager.instance.PlayCoinSound();
            collision.gameObject.SetActive(false); // Disable coin
            FindObjectOfType<Player>().OnCoinCollected();
        }
    }
}
