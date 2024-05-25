using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Cainos.PixelArtPlatformer_VillageProps;

// Manage the functionality of the treasure
public class Treasure : MonoBehaviour
{
    // UI Elements
    public GameObject questionPanel;
    public TextMeshProUGUI panelTitleText;
    public TextMeshProUGUI panelQuestionText;
    public Button yesButtonLeft;
    public TextMeshProUGUI yesButtonLeftText;
    public Button noButtonRight;
    public TextMeshProUGUI noButtonRightText;
    public GameObject player;
    public bool hasGuardianEnemy;
    public GameObject guardianEnemy;
    private bool isGuardianEnemyDead;


    // Unique ID of the treasure
    public int treasureID;
    // Unique question and answer for the treasure
    private string question;
    private int answer;


    private void Start()
    {
        // Enable the buttons
        yesButtonLeft.interactable = true;
        noButtonRight.interactable = true;

        // If the treasure has a guardian enemy, disable the collider of the treasure
        if (hasGuardianEnemy)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            isGuardianEnemyDead = false;
        }
    }

    private void Update()
    {
        // Enable the collider of the treasure once the guardian enemy is defeated
        // If the treasure has a guardian enemy, check if the enemy is dead
        if (hasGuardianEnemy)
        {
            // If collider of the guardian enemy is disabled, the enemy is dead
            if (guardianEnemy.GetComponent<CapsuleCollider2D>().enabled == false)
            {
                isGuardianEnemyDead = true;
            }

            if (isGuardianEnemyDead)
            {
                // Enable the collider of the treasure
                GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }

    // Open the chest once the player collides with it
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Open the chest using the Chest script attached to the chest
            GetComponent<Chest>().Open();

            // Wait until the animation is completed
            StartCoroutine(ShowQuestionPanel());
        }
    }

    // Start the coroutine to show the question panel
    private IEnumerator ShowQuestionPanel()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(0.5f);

        // Retrieve the question and answer for the treasure from PlayerPrefs
        string jsonResponse = PlayerPrefs.GetString("treasureQuestions", string.Empty);
        List<Question> questions = JsonConvert.DeserializeObject<List<Question>>(jsonResponse);
        
        // Get the question and answer at the index of the treasure ID
        question = questions[treasureID - 1].q;
        answer = questions[treasureID - 1].corAns;

        Debug.Log("Question: " + question);
        Debug.Log("Answer: " + answer);

        // Set the title and question text
        panelTitleText.text = "Treasure!";
        panelQuestionText.text = "Answer the question to receive a Key: \n" + question; 
        yesButtonLeftText.text = "Yes";
        noButtonRightText.text = "No";

        // Wait for button click
        yesButtonLeft.onClick.AddListener(() => ButtonClick("Yes"));
        noButtonRight.onClick.AddListener(() => ButtonClick("No"));

        // Hide the chest and player
        gameObject.SetActive(false);
        player.SetActive(false);

        // Display the question panel
        questionPanel.SetActive(true);
    }

    // Functionality of the yes button
    public void ButtonClick(string buttonPressed)
    {
        // If the answer is yes and the button pressed is yes or the answer is no and the button pressed is no
        if ((answer == 1 && buttonPressed == "Yes") || (answer == 2 && buttonPressed == "No"))
        {
            Debug.Log("Correct answer!");

            // Hide the question panel
            questionPanel.SetActive(false);

            // Enable the player
            player.SetActive(true);

            // Disable the chest
            gameObject.SetActive(false);

            // Add a key to the player's inventory
            Debug.Log("Key added to the inventory!");

            // Remove the listeners
            yesButtonLeft.onClick.RemoveListener(() => ButtonClick("Yes"));
            noButtonRight.onClick.RemoveListener(() => ButtonClick("No"));
        }
        else
        {
            Debug.Log("Incorrect answer!");

            // Hide the question panel
            questionPanel.SetActive(false);

            // Enable the player
            player.SetActive(true);

            // Disable the chest
            gameObject.SetActive(false);

            // Remove the listeners
            yesButtonLeft.onClick.RemoveListener(() => ButtonClick("Yes"));
            noButtonRight.onClick.RemoveListener(() => ButtonClick("No"));
        }
    }
}
