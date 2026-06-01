using Game.Runtime.Background;
using UnityEngine;

namespace Game.Runtime.Gameplay
{
	public class GameplayEntryPoint : MonoBehaviour
	{
        [SerializeField] private PlayerStatsView playerStats;

        private void Start()
        {
            playerStats.Init();
        }
    }

}