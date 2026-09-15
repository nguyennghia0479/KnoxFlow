using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text level;
    [SerializeField] private Image completeIcon;
    [SerializeField] private Image perfectIcon;
    [SerializeField] private Button levelBtn;

    private LevelSO levelSO;
    private LevelStageSO levelStageSO;

    private void OnEnable()
    {
        if (levelBtn)
            levelBtn.onClick.AddListener(OnLevelButtonClicked);
    }

    private void OnDisable()
    {
        if (levelBtn)
            levelBtn.onClick.RemoveListener(OnLevelButtonClicked);
    }

    public void SetupLevelUI(LevelSO levelSO, LevelStageSO levelStageSO)
    {
        this.levelSO = levelSO;
        this.levelStageSO = levelStageSO;
        level.text = levelSO.LevelNumber.ToString();

        LevelDTO levelDTO = LevelManager.Instance.GetLevelDTO(levelSO.LevelId);
        completeIcon.gameObject.SetActive(!levelDTO.isPerfect && levelDTO.isCompleted);
        perfectIcon.gameObject.SetActive(levelDTO.isPerfect && levelDTO.isCompleted);
    }

    private void OnLevelButtonClicked()
    {
        UIEvents.RaiseLevelSelected(levelSO, levelStageSO);
    }
}
