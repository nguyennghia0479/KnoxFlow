using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompleteUI : MonoBehaviour
{
    [Header("Text Elements")]
    [SerializeField] private TMP_Text completeHeader;
    [SerializeField] private TMP_Text perfectHeader;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text nextLevelBtnText;

    [Header("Button Elements")]
    [SerializeField] private Button nextLevelBtn;
    [SerializeField] private Button retryBtn;

    private void OnEnable()
    {
        nextLevelBtn.onClick.AddListener(OnNextLevelButtonClicked);
        retryBtn.onClick.AddListener(OnRetryButtonClicked);
    }

    private void OnDisable()
    {
        nextLevelBtn.onClick.RemoveListener(OnNextLevelButtonClicked);
        retryBtn.onClick.RemoveListener(OnRetryButtonClicked);
    }

    public void SetupCompleteUI(bool isPerfect, int moves)
    {
        UpdateHeader(isPerfect);
        UpdateMessageText(moves);
        UpdateNextLevelButtonText();
    }

    private void UpdateHeader(bool isPerfect)
    {
        completeHeader.gameObject.SetActive(!isPerfect);
        perfectHeader.gameObject.SetActive(isPerfect);
    }

    private void UpdateMessageText(int moves)
    {
        messageText.text = $"You have completed level in {moves} moves";
    }

    private void UpdateNextLevelButtonText()
    {
        string nextLevelText = "Next Level";
        string nextStageText = "Select Pack";

        if (LevelManager.Instance.CanLoadNextStage)
        {
            LevelStageSO nextLevelStage = LevelManager.Instance.GetNextLevelStageSO();
            nextStageText = "Play " + nextLevelStage.StageName;
        }

        string text = LevelManager.Instance.IsLastLevel ? nextStageText : nextLevelText;
        nextLevelBtnText.text = text;
    }

    private void OnNextLevelButtonClicked()
    {
        if (LevelManager.Instance.CanLoadNextStage)
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
