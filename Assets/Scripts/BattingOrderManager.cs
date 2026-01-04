using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Singleton manager that holds the authoritative player data and handles business logic
/// for reordering, save/load functionality for the batting order system.
/// </summary>
public class BattingOrderManager : MonoBehaviour
{
    public static BattingOrderManager Instance { get; private set; }

    [Header("Avatar Sprites (Exactly 6 sprites)")]
    [Tooltip("Assign exactly 6 avatar sprites for deterministic assignment")]
    public Sprite[] avatarSprites = new Sprite[6];

    private const string SAVE_KEY = "BattingOrder_v1";
    private const int PLAYER_COUNT = 11;

    private List<PlayerData> allPlayers;
    private List<string> currentOrder;
    private List<string> savedOrder;

    private void Awake()
    {
        if (Instance == null || Instance.gameObject == null)
        {
            Instance = this;
            InitializePlayers();
            LoadOrder();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void InitializePlayers()
    {
        allPlayers = new List<PlayerData>();
        
        for (int i = 1; i <= PLAYER_COUNT; i++)
        {
            allPlayers.Add(new PlayerData
            {
                ID = $"P{i:D2}",
                DisplayName = $"Player {i}"
            });
        }
    }

    /// <summary>
    /// Randomize the player order using Fisher-Yates shuffle
    /// </summary>
    public void AutoPopulate()
    {
        currentOrder = new List<string>();
        for (int i = 1; i <= PLAYER_COUNT; i++)
        {
            currentOrder.Add($"P{i:D2}");
        }
        
        // Fisher-Yates shuffle
        System.Random rng = new System.Random();
        int n = currentOrder.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (currentOrder[k], currentOrder[n]) = (currentOrder[n], currentOrder[k]);
        }
    }

    /// <summary>
    /// Move player at index up (swap with previous)
    /// </summary>
    public void MovePlayerUp(int index)
    {
        if (index <= 0 || index >= currentOrder.Count)
            return;

        (currentOrder[index], currentOrder[index - 1]) = (currentOrder[index - 1], currentOrder[index]);
    }

    /// <summary>
    /// Move player at index down (swap with next)
    /// </summary>
    public void MovePlayerDown(int index)
    {
        if (index < 0 || index >= currentOrder.Count - 1)
            return;

        (currentOrder[index], currentOrder[index + 1]) = (currentOrder[index + 1], currentOrder[index]);
    }

    /// <summary>
    /// Save current order to PlayerPrefs
    /// </summary>
    public void SaveOrder()
    {
        string json = JsonUtility.ToJson(new PlayerOrderData { playerIDs = currentOrder.ToArray() });
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        savedOrder = new List<string>(currentOrder);
    }

    /// <summary>
    /// Load order from PlayerPrefs, or use default if invalid/missing
    /// </summary>
    public void LoadOrder()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            try
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                PlayerOrderData data = JsonUtility.FromJson<PlayerOrderData>(json);
                
                if (ValidateOrder(data.playerIDs))
                {
                    currentOrder = data.playerIDs.ToList();
                    savedOrder = new List<string>(currentOrder);
                    return;
                }
            }
            catch (System.Exception)
            {
                // Fall through to default
            }
        }
        
        // Use default order
        currentOrder = GetDefaultOrder();
        savedOrder = new List<string>(currentOrder);
    }

    /// <summary>
    /// Discard changes and revert to default order, clears saved data
    /// </summary>
    public void DiscardChanges()
    {
        currentOrder = GetDefaultOrder();
        savedOrder = new List<string>(currentOrder);
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
    }

    private List<string> GetDefaultOrder()
    {
        var order = new List<string>();
        for (int i = 1; i <= PLAYER_COUNT; i++)
        {
            order.Add($"P{i:D2}");
        }
        return order;
    }

    private bool ValidateOrder(string[] playerIDs)
    {
        if (playerIDs == null || playerIDs.Length != PLAYER_COUNT)
            return false;

        var knownIDs = new HashSet<string>(allPlayers.Select(p => p.ID));
        var providedIDs = new HashSet<string>(playerIDs);
        return knownIDs.SetEquals(providedIDs);
    }

    public PlayerData GetPlayerByID(string playerID)
    {
        return allPlayers.FirstOrDefault(p => p.ID == playerID);
    }

    public List<string> GetCurrentOrder()
    {
        return new List<string>(currentOrder);
    }

    /// <summary>
    /// Get avatar sprite for a player by their ID
    /// </summary>
    public Sprite GetAvatarForPlayerID(string playerID)
    {
        if (avatarSprites == null || avatarSprites.Length == 0)
            return null;

        if (playerID.StartsWith("P") && int.TryParse(playerID.Substring(1), out int playerNumber))
        {
            int avatarIndex = (playerNumber - 1) % avatarSprites.Length;
            return avatarSprites[avatarIndex];
        }

        return null;
    }
}

[System.Serializable]
public class PlayerData
{
    public string ID;
    public string DisplayName;
}

[System.Serializable]
public class PlayerOrderData
{
    public string[] playerIDs;
}
