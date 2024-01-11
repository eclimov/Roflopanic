using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class BackgroundScroller : MonoBehaviour
{
    public bool isMenu;

    [System.Serializable]
    private class BackgroundModel
    {
        public string id;
        public GameObject backgroundPrefab;
    }

    [SerializeField]
    private List<BackgroundModel> backgrounds = new List<BackgroundModel>();

    private GameObject backgroundGameObject;
    private RawImage backgroundRawImage;

    private float speed;

    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main; // Set reneder camera
    }

    // Start is called before the first frame update
    void Start()
    {
        SettingsManager.instance.OnBackgroundChange += LoadCurrentBackground;
        SettingsManager.instance.OnDifficultyChange += SetScrollingSpeed;
    }

    private void OnDestroy()
    {
        SettingsManager.instance.OnBackgroundChange -= LoadCurrentBackground;
        SettingsManager.instance.OnDifficultyChange -= SetScrollingSpeed;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // This cannot be called in Start
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        LoadCurrentBackground();
    }

    protected void SetScrollingSpeed()
    {
        float speedByDifficulty;

        if(isMenu)
        {
            switch (SettingsManager.difficultyId)
            {
                case 1: // Medium
                    speedByDifficulty = .06f;
                    break;
                case 2: // Hardcore
                    speedByDifficulty = .13f;
                    break;
                default: // Easy
                    speedByDifficulty = .02f;
                    break;
            }
        } else
        {
            speedByDifficulty = SettingsManager.instance.GetDifficultyMap().backgroundSpeed;
        }

        speed = speedByDifficulty * backgroundRawImage.uvRect.width;
    }

    private void LoadCurrentBackground()
    {
        BackgroundModel currentBackground = backgrounds.Find(item => item.id == SettingsManager.GetBackground());

        if (backgroundGameObject != null)
        {
            Destroy(backgroundGameObject);
        }
        backgroundGameObject = Instantiate(currentBackground.backgroundPrefab, transform);

        backgroundRawImage = backgroundGameObject.GetComponent<RawImage>();

        SetScrollingSpeed();
    }

    public void NegateScrolling()
    {
        speed = -speed;
    }

    // Update is called once per frame
    void Update()
    {
        backgroundRawImage.uvRect = new Rect(backgroundRawImage.uvRect.x + speed * Time.deltaTime, 0, backgroundRawImage.uvRect.width, backgroundRawImage.uvRect.height);
    }
}
