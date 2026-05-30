using UnityEngine;
using UnityEngine.UI;

namespace Game.Core.Bar
{
    public class BarView : BarViewBase
    {
		[SerializeField] private Slider slider;

        public override void SetValue(float value)
        {
            slider.value = value * slider.maxValue;
        }
    }

}
