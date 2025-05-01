using UnityEngine;

public class AboutModalManager : MonoBehaviour
{
    public GameObject aboutModal;

    void Start()
    {
        Debug.Log("AboutModalManager initialized.");
        aboutModal.SetActive(false);
    }
    public void ShowAboutModal()
    {
        aboutModal.SetActive(true);
    }

    public void HideAboutModal()
    {
        aboutModal.SetActive(false);
    }
}
