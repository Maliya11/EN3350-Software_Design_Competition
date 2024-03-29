using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RequestManager : ScriptableObject
{    
    public bool IsRequestCompleted { get; private set; }
    private string jwtToken;

    // Method to send a request to the server
    public void SendRequest(string url, string method, string body, MonoBehaviour monoBehaviour, Dictionary<string, string> parameters = null)
    {
        jwtToken = PlayerPrefs.GetString("jwtToken");
        if (jwtToken == null)
        {
            Debug.Log("JWT Token is null");
        }
        monoBehaviour.StartCoroutine(SendRequestCoroutine(url, method, body, parameters));
    }

    // Coroutine to handle the request
    private IEnumerator SendRequestCoroutine(string url, string method, string body, Dictionary<string, string> parameters)
    {
        string parametersString = "";
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                parametersString += "&" + param.Key + "=" + param.Value;
            }
            parametersString = parametersString.TrimStart('&');
        }

        // Construct URL with parameters if they exist
        if (!string.IsNullOrEmpty(parametersString))
        {
            url += "?" + parametersString;
        }

        byte[] data = null;
        if (!string.IsNullOrEmpty(body))
        {
            data = System.Text.Encoding.UTF8.GetBytes(body);
        }

        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            
            if (data != null)
            {
                request.uploadHandler = new UploadHandlerRaw(data);
            }

            request.SetRequestHeader("Content-Type", "application/json");

            // Set JWT token if available
            if (!string.IsNullOrEmpty(jwtToken))
            {   
                request.SetRequestHeader("Authorization", "Bearer " + jwtToken);
            }

            // Send the request asynchronously
            yield return request.SendWebRequest();

            // Check the result of the request
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Request successful. Response: " + responseText);
                IsRequestCompleted = true;
            }
            else
            {
                string errorMessage = "Request failed. Error: " + request.error;
                Debug.LogError(errorMessage);
                IsRequestCompleted = false;
            }
        }
    }    
}
