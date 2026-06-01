using Game.Runtime.Core.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Runtime.Gameplay
{
	public class FriendButtonView : MonoBehaviour
	{
		[SerializeField] private GameObject friendRequest;
		[SerializeField] private TextMeshProUGUI countText;
		[SerializeField] private FriendsPanel panel;
        [SerializeField] private Button button;


		private FriendsProvider _friendsProvider;

		[Inject]
		private void Construct(FriendsProvider friendsProvider)
		{
			_friendsProvider = friendsProvider;
		}

		public void Init()
        {
            Changed();
            _friendsProvider.OnRequestsChanged += Changed;
            button.onClick.AddListener(() => panel.SetActive(true));
        }
        
        private void Changed()
        {
            if (_friendsProvider.Requests.Count > 0)
            {
                friendRequest.gameObject.SetActive(true);
                countText.text = _friendsProvider.Requests.Count.ToString();
            }
            else
            {

                friendRequest.gameObject.SetActive(false);
            }
        }
        private void OnDestroy()
        {
            _friendsProvider.OnRequestsChanged += Changed;
        }
    }

}