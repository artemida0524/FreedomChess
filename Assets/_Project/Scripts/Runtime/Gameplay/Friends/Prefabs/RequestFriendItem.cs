using Game.Core.Sounds;
using Game.Runtime.Core.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay
{
    public class RequestFriendItem : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI eloText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button discardButton;

        public event Action<string> AcceptButtonClicked;
        public event Action<string> DiscardButtonClicked;

        public string UserId { get; private set; }

        public void Init(FriendData friendData, string userId, IconStatManager iconManager)
        {
            icon.sprite = iconManager.GetIconStatById(friendData.icon).icon;
            UserId = userId;
            nameText.text = $"{friendData.name}";
            eloText.text = $"{friendData.elo}";
            acceptButton.onClick.AddListener(() => AcceptButtonClicked?.Invoke(UserId));
            discardButton.onClick.AddListener(() => DiscardButtonClicked?.Invoke(UserId));
            acceptButton.onClick.AddListener(() => SoundManager.Instance.Click());
            discardButton.onClick.AddListener(() => SoundManager.Instance.Click());
        }
    }
}