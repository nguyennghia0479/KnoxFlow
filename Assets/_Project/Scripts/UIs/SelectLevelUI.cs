using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelUI : MonoBehaviour
{
    [Header("Text Elements")]
    [SerializeField] private TMP_Text packName;
    [SerializeField] private TMP_Text stage;

    [Header("Button Elements")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    private LevelStageSO[] levelStageSOs;
    private LevelStageSO currentStageSO;
    private int currentStageIdx;
    private LevelUI[] levelUIs;

    private void Awake()
    {
        levelUIs = GetComponentsInChildren<LevelUI>();
    }

    private void OnEnable()
    {
        if (previousButton)
            previousButton.onClick.AddListener(OnPreviousButtonClicked);

        if (nextButton)
            nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    private void OnDisable()
    {
        if (previousButton)
            previousButton.onClick.RemoveListener(OnPreviousButtonClicked);

        if (nextButton)
            nextButton.onClick.RemoveListener(OnNextButtonClicked);
    }

    public void SetupSelectLevelUI(LevelPackSO levelPackSO)
    {
        currentStageIdx = LevelManager.Instance.LevelStageIndex;
        packName.text = levelPackSO.PackName;
        levelStageSOs = levelPackSO.LevelStageSOs;
        UpdateSelectlevelUI();
    }

    private void OnPreviousButtonClicked()
    {
        currentStageIdx--;
        if (currentStageIdx < 0)
            currentStageIdx = levelStageSOs.Length - 1;

        UpdateSelectlevelUI();
    }

    private void OnNextButtonClicked()
    {
        currentStageIdx++;
        if (currentStageIdx >= levelStageSOs.Length)
            currentStageIdx = 0;

        UpdateSelectlevelUI();
    }

    private void UpdateSelectlevelUI()
    {
        currentStageSO = levelStageSOs[currentStageIdx];
        stage.text = currentStageSO.StageName;
        UpdateLevelUIs();
    }

    private void UpdateLevelUIs()
    {
        foreach (var levelUI in levelUIs)
            levelUI.gameObject.SetActive(false);

        for (int i = 0; i < levelUIs.Length; i++)
        {
            if (i >= currentStageSO.LevelSOs.Length)
                break;

            LevelUI levelUI = levelUIs[i];
            levelUI.SetupLevelUI(currentStageSO.LevelSOs[i], currentStageSO);
            levelUI.gameObject.SetActive(true);
        }
    }
}
