using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [System.Serializable]
    private class PlayerSkinModel
    {
        public string id;
        public GameObject playerSkinPrefab;
    }

    [SerializeField]
    private List<PlayerSkinModel> playerSkins = new List<PlayerSkinModel>();

    public float positionX = -7.2f;
    public float scale = 1.5f;

    public RectTransform controlDown;
    public RectTransform controlUp;

    private GameObject playerGameObject;

    // Start is called before the first frame update
    private void Awake()
    {
        SettingsManager.instance.OnPlayerSkinChange += LoadCurrentSkin;
        LoadCurrentSkin();
    }

    private void OnDisable()
    {
        SettingsManager.instance.OnPlayerSkinChange -= LoadCurrentSkin;
    }

    private void LoadCurrentSkin()
    {
        PlayerSkinModel currentSkin = playerSkins.Find(item => item.id == SettingsManager.GetPlayerSkin());
        GameObject currentskinPrefab = currentSkin.playerSkinPrefab;

        if(playerGameObject != null)
        {
            Destroy(playerGameObject);
        }
        playerGameObject = Instantiate(currentSkin.playerSkinPrefab, transform);

        // Set X position of the player
        Vector3 tempPosition = playerGameObject.transform.localPosition;
        tempPosition.x = positionX;
        playerGameObject.transform.localPosition = tempPosition;

        // Set X and Y scale
        Vector3 tempScale = playerGameObject.transform.localScale;
        tempScale.x = scale;
        tempScale.y = scale;
        playerGameObject.transform.localScale = tempScale;

        Player player = playerGameObject.GetComponent<Player>();
        player.controlUp = controlUp;
        player.controlDown = controlDown;
    }
}
