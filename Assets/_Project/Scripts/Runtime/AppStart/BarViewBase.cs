using UnityEngine;

namespace Game.Core.Bar
{
    public abstract class BarViewBase : MonoBehaviour, IBarView
    {
        public abstract void SetValue(float value);
    }

}
