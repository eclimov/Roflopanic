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

    public AudioClip teleportSound;
    public GameObject teleportParticlePrefab;

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

    public void TeleportPlayerToPositionCenter()
    {
        AudioManager.instance.PlaySound(teleportSound);

        EmitTeleportParticles(playerGameObject.transform.position);

        MovePlayer(0f, 0f);

        EmitTeleportParticles(playerGameObject.transform.position);
    }

    public void TeleportPlayerToPositionDefault()
    {
        AudioManager.instance.PlaySound(teleportSound);

        EmitTeleportParticles(playerGameObject.transform.position);

        MovePlayer(positionX, 0f);

        EmitTeleportParticles(playerGameObject.transform.position);
    }

    private void EmitTeleportParticles(Vector3 position)
    {
        GameObject teleportPartclesGameObject = Instantiate(teleportParticlePrefab, GameObject.Find("Canvas UI").transform);
        teleportPartclesGameObject.transform.position = position;
    }

    private void MovePlayer(float x, float y)
    {
        Vector3 tempPosition = playerGameObject.transform.localPosition;
        tempPosition.x = x;
        tempPosition.y = y;
        playerGameObject.transform.localPosition = tempPosition;
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
        MovePlayer(positionX, 0f);

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
