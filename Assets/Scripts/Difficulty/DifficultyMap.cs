using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyMap
{
    public float backgroundSpeed;
    public float obstacleTimeBetweenSpawn;
    public float obstacleSpeed; // Coin speed should be the same
    public float playerSpeed;
    public float scoreIncrementMultiplier;
    public float coinBonusScore;

    public DifficultyMap(
        float backgroundSpeed,
        float obstacleTimeBetweenSpawn,
        float obstacleSpeed,
        float playerSpeed,
        float scoreIncrementMultiplier,
        float coinBonusScore
    )
    {
        this.backgroundSpeed = backgroundSpeed;
        this.obstacleTimeBetweenSpawn = obstacleTimeBetweenSpawn;
        this.obstacleSpeed = obstacleSpeed;
        this.playerSpeed = playerSpeed;
        this.scoreIncrementMultiplier = scoreIncrementMultiplier;
        this.coinBonusScore = coinBonusScore;
    }
}
