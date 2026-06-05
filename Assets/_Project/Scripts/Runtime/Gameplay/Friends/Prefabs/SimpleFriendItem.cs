using Game.Core.Sounds;
using Game.Runtime.Core.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay
{
	public class SimpleFriendItem : MonoBehaviour
	{
        [SerializeField] private Image icon;
        [SerializeField] private Image onlineImage;
        
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI eloText;
        [SerializeField] private TextMeshProUGUI onlineText;

        [SerializeField] private Button challenge;

        public event Action<string> OnClicked;

        public string UserId { get; private set; }

        public void Init(FriendData friendData, string userId, IconStatManager iconManager)
        {
            icon.sprite = iconManager.GetIconStatById(friendData.icon).icon;
            UserId = userId;
            nameText.text = $"{friendData.name}";
            eloText.text = $"{friendData.elo}";
            challenge.onClick.AddListener(() => OnClicked?.Invoke(UserId));
            challenge.onClick.AddListener(() => SoundManager.Instance.Click());

            if (friendData.online)
            {
                onlineImage.color = Color.green;
                onlineText.text = "Online";
            }
            else
            {
                onlineImage.color = Color.red;
                onlineText.text = "Offline";
            }
        }
    }

}