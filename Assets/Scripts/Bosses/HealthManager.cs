using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public Image delayedHealthBar;
    public float healthAmount = 100f;

    public GameObject shieldLayer;

    public TMP_Text nameText;
    public TMP_Text damageValueText;

    public LocalizedString shieldDisabledString;
    public LocalizedString shieldEnabledString;

    private bool delayedHealthUpdate;

    public delegate void OnHealthZeroDelegate();
    public event OnHealthZeroDelegate OnHealthZero;

    private void Awake()
    {
        damageValueText.gameObject.SetActive(false); // Hide by default
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void SetNameColor(Color32 color)
    {
        nameText.color = color;
    }

    public void EnableShieldLayer()
    {
        shieldLayer.SetActive(true);
        StartCoroutine(ShowInfoText(shieldEnabledString.GetLocalizedString()));
    }

    public void DisableShieldLayer()
    {
        shieldLayer.SetActive(false);
        StartCoroutine(ShowInfoText(shieldDisabledString.GetLocalizedString()));
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        if(healthAmount <= 0)
        {
            if (OnHealthZero != null) // It is a MUST to check this, because the event is null if it has no subscribers
            {
                OnHealthZero();
            }
        }

        healthBar.fillAmount = healthAmount / 100;

        StartCoroutine(ShowInfoText(damage.ToString()));
        StartCoroutine(DelayedHealthAnimation());
    }

    public void Heal(float amount)
    {
        healthAmount += amount;
        if(healthAmount > 100)
        {
            healthAmount = 100;
        }

        healthBar.fillAmount = healthAmount / 100;
        delayedHealthBar.fillAmount = healthAmount / 100; // Delayed bar is updated as well
    }

    private IEnumerator ShowInfoText(string infoText)
    {
        damageValueText.text = infoText;
        damageValueText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        damageValueText.gameObject.SetActive(false);
    }

    private IEnumerator DelayedHealthAnimation()
    {
        yield return new WaitForSeconds(1f);
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
