using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUI : MonoBehaviour
{
    [Header("Text Elements")]
    [SerializeField] private TMP_Text levelName;
    [SerializeField] private TMP_Text knoxsText;
    [SerializeField] private TMP_Text movesText;
    [SerializeField] private TMP_Text bestText;

    [Header("Button Elements")]
    [SerializeField] private Button backBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button undoBtn;
    [SerializeField] private Button clearLevelBtn;

    private GridManager gridManager;
    private int knoxsToConnect;
    private int connectedKnoxs;
    private int moves;
    private bool undoState;

    private void Awake()
    {
        gridManager = GetComponentInChildren<GridManager>();
    }

    private void OnEnable()
    {
        backBtn.onClick.AddListener(OnBackButtonClicked);
        settingsBtn.onClick.AddListener(OnSettingsButtonClicked);
        undoBtn.onClick.AddListener(OnUndoButtonClicked);
        clearLevelBtn.onClick.AddListener(OnClearLevelButtonClicked);

        gridManager.Moved += HandleMoved;
        gridManager.KnoxsConnected += HandleKnoxsConnected;
        gridManager.UndoStateChanged += HandleUndoStateChanged;
        UIEvents.OnRetryBtnClicked += HandleRetryButtonClicked;
    }

    private void OnDisable()
    {
        backBtn.onClick.RemoveListener(OnBackButtonClicked);
        settingsBtn.onClick.RemoveListener(OnSettingsButtonClicked);
        undoBtn.onClick.RemoveListener(OnUndoButtonClicked);
        clearLevelBtn.onClick.RemoveListener(OnClearLevelButtonClicked);

        gridManager.Moved -= HandleMoved;
        gridManager.KnoxsConnected -= HandleKnoxsConnected;
        gridManager.UndoStateChanged -= HandleUndoStateChanged;
        UIEvents.OnRetryBtnClicked -= HandleRetryButtonClicked;
    }

    public void SetupMainGameUI(LevelSO levelSO, LevelStageSO levelStageSO)
    {
        levelName.text = levelSO.LevelName;
        bestText.text = "Best: 0";
        knoxsToConnect = levelSO.Knoxs.Length / 2;
        ClearMainGameUI();
        LevelManager.Instance.SetupLevelLoaded(levelSO, levelStageSO);
    }

    private void ClearMainGameUI()
    {
        connectedKnoxs = 0;
        moves = 0;
        undoState = false;

        UpdateKnoxsText(connectedKnoxs);
        UpdateMovesText(moves);
        UpdateUndoButton(undoState);
    }

    private void OnBackButtonClicked()
    {
        LevelPackSO levelPackSO = LevelManager.Instance.LevelPackSO;
        UIEvents.RaiseLevelPackSelected(levelPackSO);
    }

    private void OnSettingsButtonClicked()
    {
        UIEvents.RaiseSettingsButtonClicked();
    }

    private void OnUndoButtonClicked()
    {
        UIEvents.RaiseUndoButtonClicked();
    }

    private void OnClearLevelButtonClicked()
    {
        UIEvents.RaiseClearLevelButtonClicked();
        ClearMainGameUI();
    }

    private void HandleKnoxsConnected(int connectedKnoxs)
    {
        UpdateKnoxsText(connectedKnoxs);
    }

    private void HandleMoved(int moves)
    {
        UpdateMovesText(moves);
    }

    private void HandleUndoStateChanged(bool undoState)
    {
        UpdateUndoButton(undoState);
    }

    private void HandleRetryButtonClicked()
    {
        moves = 0;
        connectedKnoxs = 0;
        undoState = false;

        UpdateKnoxsText(connectedKnoxs);
        UpdateMovesText(moves);
        UpdateUndoButton(undoState);
    }

    private void UpdateKnoxsText(int knoxsConnected)
    {
        knoxsText.text = $"Knoxs: {knoxsConnected}/{knoxsToConnect}";
    }

    private void UpdateMovesText(int moves)
    {
        movesText.text = $"Move: {moves}";
    }

    private void UpdateUndoButton(bool undoState)
    {
        undoBtn.interactable = undoState;
    }
}
