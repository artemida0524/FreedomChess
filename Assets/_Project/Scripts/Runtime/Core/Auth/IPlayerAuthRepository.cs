using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Game.Core.Player;
using System;

namespace Game.Runtime.Core.Auth
{

    public interface IPlayerAuthRepository
    {
        bool IsLoggedIn { get; }
        string Email { get; }

        event Action<string> OnLoggedOut;

        UniTask Init();

        PlayerCacheResult IsPlayerCached();

        UniTask<AuthError> CreatePlayerAsync(string email, string password);
        UniTask<AuthError> LogInAsync(string email, string password);
        UniTask<AuthError> LogInAsync();
        UniTask<bool> IsEmailVerifiedAsync();
        UniTask<EmailVerificationType> SendEmailVerificationAsync();
        UniTask<bool> ResetPassword(string email);
        UniTask<AuthError> SignInWithGoogle();

        void LogOut();
    }

}