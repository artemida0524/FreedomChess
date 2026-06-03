using Game.Runtime.Background;
using UnityEngine;

namespace Game.Runtime.Gameplay
{
	public class GameplayEntryPoint : MonoBehaviour
	{
        [SerializeField] private PlayerStatsView playerStats;
        [SerializeField] private FriendButtonView friendButton;
        [SerializeField] private FriendsPanel friendPanel;
        [SerializeField] private BattleRequestsListener battleListener;
        [SerializeField] private ExitBattleButton exitBattleButton;
        [SerializeField] private ExitBattlePanel exitBattlePanel;

        [SerializeField] private PlayerStatsView settingsPlayerStats;
        [SerializeField] private SettingsButtonView settingsButtonView;
        [SerializeField] private SettingsPanelView settingsPanelView;

        private void Start()
        {
            playerStats.Init();

            friendButton.Init();
            friendPanel.Init();
            battleListener.Init();
            BattleManager.Instance.Init();
            exitBattleButton.Init();
            exitBattlePanel.Init();

            settingsButtonView.Init();
            settingsPanelView.Init();

            settingsPlayerStats.Init();
        }
    }

}