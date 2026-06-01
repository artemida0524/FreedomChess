using Game.Runtime.Background;
using UnityEngine;

namespace Game.Runtime.Gameplay
{
	public class GameplayEntryPoint : MonoBehaviour
	{
        [SerializeField] private PlayerStatsView playerStats;
        [SerializeField] private FriendButtonView friendButton;
        [SerializeField] private FriendsPanel friendPanel;
        

        private void Start()
        {
            playerStats.Init();
            friendButton.Init();
            friendPanel.Init();
        }
    }

}