using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay
{
    public class SettingsButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private SettingsPanelView panel;


        public void Init()
        {
            button.onClick.AddListener(() => panel.SetActive(true));
        }
    }
}
