using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

// Represents an authentication manager responsible for handling authentication requests
public class AuthenticationManager : MonoBehaviour
{
    // Property to check if the authentication process is completed
    public bool IsAuthenticated { get; private set; }

    // Constructor to initialize properties
    public AuthenticationManager()
    {
        IsAuthenticated = false;
    }

    // Method to authenticate with the server using the provided API key
    public void Authenticate(string apiKey, MonoBehaviour monoBehaviour)
    {
        // Start the coroutine
        monoBehaviour.StartCoroutine(AuthenticateCoroutine(apiKey));
    }

    // Coroutine to handle the authentication process
    private IEnumerator AuthenticateCoroutine(string apiKey)
    {
        string url = "http://20.15.114.131:8080/api/login";

        string json = "{\"apiKey\":\"" + apiKey + "\"}";
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(data);

            // Send the request asynchronously
            yield return request.SendWebRequest();

            // Check the result of the request
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                AuthenticationResponse responseData = JsonUtility.FromJson<AuthenticationResponse>(responseText);
                string jwtToken = responseData.token;

                // Set the token in the player prefs for future use
                PlayerPrefs.SetString("jwtToken", jwtToken);

                Debug.Log("Authentication successful. Token: " + jwtToken);
                IsAuthenticated = true;
            }
            else
            {
                string errorMessage = "Authentication failed. Error: " + request.error;
                Debug.LogError(errorMessage);
            }
        }
    }
}

// Represents the authentication response from the server
[Serializable]
public class AuthenticationResponse
{
    public string token;
}
