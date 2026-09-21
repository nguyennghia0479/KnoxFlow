using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class CompleteUI : MonoBehaviour
{
    [Header("Text Elements")]
    [SerializeField] private TMP_Text completeHeader;
    [SerializeField] private TMP_Text perfectHeader;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] private TMP_Text nextStageText;
    [SerializeField] private TMP_Text completedPackText;

    [Header("Button Elements")]
    [SerializeField] private Button nextLevelBtn;
    [SerializeField] private Button retryBtn;

    [Header("Localization Elements")]
    [SerializeField] private string tableReference;
    [SerializeField] private string messageKey;
    [SerializeField] private string nextStageKey;

    [Space]
    [SerializeField] private InterstitialAdManager adManager;

    private LocalizedString messageLocalized;
    private LocalizedString nextStageLocalized;
    private int moves;
    private string nextStageName;
    private bool isMobilePlatform;

    private void Awake()
    {
        messageLocalized = new(tableReference, messageKey);
        nextStageLocalized = new(tableReference, nextStageKey);
        isMobilePlatform = false;
#if UNITY_ANDROID
        isMobilePlatform = true;
#endif
    }

    private void OnEnable()
    {
        nextLevelBtn.onClick.AddListener(OnNextLevelButtonClicked);
        retryBtn.onClick.AddListener(OnRetryButtonClicked);

        messageLocalized.StringChanged += UpdateMessageText;
        nextStageLocalized.StringChanged += UpdatePlayNextStage;
    }

    private void OnDisable()
    {
        nextLevelBtn.onClick.RemoveListener(OnNextLevelButtonClicked);
        retryBtn.onClick.RemoveListener(OnRetryButtonClicked);

        messageLocalized.StringChanged -= UpdateMessageText;
        nextStageLocalized.StringChanged -= UpdatePlayNextStage;
    }

    public void SetupCompleteUI(bool isPerfect, int moves)
    {
        UpdateHeader(isPerfect);
        UpdateNextLevelButtonText();

        this.moves = moves;
        messageLocalized.RefreshString();
    }

    private void UpdateHeader(bool isPerfect)
    {
        completeHeader.gameObject.SetActive(!isPerfect);
        perfectHeader.gameObject.SetActive(isPerfect);
    }

    private void UpdateNextLevelButtonText()
    {
        bool isLastLevelofStage = LevelManager.Instance.IsLastLevel;
        bool canLoadNextStage = LevelManager.Instance.CanLoadNextStage;
        if (canLoadNextStage)
        {
            LevelStageSO nextLevelStage = LevelManager.Instance.GetNextLevelStageSO();
            nextStageName = nextLevelStage.StageName;
            nextStageLocalized.RefreshString();
        }

        nextLevelText.gameObject.SetActive(!isLastLevelofStage);
        nextStageText.gameObject.SetActive(isLastLevelofStage && canLoadNextStage);
        completedPackText.gameObject.SetActive(isLastLevelofStage && !canLoadNextStage);
    }

    private void UpdatePlayNextStage(string value) => nextStageText.text = string.Format(value, nextStageName);
    private void UpdateMessageText(string value) => messageText.text = string.Format(value, moves);

    private void OnNextLevelButtonClicked()
    {
        if (isMobilePlatform && adManager != null)
            adManager.ShowAd(ProceedAfterAd);
        else
            ProceedAfterAd();
    }

    private void ProceedAfterAd()
    {
        if (LevelManager.Instance.CanLoadNextStage || !LevelManager.Instance.IsLastLevel)
            UIEvents.RaiseNextLevelButtonClicked();
        else
            UIEvents.RaisePlayButtonClicked();

        gameObject.SetActive(false);
    }

    private void OnRetryButtonClicked()
    {
        UIEvents.RaiseRetryButtonClicked();
        gameObject.SetActive(false);
    }
}
