using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class VersusController : MonoBehaviour
{
    public delegate void OnPlayerTypeChangedDelegate(VersusGameManager.ActorTypesEnum actorType);
    public event OnPlayerTypeChangedDelegate OnPlayerTypeChanged;

    private VersusGameManager.ActorTypesEnum actorType = VersusGameManager.ActorTypesEnum.AI;

    private void Start()
    {
        SetActorType(VersusGameManager.ActorTypesEnum.AI);
    }

    public void OnClick()
    {
        // Toogle actor type
        if (actorType == VersusGameManager.ActorTypesEnum.AI)
        {
            SetActorType(VersusGameManager.ActorTypesEnum.Player);
        }
        else
        {
            SetActorType(VersusGameManager.ActorTypesEnum.AI);
        }


    }

    private void SetActorType(VersusGameManager.ActorTypesEnum type)
    {
        actorType = type;

        if (OnPlayerTypeChanged != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnPlayerTypeChanged(actorType);
        }
    }
}
