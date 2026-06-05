using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Runtime.Gameplay
{
    public class Icons : MonoBehaviour
    {
        [SerializeField] private Button exitButton;
        [SerializeField] private GameObject panel;
        [SerializeField] private IconItem iconItem;
        [SerializeField] private RectTransform container;

        public event Action<IconStat> iconClicked;

        private IconStatManager _iconStatManager;

        [Inject]
        private void Construct(IconStatManager iconStatManager)
        {
            _iconStatManager = iconStatManager;
        }

        public void Init()
        {
            exitButton.onClick.AddListener(() => SetActive(false));
            foreach (var item in _iconStatManager.icons)
            {
                IconItem instance = Instantiate(iconItem, container);

                IconStat iconStat = _iconStatManager.GetIconStatById(item.iconId);

                instance.Init(iconStat);
                instance.IconClicked += Instance_IconClicked;
            }
        }

        public void SetActive(bool active)
        {
            panel.SetActive(active);
        }

        private void Instance_IconClicked(IconStat obj)
        {
            iconClicked?.Invoke(obj);
        }
    }

}
