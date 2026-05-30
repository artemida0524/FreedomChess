using Game.Core.Bar;
using TMPro;
using UnityEngine;


namespace Game.Runtime.AppStart.Views
{

    public class AppStartPanelView : MonoBehaviour, IAppStartPanelView
    {
        [SerializeField] private BarViewBase globalBarView;

        [SerializeField] private TextMeshProUGUI taskNameText;
        [SerializeField] private TextMeshProUGUI procentText;

        public void SetGlobalValue(float value)
        {
            globalBarView.SetValue(value);
            procentText.text = $"{(int)(value * 100)}%";
        }

        public void SetTaskName(string name)
        {
            taskNameText.text = name;
        }

        public void SetTaskValue(float value)
        {
            
        }
    }

}
