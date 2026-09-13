using UnityEngine;

public class SelectPackUI : MonoBehaviour
{
    private LevelPackSO[] levelPackSOs;
    private LevelPackUI[] levelPackUIs;

    private void Awake()
    {
        levelPackUIs = GetComponentsInChildren<LevelPackUI>();
    }

    private void OnEnable()
    {
        levelPackSOs = LevelManager.Instance.LevelPackSOs;
        if (levelPackSOs == null || levelPackSOs.Length <= 0)
            return;

        SetupSelectPackUI();
    }

    private void SetupSelectPackUI()
    {
       foreach (var levelPack in levelPackUIs)
            levelPack.gameObject.SetActive(false);

       for (int i = 0; i < levelPackUIs.Length; i++)
        {
            if (i >= levelPackSOs.Length)
                break;

            LevelPackUI levelPackUI = levelPackUIs[i];
            levelPackUI.SetupLevelPackUI(levelPackSOs[i]);
            levelPackUI.gameObject.SetActive(true);
        }
    }
}
