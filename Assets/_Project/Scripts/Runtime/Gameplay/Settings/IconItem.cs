using Game.Core.Sounds;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay
{
    public class IconItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        public event Action<IconStat> IconClicked;

        public void Init(IconStat iconStat)
        {
            icon.sprite = iconStat.icon;
            button.onClick.AddListener(() => IconClicked?.Invoke(iconStat));
            button.onClick.AddListener(() => SoundManager.Instance.Click());
        }
    }

}
