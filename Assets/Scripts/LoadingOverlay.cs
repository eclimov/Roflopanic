using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadingOverlay : MonoBehaviour
{
    public GameObject loadingImageGameObject;
    public TMP_Text loadingText;

    public Sprite[] spritesToChooseFrom;

    private readonly int imageRotationSpeed = 125;

    private void OnEnable()
    {
        // Set random sprite
        loadingImageGameObject.GetComponent<Image>().sprite = spritesToChooseFrom[Random.Range(0, spritesToChooseFrom.Length)];

        // Set random sprite rotation
        loadingImageGameObject.transform.Rotate(0, 0, Random.Range(-100, 100));

        // Set random text color
        int randomColorId = Random.Range(0, 5);
        switch(randomColorId)
        {
            case 0:
                loadingText.color = new Color32(61, 183, 255, 255); // Blue
                break;
            case 1:
                loadingText.color = new Color32(255, 204, 3, 255); // Yellow
                break;
            case 2:
                loadingText.color = new Color32(150, 195, 98, 255); // Green
                break;
            case 3:
                loadingText.color = new Color32(255, 117, 117, 255); // Red
                break;
            case 4:
                loadingText.color = new Color32(255, 97, 247, 255); // Purple
                break;
        }

        Time.timeScale = 0f;
        AudioManager.instance.PauseMusic();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        AudioManager.instance.UnpauseMusic();
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate image
        loadingImageGameObject.transform.Rotate(0, 0, (-1) * imageRotationSpeed * Time.unscaledDeltaTime);
    }
}
