using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersusGameManager : MonoBehaviour
{
    public VersusController playerOneChoiceController;
    public VersusController playerTwoChoiceController;

    public GameObject phaseOneGameobject;
    public GameObject phaseTwoGameobject;

    public AudioClip musicPhaseOne;

    public enum ActorTypesEnum { AI, Player };

    private ActorTypesEnum playerOneType;
    private ActorTypesEnum playerTwoType;

    private void Start()
    {
        AudioManager.instance.PlayMusic(musicPhaseOne);

        playerOneChoiceController.OnPlayerTypeChanged += SetPlayerOneType;
        playerTwoChoiceController.OnPlayerTypeChanged += SetPlayerTwoType;
    }

    private void OnDestroy()
    {
        playerOneChoiceController.OnPlayerTypeChanged -= SetPlayerOneType;
        playerTwoChoiceController.OnPlayerTypeChanged -= SetPlayerTwoType;
    }

    private void SetPlayerOneType(ActorTypesEnum type)
    {
        playerOneType = type;
    }

    public bool IsPlayerOneAI()
    {
        return playerOneType == ActorTypesEnum.AI;
    }

    private void SetPlayerTwoType(ActorTypesEnum type)
    {
        playerTwoType = type;
    }

    public bool IsPlayerTwoAI()
    {
        return playerTwoType == ActorTypesEnum.AI;
    }

    public void StartPhaseTwo()
    {
        AudioManager.instance.StopMusic(); // The music will start plating when countdown timer ends

        phaseOneGameobject.SetActive(false);
        phaseTwoGameobject.SetActive(true);
    }
}
