using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using TMPro;

public class QuestionnaireManager : MonoBehaviour
{
    // Reference to the RequestManager
    private RequestManager requestManager;
    
    private bool userValidity;
    /* User Validity:
     * true  - User is  a valid player in the database
     * false - User is not a valid player in the database
     */

    private int questionnaireStatus;
    /* Questionnaire Status:
     * 0   - User has not completed the questionnaire         => Prompt user to complete the questionnaire
     * 1-9 - User has partially completed the questionnaire   => Prompt user to complete the questionnaire
     * 10  - User has completed the questionnaire             => Proceed to the game
     */

    private int questionnaireMarks;
    /* Questionnaire Marks:
     * Marks are assigned out of 10
     */

    private int bonusGiven;
    /*
     * 0 - Bonus perks have not been given yet
     * 1 - Bonus perks have been given to the user
     */

    // UI elements
    public GameObject notificationBar;
    public TextMeshProUGUI notificationText;
    public TextMeshProUGUI questionnaireButtonNormalText;
    public TextMeshProUGUI questionnaireButtonPressedText;
    public GameObject questionnaireButtonNormal;
    public GameObject questionnaireButtonPressed;

    // Method to get the questionnaire status from the database
    public void GetQuestionnaireStatus(int promptingOrigin)
    {
        string questionnaireStatusURL = "http://16.170.233.8:8080/player/authenticate";
        string questionnaireStatusMethod = "POST";
        string apiKey = "NjVjNjA0MGY0Njc3MGQ1YzY2MTcyMmNiOjY1YzYwNDBmNDY3NzBkNWM2NjE3MjJjMQ";

        // Create a new instance of the RequestManager
        requestManager = ScriptableObject.CreateInstance<RequestManager>();

        // Create the body of the request
        var requestBody = new JSONObject();
        requestBody["apiKey"] = apiKey;
        string jsonBody = requestBody.ToString();

        requestManager.SendRequest(questionnaireStatusURL, questionnaireStatusMethod, jsonBody, this, null);

        StartCoroutine(WaitForQuestionnaireStatusRequestCompletion(promptingOrigin));

        Debug.Log("Questionnaire status request sent");
    }

    private IEnumerator WaitForQuestionnaireStatusRequestCompletion(int promptingOrigin)
    {
        while (!requestManager.isRequestCompleted)
        {
            yield return null;
        }

        if (requestManager.isRequestSuccessful)
        {
            Debug.Log("Questionnaire status request successful");
            Debug.Log(requestManager.jsonResponse);
            userValidity = requestManager.jsonResponse["validKey"];
            questionnaireStatus = requestManager.jsonResponse["completedQuestions"];
            Debug.Log("User validity: " + userValidity);
            Debug.Log("Questionnaire status: " + questionnaireStatus);
            PromptUser(promptingOrigin);
        }
        else
        {
            Debug.Log("Questionnaire status request failed");
        }
    }

    // Method to prompt the user based on the questionnaire status
    private void PromptUser(int promptingOrigin)
    {
        /* Prompting Origin:
         * 0  - Prompting user from the play button
         * 1  - Prompting user from the questionnaire button
         */
        if (userValidity)
        {
            Debug.Log("User is valid");
            if (questionnaireStatus < 10)
            {
                Debug.Log("Prompting user to complete the questionnaire...");
                notificationBar.SetActive(true);
                notificationText.text = "Explore the fundamentals of energy efficiency with questions on electricity generation, transmission, and usage.\n All users must complete the questionnaire before proceeding to the game.";
                questionnaireButtonNormalText.text = "Fill the Questionnaire";
                questionnaireButtonPressedText.text = "Fill the Questionnaire";
            }
            else
            {
                if (promptingOrigin == 1)
                {
                    Debug.Log("User has already completed the questionnaire");
                    notificationBar.SetActive(true);
                    notificationText.text = "Explore the fundamentals of energy efficiency with questions on electricity generation, transmission, and usage.\n You have already completed the questionnaire.";
                    questionnaireButtonNormalText.text = "Review the Questionnaire";
                    questionnaireButtonPressedText.text = "Review the Questionnaire";
                    GetQuestionnaireMarks();
                }
                else
                {
                    Debug.Log("User has completed the questionnaire");
                    GetQuestionnaireMarks();
                }
            }
        }
        else
        {
            Debug.Log("User is invalid");
        }
    }

    // Method to direct the user to the questionnaire
    public void DirectToQuestionnaire()
    {
        Debug.Log("Directing user to the questionnaire...");
        questionnaireButtonNormal.gameObject.SetActive(false);
        questionnaireButtonPressed.gameObject.SetActive(true);
        Application.OpenURL("https://www.google.com/");
    }

    // Method to close the notification bar 
    public void CloseNotificationBar()
    {
        notificationBar.SetActive(false);
        questionnaireButtonNormal.gameObject.SetActive(true);
        questionnaireButtonPressed.gameObject.SetActive(false);
    }

    // Method to get the marks obtained by the user in the questionnaire
    public void GetQuestionnaireMarks()
    {
        string questionnaireMarksURL = "http://16.170.233.8:8080/player/details";
        string questionnaireMarksMethod = "GET";

        // Create a new instance of the RequestManager
        requestManager = ScriptableObject.CreateInstance<RequestManager>();

        requestManager.SendRequest(questionnaireMarksURL, questionnaireMarksMethod, null, this, null);

        StartCoroutine(WaitForQuestionnaireMarksRequestCompletion());   
    }

    private IEnumerator WaitForQuestionnaireMarksRequestCompletion()
    {
        while (!requestManager.isRequestCompleted)
        {
            yield return null;
        }

        if (requestManager.isRequestSuccessful)
        {
            Debug.Log("Questionnaire marks request successful");
            questionnaireMarks = requestManager.jsonResponse["marks"];
            bonusGiven = requestManager.jsonResponse["bonusGiven"];
            Debug.Log("Questionnaire marks: " + questionnaireMarks);
            Debug.Log("Bonus given: " + bonusGiven);
            AssignBonusPerks();
            Debug.Log("Bonus perks: " + PlayerPrefs.GetInt("questionnaireBonus"));
        }
        else
        {
            Debug.Log("Questionnaire marks request failed");
        }
    }

    // Method to assign bonus perks based on the marks obtained by the user in the questionnaire
    private void AssignBonusPerks()
    {
        if (bonusGiven == 0)
        {
            Debug.Log("Assigning bonus perks to the user...");
            PlayerPrefs.SetInt("questionnaireBonus", questionnaireMarks);
            Debug.Log("Bonus perks assigned to the user");

            // Update the bonusGiven status in the database
            string bonusPerksURL = "http://16.170.233.8:8080/player/bonus";
            string bonusPerksMethod = "POST";

            // Create the body of the request
            var requestBody = new JSONObject();
            requestBody["bonusGiven"] = 1;
            string jsonBody = requestBody.ToString();

            requestManager.SendRequest(bonusPerksURL, bonusPerksMethod, jsonBody, this, null);

            StartCoroutine(WaitForBonusPerksRequestCompletion());
        }
        else
        {
            Debug.Log("Bonus perks have already been assigned to the user");
        }
    }

    private IEnumerator WaitForBonusPerksRequestCompletion()
    {
        while (!requestManager.isRequestCompleted)
        {
            yield return null;
        }

        if (requestManager.isRequestSuccessful)
        {
            Debug.Log("Bonus given status update request successful");
        }
        else
        {
            Debug.Log("Bonus given status update request failed");
        }
    }

}
