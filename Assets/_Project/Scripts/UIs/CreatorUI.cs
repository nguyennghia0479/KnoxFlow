using TMPro;
using UnityEngine;

public class CreatorUI : MonoBehaviour
{
    [SerializeField] private string creator;
    [SerializeField] private string url;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        if (text)
            text.text = creator;
    }

    public void OpenURL() => Application.OpenURL(url);
}
