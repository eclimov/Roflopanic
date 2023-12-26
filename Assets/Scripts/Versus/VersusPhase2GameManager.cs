using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersusPhase2GameManager : MonoBehaviour
{
    public VersusGameManager versusGameManager;

    public VersusPlayer playerOne;
    public RectTransform playerOneControl;

    public VersusPlayer playerTwo;
    public RectTransform playerTwoControl;

    public CountdownTimer countdownTimer;
    public GameObject victoryTextPrefab;
    public GameObject orangePrefab;
    public AudioClip musicPhaseTwo;

    private Vector3 mousePos;
    private Camera mainCam;

    private float timeBetweenPlayerMove = .3f;
    private float moveTimeP1;
    private float moveTimeP2;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;

        StartCoroutine(StartCountdownTimerDelayed(1f));

        countdownTimer.OnTick += OnTimerTick;
        playerOne.OnDeath += OnVictoryP2; // If p1 dies, p2 wins. And vice-versa
        playerTwo.OnDeath += OnVictoryP1;

        if (versusGameManager.IsPlayerOneAI())
        {
            playerTwo.OnProjectileShot += MovePlayerOneAtRandomPosition;
        }
        if (versusGameManager.IsPlayerTwoAI())
        {
            playerOne.OnProjectileShot += MovePlayerTwoAtRandomPosition;
        }
    }

    private void OnDestroy()
    {
        countdownTimer.OnTick -= OnTimerTick;
        playerOne.OnDeath -= OnVictoryP2;
        playerTwo.OnDeath -= OnVictoryP1;

        if (versusGameManager.IsPlayerOneAI())
        {
            playerTwo.OnProjectileShot -= MovePlayerOneAtRandomPosition;
        }
        if (versusGameManager.IsPlayerTwoAI())
        {
            playerOne.OnProjectileShot -= MovePlayerTwoAtRandomPosition;
        }
    }

    private IEnumerator StartSpawningOrangesDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        WaitForSeconds cachedWaitForSeconds = new WaitForSeconds(2f);
        bool spawnForPlayerOne = Random.Range(0, 2) == 0;
        while(!playerOne.IsDead() && !playerTwo.IsDead())
        {
            Instantiate(
                orangePrefab,
                spawnForPlayerOne ? GetRandomPointWithinRectTransform(playerOneControl) : GetRandomPointWithinRectTransform(playerTwoControl), 
                Quaternion.identity,
                gameObject.transform
            );
            spawnForPlayerOne = !spawnForPlayerOne;
            yield return cachedWaitForSeconds;
        }
    }

    private IEnumerator StartCountdownTimerDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        countdownTimer.SetCountdownTime(5);
    }

    private void OnTimerTick(int timeRemains)
    {
        if(timeRemains == 3)
        {
            playerOne.DisableShield();
            playerTwo.DisableShield();
        }

        if (timeRemains == 0)
        {
            AudioManager.instance.PlayMusic(musicPhaseTwo);

            playerOne.StartSpawningProjectiles();
            playerTwo.StartSpawningProjectiles();

            if (versusGameManager.IsPlayerOneAI())
            {
                StartCoroutine(StartInfiniteMovementPlayerOneAI());
            }

            if (versusGameManager.IsPlayerTwoAI())
            {
                StartCoroutine(StartInfiniteMovementPlayerTwoAI());
            }

            StartCoroutine(StartSpawningOrangesDelayed(2f));
        }
    }

    private void OnVictoryP1()
    {
        AudioManager.instance.StopMusic();

        TitleFadeText victoryText = Instantiate(victoryTextPrefab, GameObject.Find("Canvas Players").transform).GetComponent<TitleFadeText>();
        victoryText.SetColor(new Color32(112, 191, 255, 255)); // Blue
        victoryText.SetText(playerOne.GetName(), TitleFadeText.TitleTextType.Versus);
        victoryText.FadeIn();
    }

    private void OnVictoryP2()
    {
        AudioManager.instance.StopMusic();

        TitleFadeText victoryText = Instantiate(victoryTextPrefab, GameObject.Find("Canvas Players").transform).GetComponent<TitleFadeText>();
        victoryText.SetColor(new Color32(255, 117, 117, 255)); // Red
        victoryText.SetText(playerTwo.GetName(), TitleFadeText.TitleTextType.Versus);
        victoryText.FadeIn();
    }

    private Vector3 GetRandomPointWithinRectTransform(RectTransform rectTransform)
    {
        Vector3[] v = new Vector3[4];
        rectTransform.GetWorldCorners(v); // https://docs.unity3d.com/ScriptReference/RectTransform.GetWorldCorners.html

        return new Vector3(
            Random.Range(v[0].x, v[3].x),
            Random.Range(v[0].y, v[1].y),
            0
        );
    }

    private Vector3 GetAiTargetPosition(RectTransform playerControl)
    {
        VersusOrange[] versusOranges = FindObjectsOfType<VersusOrange>();
        foreach (VersusOrange versusOrange in versusOranges)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(playerControl, versusOrange.gameObject.transform.position))
            {
                if(Random.Range(0, 5) == 0) // Do not merge with the oue IF, because it should have its own probability
                {
                    return versusOrange.gameObject.transform.position;
                }
            }
        }

        return GetRandomPointWithinRectTransform(playerControl);
    }

    private void MovePlayerOneAtRandomPosition()
    {
        if(!playerOne.IsDead())
        {
            AddTargetPositionToPlayer(playerOne, GetAiTargetPosition(playerOneControl), ref moveTimeP1);
        }
    }

    private void MovePlayerTwoAtRandomPosition()
    {
        if (!playerTwo.IsDead())
        {
            AddTargetPositionToPlayer(playerTwo, GetAiTargetPosition(playerTwoControl), ref moveTimeP2);
        }
    }

    private IEnumerator StartInfiniteMovementPlayerOneAI()
    {
        MovePlayerOneAtRandomPosition();
        yield return new WaitForSeconds(Random.Range(.5f, .9f));
    }

    private IEnumerator StartInfiniteMovementPlayerTwoAI()
    {
        MovePlayerTwoAtRandomPosition();
        yield return new WaitForSeconds(Random.Range(.5f, .9f));
    }

    private void AddTargetPositionToPlayer(VersusPlayer player, Vector3 targetPosition, ref float refMoveTime)
    {
        player.AddTargetPosition(targetPosition);
        refMoveTime = Time.time + timeBetweenPlayerMove;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: wrap everything into condition: if game is not paused

        if (Input.GetMouseButton(0))
        {
            // the dimensions represent FOV: https://forum.unity.com/threads/screentoworldpoint-always-the-same.337105/
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // Need to assign the zero, because it gets populated with useless value: https://forum.unity.com/threads/how-to-find-the-mouse-position-in-2d.1074118/#post-6930142

            if (!versusGameManager.IsPlayerOneAI() && RectTransformUtility.RectangleContainsScreenPoint(playerOneControl, mousePos) && !playerOne.IsDead() && Time.time > moveTimeP1)
            {
                AddTargetPositionToPlayer(playerOne, mousePos, ref moveTimeP1);
            }
            if (!versusGameManager.IsPlayerTwoAI() && RectTransformUtility.RectangleContainsScreenPoint(playerTwoControl, mousePos) && !playerTwo.IsDead() && Time.time > moveTimeP2)
            {
                AddTargetPositionToPlayer(playerTwo, mousePos, ref moveTimeP2);
            }
        }
    }
}
