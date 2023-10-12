using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthManager : MonoBehaviour
{
    public Image healthBar;
    public Image delayedHealthBar;
    public float healthAmount = 100f;

    public GameObject shieldLayer;

    public TMP_Text nameText;
    public TMP_Text damageValueText;

    private bool delayedHealthUpdate;

    public delegate void OnBossHealthZeroDelegate();
    public event OnBossHealthZeroDelegate OnBossHealthZero;

    private void Awake()
    {
        damageValueText.gameObject.SetActive(false); // Hide by default
    }

    public void SetBossName(string name)
    {
        nameText.text = name;
    }

    public void DisableShieldLayer()
    {
        shieldLayer.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        if(healthAmount <= 0)
        {
            if (OnBossHealthZero != null) // It is a MUST to check this, because the event is null if it has no subscribers
            {
                OnBossHealthZero();
            }
        }

        healthBar.fillAmount = healthAmount / 100;

        StartCoroutine(ShowDamageText(damage));
        StartCoroutine(DelayedHealthAnimation());
    }

    private IEnumerator ShowDamageText(float damage)
    {
        damageValueText.text = damage.ToString();
        damageValueText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        damageValueText.gameObject.SetActive(false);
    }

    private IEnumerator DelayedHealthAnimation()
    {
        yield return new WaitForSecondsRealtime(1f);
        delayedHealthUpdate = true;
    }

    private void Update()
    {
        if(delayedHealthUpdate)
        {
            if(delayedHealthBar.fillAmount > healthBar.fillAmount)
            {
                delayedHealthBar.fillAmount -= .01f;
            } else
            {
                delayedHealthUpdate = false;
            }
        }
    }
}
