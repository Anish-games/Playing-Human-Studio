using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI controller that bridges BattingOrderManager data with visual prefab instances.
/// </summary>
public class BattingOrderUIController : MonoBehaviour
{
    [Header("Player Prefab")]
    [SerializeField] private GameObject playerRowPrefab;

    [Header("Panel Slots (Drag 11 Panels Here)")]
    [SerializeField] private Transform[] panelSlots = new Transform[11];

    [Header("Animation Settings")]
    [SerializeField] private float swapAnimationDuration = 0.8f;
    [SerializeField] private AnimationCurve swapAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Buttons")]
    [SerializeField] private Button autoPopulateButton;

    private PlayerRowController[] panelContents = new PlayerRowController[11];
    private Dictionary<string, int> playerToPanelIndex = new Dictionary<string, int>();
    private bool isAnimating = false;
    private BattingOrderManager manager;

    private void Start()
    {
        manager = BattingOrderManager.Instance;
        if (manager == null) return;

        if (panelSlots == null || panelSlots.Length != 11) return;

        for (int i = 0; i < panelSlots.Length; i++)
        {
            if (panelSlots[i] == null) return;
        }

        if (autoPopulateButton != null)
            autoPopulateButton.onClick.AddListener(OnAutoPopulateClicked);

        InitializePanels();
    }

    private void InitializePanels()
    {
        if (manager == null) return;

        List<string> currentOrder = manager.GetCurrentOrder();

        for (int i = 0; i < currentOrder.Count && i < panelSlots.Length; i++)
        {
            string playerID = currentOrder[i];
            PlayerData player = manager.GetPlayerByID(playerID);
            if (player == null) continue;

            GameObject rowObj = Instantiate(playerRowPrefab, panelSlots[i]);
            PlayerRowController rowController = rowObj.GetComponent<PlayerRowController>();

            if (rowController == null)
            {
                Destroy(rowObj);
                continue;
            }

            RectTransform rowRect = rowObj.GetComponent<RectTransform>();
            rowRect.localScale = Vector3.one;
            rowRect.anchoredPosition = Vector2.zero;
            rowRect.anchorMin = Vector2.zero;
            rowRect.anchorMax = Vector2.one;
            rowRect.offsetMin = Vector2.zero;
            rowRect.offsetMax = Vector2.zero;

            panelContents[i] = rowController;
            playerToPanelIndex[playerID] = i;

            Sprite avatar = manager.GetAvatarForPlayerID(playerID);
            rowController.SetData(i, player, avatar, OnPlayerMoveUp, OnPlayerMoveDown);
            UpdateButtonStates(i);
        }
    }

    private void UpdateButtonStates(int panelIndex)
    {
        if (panelContents[panelIndex] == null) return;
        bool canMoveUp = panelIndex > 0;
        bool canMoveDown = panelIndex < panelContents.Length - 1;
        panelContents[panelIndex].SetButtonStates(canMoveUp, canMoveDown);
    }

    private void UpdateAllButtonStates()
    {
        for (int i = 0; i < panelContents.Length; i++)
            UpdateButtonStates(i);
    }

    private void SwapPanelContents(int panelA, int panelB)
    {
        if (isAnimating) return;
        if (panelA < 0 || panelA >= panelSlots.Length || panelB < 0 || panelB >= panelSlots.Length) return;

        PlayerRowController rowA = panelContents[panelA];
        PlayerRowController rowB = panelContents[panelB];
        if (rowA == null || rowB == null) return;

        StartCoroutine(AnimatedSwapCoroutine(panelA, panelB, rowA, rowB));
    }

    private IEnumerator AnimatedSwapCoroutine(int panelA, int panelB, PlayerRowController rowA, PlayerRowController rowB)
    {
        isAnimating = true;

        string playerIdA = rowA.GetCurrentPlayerID();
        string playerIdB = rowB.GetCurrentPlayerID();

        Vector3 startPosA = rowA.transform.position;
        Vector3 startPosB = rowB.transform.position;
        float yDistanceWorld = startPosB.y - startPosA.y;
        
        Vector3 startLocalPosA = rowA.transform.localPosition;
        Vector3 startLocalPosB = rowB.transform.localPosition;

        float elapsed = 0f;
        while (elapsed < swapAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / swapAnimationDuration);
            float easedT = swapAnimationCurve.Evaluate(t);

            float offsetA = yDistanceWorld * easedT;
            float offsetB = -yDistanceWorld * easedT;
            
            rowA.transform.localPosition = new Vector3(startLocalPosA.x, startLocalPosA.y + offsetA, startLocalPosA.z);
            rowB.transform.localPosition = new Vector3(startLocalPosB.x, startLocalPosB.y + offsetB, startLocalPosB.z);

            yield return null;
        }

        rowA.transform.localPosition = new Vector3(startLocalPosA.x, startLocalPosA.y + yDistanceWorld, startLocalPosA.z);
        rowB.transform.localPosition = new Vector3(startLocalPosB.x, startLocalPosB.y - yDistanceWorld, startLocalPosB.z);

        rowA.transform.SetParent(panelSlots[panelB], false);
        rowB.transform.SetParent(panelSlots[panelA], false);
        
        ResetPrefabPosition(rowA);
        ResetPrefabPosition(rowB);

        panelContents[panelA] = rowB;
        panelContents[panelB] = rowA;

        playerToPanelIndex[playerIdA] = panelB;
        playerToPanelIndex[playerIdB] = panelA;

        rowA.UpdatePositionOnly(panelB, OnPlayerMoveUp, OnPlayerMoveDown);
        rowB.UpdatePositionOnly(panelA, OnPlayerMoveUp, OnPlayerMoveDown);

        UpdateAllButtonStates();
        isAnimating = false;
    }

    private void ResetPrefabPosition(PlayerRowController row)
    {
        RectTransform rect = row.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.anchoredPosition = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    public void RefreshUI()
    {
        for (int i = 0; i < panelContents.Length; i++)
        {
            if (panelContents[i] != null)
            {
                Destroy(panelContents[i].gameObject);
                panelContents[i] = null;
            }
        }
        playerToPanelIndex.Clear();
        InitializePanels();
    }

    private void OnAutoPopulateClicked()
    {
        manager.AutoPopulate();
        RefreshUI();
    }

    private void OnPlayerMoveUp(int panelIndex)
    {
        if (panelIndex <= 0) return;
        manager.MovePlayerUp(panelIndex);
        SwapPanelContents(panelIndex, panelIndex - 1);
    }

    private void OnPlayerMoveDown(int panelIndex)
    {
        if (panelIndex >= panelContents.Length - 1) return;
        manager.MovePlayerDown(panelIndex);
        SwapPanelContents(panelIndex, panelIndex + 1);
    }

    private void OnDestroy()
    {
        if (autoPopulateButton != null)
            autoPopulateButton.onClick.RemoveListener(OnAutoPopulateClicked);
    }
}
