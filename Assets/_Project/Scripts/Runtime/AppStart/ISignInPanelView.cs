using System;

namespace Game.Runtime.AppStart.Views
{
    public interface ISignInPanelView
    {
        string Email { get; }
        string Password { get; }
        string ConfirmPassword { get; }

        event Action OnSignInButtonClicked;
        event Action OnSignUpButtonClicked;

        event Action OnGoogleSignInClicked;
        event Action OnResetPasswordButtonClicked;

        event Action OnGoBackToExistentClicked;

        void Init();

        void SetInteractable(bool interactable);
        void SetLoading(bool isLoading);

        void ShowGoBackToSignedPanel(string email);

        void DisableContinueAs();

        void Open();
        void Close();

    }
}
