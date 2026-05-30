using System;
using UnityEngine;
using Firebase.Auth;
using Google;
using Cysharp.Threading.Tasks;

public class GoogleSignInWithFirebase
{

    private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;

    public TextAsset textAsset;


    public static void Init()
    {
        
        var configuration = new GoogleSignInConfiguration
        {
            WebClientId = "661963047284-kcs5fdhqdr6pe5tqkhdra54u7g09h7aa.apps.googleusercontent.com",
            RequestIdToken = true,
            RequestEmail = true,
        };

        GoogleSignIn.Configuration = configuration;
    }


    public async UniTask<Credential> GetGoogleCredentialAsync()
    {
        GoogleSignInUser user = null;

        try
        {
            user = await GoogleSignIn.DefaultInstance.SignIn();
        }
        catch (Exception)
        {

        }


        string idToken = user.IdToken;

        if (string.IsNullOrEmpty(idToken))
        {
            Debug.LogError("IdToken is null or empty.");
            return null;
        }


        return GoogleAuthProvider.GetCredential(idToken, null);
    }
}
