using System;

namespace Game.Runtime.AppStart.Views
{
    public interface IGoBacktoExistentPanelView
    {
        event Action OnGoBackToExistentClicked;
        void Init(string email);
        void Disable();
    }

}