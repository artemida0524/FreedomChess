using System;
using UnityEngine;
using UnityEngine.UI;

public class ExitBattlePanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [SerializeField] private Button cancelButton;

    public event Action OnYesClicked;

    public void Init()
    {
        yesButton.onClick.AddListener(() => OnYesClicked?.Invoke());
        noButton.onClick.AddListener(() => SetActive(false));
        cancelButton.onClick.AddListener(() => SetActive(false));
    }

    public void SetActive(bool active)
    {
        panel.SetActive(active);
    }

}
