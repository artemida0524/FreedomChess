using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.AppStart.Views
{

    public class GoBacktoExistentPanelView : MonoBehaviour, IGoBacktoExistentPanelView
    {
        [SerializeField] private TextMeshProUGUI email;
        [SerializeField] private Button goBackButton;

        public event Action OnGoBackToExistentClicked;

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public void Init(string email)
        {
            goBackButton.onClick.AddListener(() => OnGoBackToExistentClicked?.Invoke());
            this.email.text = email;
        }

    }

}