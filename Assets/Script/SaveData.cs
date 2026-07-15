using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


[System.Serializable]
public struct SaveData
{
    public static SaveData saveinstance;

    public HashSet<string> sceneNames;

    public string spotSceneName;
    public Vector2 lightPos;

    public int playerHealth;
    public int playerHealthMax;
    public int playerHeartShards;
    public float playerMana;
    public float playerExcessMana;
    public int playerManaShards;
    public float playerManaPenalty;
    public Vector2 playerPosition;
    public string lastScene;

    public Vector2 shadePos;
    public string scenewithShade;
    public Quaternion shadeRotation;
    public PlayerController.Abilities playerAbilities;

    public void Instantiate()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.light.dat"))
        {
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.light.dat"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.player.dat"))
        {
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.dat"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.shade.dat"))
        {
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.shade.dat"));
        }
        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }

    public void SavedLightSpot()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.light.dat")))
        {
            writer.Write(spotSceneName);
            writer.Write(lightPos.x);
            writer.Write(lightPos.y);
        }
    }
    public void LoadLightSpot()
    {
        if (File.Exists(Application.persistentDataPath + "/save.light.dat"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.light.dat")))
            {
                spotSceneName = reader.ReadString();
                lightPos.x = reader.ReadSingle();
                lightPos.y = reader.ReadSingle();
            }
        }
    }
    public void SavePlayerData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.dat")))
        {
            playerHealth = PlayerController.Instance.Health;
            writer.Write(playerHealth);
            playerHealthMax = PlayerController.Instance.maxHealth;
            writer.Write(playerHealthMax);
            playerHeartShards = PlayerController.Instance.heartShards;
            writer.Write(playerHeartShards);
            playerMana = PlayerController.Instance.Mana;
            writer.Write(playerMana);
            playerManaPenalty = PlayerController.Instance.manaPenalty;
            writer.Write(playerManaPenalty);
            playerExcessMana = PlayerController.Instance.excessMana;
            writer.Write(PlayerController.Instance.excessMana);
            playerManaShards = PlayerController.Instance.manaShards;
            writer.Write(PlayerController.Instance.manaShards);
            playerAbilities = PlayerController.Instance.abilities;
            writer.Write((int)playerAbilities);

            playerPosition = PlayerController.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);
            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);
        }
    }
    public void LoadPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.player.dat"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.dat")))
            {
                playerHealth = reader.ReadInt32();
                playerHealthMax = reader.ReadInt32();
                playerHeartShards = reader.ReadInt32();
                playerMana = reader.ReadSingle();
                playerManaPenalty = reader.ReadSingle();
                playerExcessMana = reader.ReadSingle();
                playerManaShards = reader.ReadInt32();
                playerAbilities = (PlayerController.Abilities)reader.ReadInt32();
                playerPosition.x = reader.ReadSingle();
                playerPosition.y = reader.ReadSingle();
                lastScene = reader.ReadString();

                SceneManager.LoadScene(lastScene);
                PlayerController.Instance.transform.position = playerPosition;
                PlayerController.Instance.Health = playerHealth;
                PlayerController.Instance.maxHealth = playerHealthMax;
                PlayerController.Instance.heartShards = playerHeartShards;
                PlayerController.Instance.Mana = playerMana;
                PlayerController.Instance.manaPenalty = playerManaPenalty;
                PlayerController.Instance.excessMana = playerExcessMana;
                PlayerController.Instance.manaShards = playerManaShards;
                PlayerController.Instance.abilities = playerAbilities;
            }
        }
        else
        {
            Debug.Log("No save file found for player data.");
            PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
            PlayerController.Instance.Mana = 0.5f;
            PlayerController.Instance.heartShards = 0;
            PlayerController.Instance.manaPenalty = 0;
            PlayerController.Instance.excessMana = 0;
            PlayerController.Instance.manaShards = 0;
            PlayerController.Instance.abilities = 0;
        }
    }
    public void SaveShadeData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.shade.dat")))
        {
            scenewithShade = SceneManager.GetActiveScene().name;
            shadePos = Shade.Instance.transform.position;
            shadeRotation = Shade.Instance.transform.rotation;

            writer.Write(scenewithShade);
            writer.Write(shadePos.x);
            writer.Write(shadePos.y);
            writer.Write(shadeRotation.x);
            writer.Write(shadeRotation.y);
            writer.Write(shadeRotation.z);
            writer.Write(shadeRotation.w);
        }
    }
    public void LoadShadeData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.shade.dat"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.shade.dat")))
            {
                scenewithShade = reader.ReadString();
                shadePos.x = reader.ReadSingle();
                shadePos.y = reader.ReadSingle();
                float shadeRotationx = reader.ReadSingle();
                float shadeRotationy = reader.ReadSingle();
                float shadeRotationz = reader.ReadSingle();
                float shadeRotationw = reader.ReadSingle();
                shadeRotation = new Quaternion(shadeRotationx, shadeRotationy, shadeRotationz, shadeRotationw);
            }
        }
        else
        {
            Debug.Log("No save file found for shade data.");
        }
    }
}
