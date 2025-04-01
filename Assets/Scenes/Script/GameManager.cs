using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

/// <summary>
/// Class Documentation for `GameManager`
/// Responsible for managing the game logic for a trivia game where players guess a word based on four images.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance for easy access

    public Image[] questionImages; // Array of the 4 images for the current question
    public Button letterButtonPrefab; // Prefab for letter buttons
    public Transform letterButtonContainer; // Container for letter buttons
    public Transform answerSlotContainer1; // Container for answer slots (1st word)
    public Transform answerSlotContainer2; // Container for answer slots (2nd word)
    public Transform answerSlotContainer3; // Container for answer slots (3rd word)

    public int score = 0; // Player Score
    public Text scoreText; // UI element to display the score
    public GameObject gameOverPanel; // Panel shown on game over
    public Text finalScoreText; // UI element to display the final score

    public QuestionData[] allQuestions; // Array of questions to choose from
    private QuestionData currentQuestion; // Current question being answered
    private int questionIndex = 0; // Index of the current question

    public GameObject inputFieldPrefab; // Prefab for input fields

    public Button submitButton; // Button to submit the answer
    private int questionsAnswered = 0; // Number of questions answered
    private int totalQuestions; // Total number of questions available

    public AudioClip correctAnswerClip; // Audio clip for correct answer feedback
    public AudioClip wrongAnswerClip; // Audio clip for wrong answer feedback
    private AudioSource audioSource; // Audio source component for playing audio
    public AudioClip buttonClickClip; // Audio clip for button click feedback

    // **Additional UI Elements**
    public GameObject triviaModal; // Panel displaying trivia information
    public Text triviaText; // Text component for showing trivia
    public Button closeTriviaButton; // Button to close the trivia panel

    public GameObject hintPanel; // Panel for hint confirmation
    public Button confirmHintButton; // Button to confirm using a hint
    public Button cancelHintButton; // Button to cancel hint usage
    public TextMeshProUGUI hintText; // Text to show hint message

    public GameObject noHintPanel; // Panel to show insufficient points message
    public TextMeshProUGUI noHintText; // Text inside insufficient points panel
    public Button noHintOkButton; // OK button for insufficient points panel

    public GameObject continuePanel; // Panel to confirm continuation after answering
    public Button continueButton; // Button to continue to the next question
    public Button exitButton; // Button to exit the game

    // **Dictionary to track input fields and their corresponding buttons**
    private Dictionary<InputField, Button> inputFieldButtonMap = new Dictionary<InputField, Button>();

    /// <summary>
    /// Awake method used to implement Singleton pattern.
    /// Initializes the GameManager instance or destroys duplicate instances.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set the singleton instance
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }

    /// <summary>
    /// Start method called when the script instance is being loaded.
    /// Initializes variables and sets up event listeners.
    /// </summary>
    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the audio source component
        ShuffleArray(allQuestions); // Shuffle questions for randomness
        totalQuestions = allQuestions.Length; // Get the total number of questions

        score = PlayerPrefs.GetInt("PlayerScore", 0); // Load the saved score
        scoreText.text = score.ToString(); // Update score UI
        submitButton.onClick.AddListener(CheckAnswer); // Add listener for submitting answers
        closeTriviaButton.onClick.AddListener(CloseTrivia); // Add listener for closing trivia modal
        triviaModal.SetActive(false); // Hide trivia modal initially

        // Additional event listeners for hint purchase
        confirmHintButton.onClick.AddListener(ConfirmHintPurchase);
        cancelHintButton.onClick.AddListener(CloseHintPanel);
        hintPanel.SetActive(false); // Hide the hint panel initially

        noHintPanel.SetActive(false); // Hide insufficient points panel initially
        noHintOkButton.onClick.AddListener(CloseNoHintPanel); // Add listener to OK button
        ResetGame(); // Start the first round of the game

        continuePanel.SetActive(false); // Hide continue panel initially
        continueButton.onClick.AddListener(ContinueGame); // Add listener for continuation
        exitButton.onClick.AddListener(ExitGame); // Add listener for exiting the game
    }

    /// <summary>
    /// Resets the game for the next question or shows the game over panel.
    /// </summary>
    private void ResetGame()
    {
        if (questionsAnswered >= totalQuestions)
        {
            ShowGameOverPanel(); // Show the game over panel if all questions are answered
            return;
        }

        currentQuestion = GetNextQuestion(); // Get the next question
        if (currentQuestion != null)
        {
            SetQuestionImages(); // Set images for the current question
            SetupAnswerSlots(); // Setup answer slots for input
            InstantiateLetterButtons(); // Instantiate letter buttons for user input
        }
    }

    private HashSet<InputField> lockedHintFields = new HashSet<InputField>(); // Stores locked input fields that were filled by hints

    /// <summary>
    /// Confirms the purchase of a hint and applies it if sufficient points are available.
    /// </summary>
    private void ConfirmHintPurchase()
    {
        if (score >= 10) // Check if the player has enough points
        {
            hintPanel.SetActive(false); // Hide the hint confirmation panel
            ApplyHint(); // Call to apply the hint
        }
        else
        {
            noHintText.text = "Not enough points to use a hint!"; // Show message for insufficient points
            noHintPanel.SetActive(true);
            hintPanel.SetActive(false); // Hide the hint panel
        }
    }

    /// <summary>
    /// Closes the hint panel.
    /// </summary>
    private void CloseHintPanel()
    {
        hintPanel.SetActive(false); // Hide hint panel
    }

    /// <summary>
    /// Applies a hint by revealing a random letter in one of the answer slots.
    /// </summary>
    private void ApplyHint()
    {
        string correctAnswer = currentQuestion.correctWord.Replace(" ", "").ToUpper(); // Get correct answer in uppercase
        char[] correctChars = correctAnswer.ToCharArray(); // Convert answer to character array

        List<InputField> emptySlots = new List<InputField>(); // List to hold empty input fields
        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 }) // Iterate through answer containers
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                if (string.IsNullOrEmpty(inputField.text)) // Check if input field is empty
                {
                    emptySlots.Add(inputField); // Add empty field to the list
                }
            }
        }

        if (emptySlots.Count > 0) // Proceed if there are empty slots
        {
            InputField chosenSlot = emptySlots[Random.Range(0, emptySlots.Count)]; // Randomly select an empty slot
            int index = GetSlotIndex(chosenSlot); // Get the index of the selected slot

            if (index != -1) // Check if index is valid
            {
                chosenSlot.text = correctChars[index].ToString(); // Fill selected slot with the hint letter
                chosenSlot.interactable = false; // Lock the field so it can't be changed
                lockedHintFields.Add(chosenSlot); // Add to locked fields

                // Disable the corresponding letter button
                foreach (Button button in letterButtonContainer.GetComponentsInChildren<Button>())
                {
                    if (button.GetComponentInChildren<Text>().text == correctChars[index].ToString() && button.interactable)
                    {
                        button.interactable = false; // Disable hint button
                        break; // Exit loop after disabling
                    }
                }

                // Save hint data for persistency
                PlayerPrefs.SetString("HintedLetter_" + index, correctChars[index].ToString());
                PlayerPrefs.SetInt("HintedSlot_" + index, 1);
                PlayerPrefs.Save(); // Save changes to PlayerPrefs

                UpdateScore(-10); // Deduct points for using a hint
            }
        }
    }

    /// <summary>
    /// Helper function to find the index of an input slot in the answer.
    /// </summary>
    /// <param name="slot">The input field whose index needs to be found.</param>
    /// <returns>The index of the input field in the overall answer.</returns>
    private int GetSlotIndex(InputField slot)
    {
        List<InputField> allSlots = new List<InputField>(); // List to hold all slots
        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 }) // Iterate through all answer containers
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                allSlots.Add(inputField); // Add each input field to the allSlots list
            }
        }

        return allSlots.IndexOf(slot); // Get index of the current slot
    }

    /// <summary>
    /// Closes the 'no hint' panel.
    /// </summary>
    private void CloseNoHintPanel()
    {
        noHintPanel.SetActive(false); // Hide no hint panel
    }

    /// <summary>
    /// Method to show the hint panel when the player wants to use a hint.
    /// </summary>
    public void UseHint()
    {
        if (score < 10)
        {
            noHintText.text = "Not enough points to use a hint!"; // Message for insufficient points
            noHintPanel.SetActive(true); // Show no hint panel
            return; // Exit method
        }

        // Open hint confirmation panel
        hintPanel.SetActive(true);
        hintText.text = "Use 10 points to reveal a letter?"; // Show confirmation message
    }

    /// <summary>
    /// Retrieves the next question from the question pool.
    /// </summary>
    /// <returns>The next QuestionData object.</returns>
    private QuestionData GetNextQuestion()
    {
        if (questionIndex < allQuestions.Length)
        {
            return allQuestions[questionIndex++]; // Return the next question and increment index
        }
        return null; // No more questions available
    }

    /// <summary>
    /// Sets the images for the current question.
    /// </summary>
    private void SetQuestionImages()
    {
        for (int i = 0; i < questionImages.Length; i++)
        {
            questionImages[i].sprite = currentQuestion.images[i]; // Set image for each question image slot
        }
    }

    /// <summary>
    /// Sets up the answer slots based on the correct answer from the current question.
    /// </summary>
    private void SetupAnswerSlots()
    {
        // Destroy previous answer slots and prepare for the new ones
        DestroyAnswerSlots(answerSlotContainer1);
        DestroyAnswerSlots(answerSlotContainer2);
        DestroyAnswerSlots(answerSlotContainer3);

        string[] words = currentQuestion.correctWord.Split(' '); // Split the correct word into individual words

        if (words.Length > 0) SetupSlotsForWord(words[0], answerSlotContainer1); // Setup slots for the first word
        if (words.Length > 1) SetupSlotsForWord(words[1], answerSlotContainer2); // Setup slots for the second word
        if (words.Length > 2) SetupSlotsForWord(words[2], answerSlotContainer3); // Setup slots for the third word
    }

    /// <summary>
    /// Destroys all existing answer slots in a specified container.
    /// </summary>
    /// <param name="container">The container whose slots are to be destroyed.</param>
    private void DestroyAnswerSlots(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject); // Destroy each child (answer slot)
        }
    }

    /// <summary>
    /// Sets up input fields for a given word in a specified container.
    /// </summary>
    /// <param name="word">The word for which slots are being created.</param>
    /// <param name="container">The container for these slots.</param>
    private void SetupSlotsForWord(string word, Transform container)
    {
        foreach (char _ in word) // Iterate through each character in the word
        {
            GameObject slot = Instantiate(inputFieldPrefab, container); // Instantiate a new input field
            InputField inputField = slot.GetComponent<InputField>(); // Get the input field component
            inputField.text = ""; // Clear any existing text
            inputField.characterLimit = 1; // Set limit to 1 character
            inputField.image.color = Color.black; // Set default image color (border color)
            inputField.textComponent.color = Color.white; // Set the text color to white
            inputField.textComponent.alignment = TextAnchor.MiddleCenter; // Center the text in the input field

            RectTransform rectTransform = inputField.GetComponent<RectTransform>(); // Get the RectTransform component
            rectTransform.sizeDelta = new Vector2(50, 50); // Set size for the input field
            rectTransform.localPosition = Vector3.zero; // Reset local position

            inputField.interactable = false; // Make the input field non-interactable initially

            // Add EventTrigger to detect click
            EventTrigger trigger = inputField.gameObject.AddComponent<EventTrigger>(); // Add an EventTrigger component

            EventTrigger.Entry entry = new EventTrigger.Entry(); // Create a new entry for the event
            entry.eventID = EventTriggerType.PointerDown; // Specify the event type
            entry.callback.AddListener((data) => { ClearInputField(inputField); }); // Add listener to clear the input field on click

            trigger.triggers.Add(entry); // Add the entry to the trigger
        }
    }

    /// <summary>
    /// Clears the input field when clicked unless it is locked by a hint.
    /// </summary>
    /// <param name="inputField">The input field to be cleared.</param>
    private void ClearInputField(InputField inputField)
    {
        if (lockedHintFields.Contains(inputField))
        {
            return; // Prevent clearing if the field was filled by a hint
        }

        inputField.text = ""; // Clear the input field

        // Handle the associated button reference
        if (inputFieldButtonMap.ContainsKey(inputField))
        {
            Button associatedButton = inputFieldButtonMap[inputField]; // Get the associated button
            if (associatedButton != null)
            {
                associatedButton.interactable = true; // Make the button interactable again
            }

            inputFieldButtonMap.Remove(inputField); // Remove the mapping of input field and button
        }
    }

    /// <summary>
    /// Instantiates the letter buttons based on the current question.
    /// </summary>
    private void InstantiateLetterButtons()
    {
        foreach (Transform child in letterButtonContainer)
        {
            Destroy(child.gameObject); // Clear existing letter buttons
        }

        string correctAnswer = currentQuestion.correctWord.Replace(" ", "").ToUpper(); // Process the correct answer

        List<string> lettersForButtons = correctAnswer.ToCharArray()
            .Select(c => c.ToString())
            .ToList(); // Convert answer characters to a list of strings

        // Generate additional random letters to fill the button pool
        List<string> randomLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            .ToCharArray()
            .Select(c => c.ToString())
            .Where(letter => !lettersForButtons.Contains(letter))
            .OrderBy(_ => Random.value)
            .Take(5) // Limit to 5 random letters
            .ToList();

        lettersForButtons.AddRange(randomLetters); // Combine correct and random letters
        ShuffleList(lettersForButtons); // Shuffle letters for randomness

        // Create buttons for letter choices
        foreach (string letter in lettersForButtons)
        {
            GameObject newButton = Instantiate(letterButtonPrefab.gameObject, letterButtonContainer); // Instantiate a new letter button
            Button button = newButton.GetComponent<Button>(); // Get the button component
            button.GetComponentInChildren<Text>().text = letter; // Set button text to the letter
            button.onClick.AddListener(() => OnLetterButtonClick(button)); // Add click listener
        }
    }

    /// <summary>
    /// Shuffles a list in place.
    /// </summary>
    /// <param name="list">The list to be shuffled.</param>
    private void ShuffleList(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); // Get a random index
            string temp = list[i]; // Temporarily store the value to swap
            list[i] = list[j]; // Swap values
            list[j] = temp; // Continue the shuffle
        }
    }

    /// <summary>
    /// Shuffles an array in place.
    /// </summary>
    /// <param name="array">The array to be shuffled.</param>
    private void ShuffleArray(QuestionData[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); // Get a random index
            QuestionData temp = array[i]; // Temporarily store the current item
            array[i] = array[j]; // Swap the current with the random one
            array[j] = temp; // Continue the shuffle
        }
    }

    /// <summary>
    /// Handles the event when a letter button is clicked.
    /// </summary>
    /// <param name="clickedButton">The button that was clicked.</param>
    public void OnLetterButtonClick(Button clickedButton)
    {
        if (buttonClickClip != null)
        {
            audioSource.PlayOneShot(buttonClickClip); // Play click audio
        }

        string letter = clickedButton.GetComponentInChildren<Text>().text; // Get the letter from the button

        // Attempt to add the letter to answer slots
        if (!AddLetterToAnswer(letter, answerSlotContainer1, clickedButton) &&
            !AddLetterToAnswer(letter, answerSlotContainer2, clickedButton) &&
            !AddLetterToAnswer(letter, answerSlotContainer3, clickedButton))
        {
            return; // If no slots were filled, exit the method
        }

        clickedButton.interactable = false; // Disable button after use
    }

    /// <summary>
    /// Adds a letter to the answer in the specified container if possible.
    /// </summary>
    /// <param name="letter">The letter to add.</param>
    /// <param name="container">The answer container to check.</param>
    /// <param name="clickedButton">The button associated with this letter.</param>
    /// <returns>True if the letter was added; otherwise, false.</returns>
    private bool AddLetterToAnswer(string letter, Transform container, Button clickedButton)
    {
        foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
        {
            if (string.IsNullOrEmpty(inputField.text)) // Check if the input field is empty
            {
                inputField.text = letter; // Populate the input field with the letter
                inputField.interactable = false; // Lock the field

                // Store the button reference
                inputFieldButtonMap[inputField] = clickedButton; // Map input field to the button

                return true; // Letter added successfully
            }
        }
        return false; // No empty input field found
    }

    /// <summary>
    /// Displays trivia related to the current question.
    /// </summary>
    private void ShowTrivia()
    {
        triviaText.text = currentQuestion.trivia; // Display trivia text
        triviaModal.SetActive(true); // Show the trivia modal
    }

    /// <summary>
    /// Closes the trivia modal and shows the continue panel.
    /// </summary>
    private void CloseTrivia()
    {
        triviaModal.SetActive(false); // Hide trivia modal
        continuePanel.SetActive(true); // Show continue panel for next action
    }

    /// <summary>
    /// Checks the player's answer against the correct answer.
    /// </summary>
    private void CheckAnswer()
    {
        string playerInput = "";

        // Construct player input from answer slots
        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 })
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                playerInput += inputField.text.Trim(); // Trim and concatenate texts
            }
        }

        string normalizedPlayerInput = playerInput.ToUpper().Replace(" ", ""); // Normalize player input
        string normalizedCorrectWord = currentQuestion.correctWord.ToUpper().Replace(" ", ""); // Normalize correct answer

        if (normalizedPlayerInput.Equals(normalizedCorrectWord)) // Compare inputs
        {
            PlayAudio(correctAnswerClip); // Play correct answer audio
            UpdateScore(10); // Update score for correct answer
            questionsAnswered++; // Increment the number of questions answered

            ShowTrivia(); // Display trivia for the answered question
        }
        else
        {
            PlayAudio(wrongAnswerClip); // Play wrong answer audio
            HighlightIncorrectAnswer(); // Highlight incorrect answer
            ClearAnswerSlots(); // Clear input fields for the next question
        }
    }

    /// <summary>
    /// Highlights input fields for incorrect answers.
    /// </summary>
    private void HighlightIncorrectAnswer()
    {
        PlayAudio(wrongAnswerClip); // Play wrong answer audio
        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 })
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                inputField.image.color = Color.red; // Change the border color to red
            }
        }

        // Reset color after a delay (optional)
        StartCoroutine(ResetInputFieldColors()); // Start the color resetting coroutine
    }

    /// <summary>
    /// Coroutine to reset the color of input fields after a delay.
    /// </summary>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator ResetInputFieldColors()
    {
        PlayAudio(wrongAnswerClip); // Play wrong answer audio
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds

        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 })
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                inputField.image.color = Color.black; // Reset border color to black
            }
        }
    }

    /// <summary>
    /// Plays a specified audio clip.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    private void PlayAudio(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip); // Play audio clip
        }
    }

    /// <summary>
    /// Clears all answer slots for the next question.
    /// </summary>
    public void ClearAnswerSlots()
    {
        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 })
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                if (lockedHintFields.Contains(inputField))
                {
                    continue; // Skip clearing hinted letters
                }

                inputField.text = ""; // Clear manually entered letters

                // Restore the associated letter button if it exists
                if (inputFieldButtonMap.ContainsKey(inputField))
                {
                    Button associatedButton = inputFieldButtonMap[inputField];
                    if (associatedButton != null)
                    {
                        associatedButton.interactable = true; // Make button interactable again
                    }
                    inputFieldButtonMap.Remove(inputField); // Remove the mapping
                }
            }
        }
    }

    /// <summary>
    /// Updates the player's score based on the increment value.
    /// </summary>
    /// <param name="increment">The amount to change the score by.</param>
    public void UpdateScore(int increment)
    {
        score += increment; // Update score
        scoreText.text = score.ToString(); // Update score display
        PlayerPrefs.SetInt("PlayerScore", score); // Save the score
        PlayerPrefs.Save(); // Confirm the saved data
    }

    /// <summary>
    /// Shows the game over panel and displays the final score.
    /// </summary>
    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true); // Show game over panel
        finalScoreText.text = $"Final Score: {score}"; // Display final score
    }

    /// <summary>
    /// Continues the game after answering a question.
    /// </summary>
    private void ContinueGame()
    {
        continuePanel.SetActive(false); // Hide continue panel
        ResetGame(); // Start the next question
    }

    /// <summary>
    /// Exits the game and optionally returns to main menu.
    /// </summary>
    private void ExitGame()
    {
        continuePanel.SetActive(false); // Hide continue panel
        Debug.Log("Game Over - Exiting to Main Menu"); // Log exit to console
        // Implement logic to return to main menu or exit the game  
    }
}