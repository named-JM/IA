using UnityEngine;

public class AboutModalManager : MonoBehaviour
{
    public GameObject aboutModal;

    public void ShowAboutModal()
    {
        aboutModal.SetActive(true);
    }

    public void HideAboutModal()
    {
        aboutModal.SetActive(false);
    }
}
