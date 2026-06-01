using Game.Runtime.Core.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Runtime.Gameplay
{
	public class PlayerStatsView : MonoBehaviour
	{
		[SerializeField] private Image image;
		[SerializeField] private TextMeshProUGUI nameEloText;

		private PlayerProvider _playerProvider;
		private IconStatManager _iconStatManager;

        [Inject]
		private void Construct(PlayerProvider playerProvider, IconStatManager iconStatManager)
		{
			_playerProvider = playerProvider;
			_iconStatManager = iconStatManager;
		}

		public void Init()
		{
			_playerProvider_OnPlayerDataChanged();
            _playerProvider.OnPlayerDataChanged += _playerProvider_OnPlayerDataChanged;
		}

        private void _playerProvider_OnPlayerDataChanged()
        {
            UpdateData(_playerProvider.Name, _playerProvider.Elo, _playerProvider.Icon);
        }

        private void UpdateData(string name, int elo, int icon)
		{
			nameEloText.text = $"{name} ({elo})";
			image.sprite = _iconStatManager.GetIconStatById(icon).icon;
        }


        private void OnDestroy()
        {
            _playerProvider.OnPlayerDataChanged -= _playerProvider_OnPlayerDataChanged;
        }
    } 
}
