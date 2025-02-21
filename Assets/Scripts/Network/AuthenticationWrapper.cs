using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState CurrentAuthState { get; private set; } = AuthState.NotAuthenticated;

    static TaskCompletionSource<AuthState> authTaskCompletionSource;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        Debug.Log("Authenticating...");

        if (CurrentAuthState == AuthState.Authenticated)
        {
            Debug.Log("Already Authenticated");

            return CurrentAuthState;
        }

        if (CurrentAuthState == AuthState.Authenticating)
        {
            Debug.Log("Already in the process of Authenticating");

            return await authTaskCompletionSource.Task;
        }

        CurrentAuthState = AuthState.Authenticating;
        authTaskCompletionSource = new TaskCompletionSource<AuthState>();

        int tries = 0;

        while (CurrentAuthState == AuthState.Authenticating && tries < maxTries)
        {
            Debug.Log($"Attempts {tries + 1} to authenticate");

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    CurrentAuthState = AuthState.Authenticated;
                    Debug.Log("Authentication successful");

                    authTaskCompletionSource.SetResult(CurrentAuthState);

                    break;
                }
            }
            catch(AuthenticationException ex)
            {
                Debug.LogError($"Authentication failed with exception: {ex.Message}");

                CurrentAuthState = AuthState.Error;
                authTaskCompletionSource.SetResult(CurrentAuthState);
                break;
            }

            tries++;
            Debug.LogWarning("Authentication failed. Trying again!");
            await Task.Delay(2000);
        }

        if(CurrentAuthState != AuthState.Authenticated)
        {
            Debug.LogError("Authentication failed after max number of tries!");
            CurrentAuthState = AuthState.Error;
            authTaskCompletionSource.SetResult(CurrentAuthState);
        }

        return CurrentAuthState;
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    Timeout
}
