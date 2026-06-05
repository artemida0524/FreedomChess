using Game.Runtime.Core.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static UnityEngine.Rendering.DebugUI;

namespace Game.Runtime.Gameplay
{
	public class EnemyStatsView : MonoBehaviour
	{
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI eloText;
        [SerializeField] private GameObject panel;
        private IconStatManager _iconStatManager;
        private FriendsProvider _friendsProvider;
        


        [Inject]
        private void Construct(IconStatManager iconStatManager, FriendsProvider friendsProvider)
        {
            _iconStatManager = iconStatManager;
            _friendsProvider = friendsProvider;
        }


        public void View(string uid)
        {
            FriendData friendData = _friendsProvider.Friends[uid];

            panel.gameObject.SetActive(true);
            nameText.text = $"{friendData.name}";
            eloText.text = $"{friendData.elo}";
            image.sprite = _iconStatManager.GetIconStatById(friendData.icon).icon;
        }


        public void Hide()
        {
            panel.gameObject.SetActive(false);
        }

    }

}