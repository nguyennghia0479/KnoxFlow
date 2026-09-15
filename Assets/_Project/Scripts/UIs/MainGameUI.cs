using TMPro;
using UnityEngine;
using UnityEngine.Localization;
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

    [Header("Localization Elements")]
    [SerializeField] protected string tableReference;
    [SerializeField] private string levelNameKey;
    [SerializeField] private string movesKey;
    [SerializeField] private string bestKey;

    private GridManager gridManager;
    private LevelSO levelSO;
    private int knoxsToConnectAmount;
    private int connectedKnoxs;
    private int moves;
    private int best;
    private bool undoState;
    private LocalizedString levelNameLocalized;
    private LocalizedString movesLocalized;
    private LocalizedString bestLocalized;

    private void Awake()
    {
        gridManager = GetComponentInChildren<GridManager>();
        levelNameLocalized = new(tableReference, levelNameKey);
        movesLocalized = new(tableReference, movesKey);
        bestLocalized = new(tableReference, bestKey);
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

        levelNameLocalized.StringChanged += UpdateLevelNameText;
        movesLocalized.StringChanged += UpdateMovesText;
        bestLocalized.StringChanged += UpdateBestText;
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

        levelNameLocalized.StringChanged -= UpdateLevelNameText;
        movesLocalized.StringChanged -= UpdateMovesText;
        bestLocalized.StringChanged -= UpdateBestText;
    }

    public void SetupMainGameUI(LevelSO levelSO, LevelStageSO levelStageSO)
    {
        this.levelSO = levelSO;
        LevelDTO levelDTO = LevelManager.Instance.GetLevelDTO(levelSO.LevelId);
        best = levelDTO.best;
        knoxsToConnectAmount = levelSO.Knoxs.Length / 2;
        ClearMainGameUI();
        LevelManager.Instance.SetupLevelLoaded(levelSO, levelStageSO);

        levelNameLocalized.RefreshString();
        bestLocalized.RefreshString();
    }

    private void ClearMainGameUI()
    {
        connectedKnoxs = 0;
        moves = 0;
        undoState = false;
        movesLocalized.RefreshString();
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
        this.moves = moves;
        movesLocalized.RefreshString();
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

        movesLocalized.RefreshString();

        UpdateKnoxsText(connectedKnoxs);
        UpdateUndoButton(undoState);
    }

    private void UpdateLevelNameText(string value)
    {
        if (levelSO != null && levelName != null)
            levelName.text = string.Format(value, levelSO.LevelName);
    }

    private void UpdateKnoxsText(int knoxsConnected)
    {
        knoxsText.text = $"Knoxs: {knoxsConnected}/{knoxsToConnectAmount}";
    }

    private void UpdateMovesText(string value)
    {
        if (movesText != null)
            movesText.text = string.Format(value, moves);
    }

    private void UpdateBestText(string value)
    {
        if (bestText != null)
            bestText.text = string.Format (value, best);
    }

    private void UpdateUndoButton(bool undoState)
    {
        undoBtn.interactable = undoState;
    }
}
