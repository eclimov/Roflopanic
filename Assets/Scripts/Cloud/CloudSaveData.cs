using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveData
{
    [System.Serializable]
    public class CloudSaveData
    {
        public int highscore = 0;
        public int totalScore = 0;
        public int experience = 0;
        public int coinChance = 0;
        public string purchasedAbilitiesList = "";
        public string purchasedSkinsList = "";
        public string purchasedBackgroundsList = "";

        public int bossFightsWonCountClownich = 0;
    }
}
