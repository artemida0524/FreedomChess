using Game.Core.Sounds;
using Game.Runtime.Core.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay
{
	public class AddFriendItem : MonoBehaviour
	{
		[SerializeField] private Image icon;
		[SerializeField] private TextMeshProUGUI nameText;
		[SerializeField] private TextMeshProUGUI eloText;
		[SerializeField] private Button button;


        public event Action<string> OnClicked;

        public string UserId { get; private set; }

        public void Init(FriendData friendData, string userId, IconStatManager iconManager)
        {
            icon.sprite = iconManager.GetIconStatById(friendData.icon).icon;
            UserId = userId;
            nameText.text = $"{friendData.name}";
            eloText.text = $"{friendData.elo}";
            button.onClick.AddListener(() => OnClicked?.Invoke(UserId));
            button.onClick.AddListener(() => SoundManager.Instance.Click());
        }
    }
}