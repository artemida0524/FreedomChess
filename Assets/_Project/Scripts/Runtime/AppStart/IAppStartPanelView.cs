namespace Game.Runtime.AppStart.Views
{
    public interface IAppStartPanelView
    {
        void SetGlobalValue(float value); // ALWAYS RANGE 0-1
        void SetTaskValue(float value); // ALWAYS RANGE 0-1
        void SetTaskName(string name);
    }
}
