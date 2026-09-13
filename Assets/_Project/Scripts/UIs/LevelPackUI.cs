using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPackUI : MonoBehaviour
{
    [SerializeField] private Button levelPackBtn;
    [SerializeField] private TMP_Text packName;
    [SerializeField] private TMP_Text levelAmount;

    private LevelPackSO levelPackSO;

    private void OnEnable()
    {
        if (levelPackBtn)
            levelPackBtn.onClick.AddListener(OnLevelPackButtonClicked);
    }

    private void OnDisable()
    {
        if (levelPackBtn)
            levelPackBtn.onClick.RemoveListener(OnLevelPackButtonClicked);
    }

    public void SetupLevelPackUI(LevelPackSO levelPackSO)
    {
        this.levelPackSO = levelPackSO;
        packName.text = levelPackSO.PackName;
        levelAmount.text = $"0/{levelPackSO.GetLevelAmountInPack()}";
    }

    private void OnLevelPackButtonClicked()
    {
        UIEvents.RaiseLevelPackSelected(levelPackSO);
    }
}
