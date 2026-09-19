using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject webGuide;
    [SerializeField] private GameObject mobileGuide;

    private void Awake()
    {
        webGuide.SetActive(true);
        mobileGuide.SetActive(false);

#if UNITY_ANDROID
        webGuide.SetActive(false);
        mobileGuide.SetActive(true);
#endif
    }
}
