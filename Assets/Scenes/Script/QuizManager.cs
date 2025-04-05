using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// QuizManager is responsible for handling the quiz game logic including question loading, answer checking, score tracking, and UI updates.
/// </summary>
public class QuizManager : MonoBehaviour
{
    // Represents a question structure for the quiz
    [System.Serializable]
    public class Question
    {
        public string questionText; // Text of the question
        public Sprite questionImage; // Image associated with the question
        public string[] options; // Multiple choice options for the question
        public int correctAnswerIndex; // Index of the correct answer in options
        public bool isHard; // Indicates if the question is hard
    }

    [Header("Quiz Setup")]
    public List<Question> quizzes = new List<Question>(); // List of questions in the quiz
    public float timerDuration = 30f; // Duration for the timer per question
    public string nextSceneName = "MainMenu"; // Scene to load when the quiz is exited

    [Header("UI Elements")]
    public Text questionText; // UI text for displaying the question
    public Image questionImage; // UI image to show an associated image for the question
    public Button[] optionButtons; // Array of buttons for answer options
    public Text timerText; // Display for the quiz timer
    public Text scoreText; // Display for the player's score
    public GameObject resultPanel; // Panel to show results at the end of the quiz
    public GameObject continueExitPanel; // Panel with options to continue to the next question or exit
    public Text finalScoreText; // Text to display final score

    // Continue panel after every question answered
    public GameObject continuePanel; // Panel visible after each question to continue
    public Button continueQuestionButton; // Button to continue to the next question
    public Button exitQuestionButton; // Button to exit the quiz

    [Header("Audio Clips")]
    public AudioSource audioSource; // Audio source to play sounds
    public AudioClip correctSound; // Sound played on a correct answer
    public AudioClip wrongSound; // Sound played on a wrong answer

    private int currentQuestionIndex = 0; // Tracks the index of the current question
    private int score = 0; // Player's current score
    private float timer; // Countdown timer for the current question
    private bool isQuizActive = true; // Status to check if the quiz is ongoing

    // List of words to be italicized
    private List<string> wordsToItalicize = new List<string> { "indang" };

    /// <summary>
    /// Initializes the quiz manager, sets up UI, and loads the first question.
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component

        // Check if quizzes have been assigned
        if (quizzes.Count == 0)
        {
            Debug.LogError("No questions assigned to the QuizManager!");
            return;
        }

        score = PlayerPrefs.GetInt("PlayerScore", 0); // Load the player's saved score
        ShuffleList(quizzes); // Shuffle the list of questions for randomness

        // Initialize timer and question index for the quiz
        timer = timerDuration;
        currentQuestionIndex = 0;
        isQuizActive = true;

        UpdateScoreText(); // Update the UI score display
        resultPanel.SetActive(false); // Hide the results panel initially
        continueExitPanel.SetActive(false); // Hide exit options initially
        continuePanel.SetActive(false); // Hide the continue panel initially

        // Set up continue question button listener
        if (continueQuestionButton != null)
        {
            continueQuestionButton.onClick.RemoveAllListeners();
            continueQuestionButton.onClick.AddListener(ContinueToNextQuestion);
        }

        // Set up exit question button listener
        if (exitQuestionButton != null)
        {
            exitQuestionButton.onClick.RemoveAllListeners();
            exitQuestionButton.onClick.AddListener(ExitGame);
        }

        LoadQuestion(); // Load the first question
        StartCoroutine(TimerCountdown()); // Start the timer countdown
    }


    /// <summary>
    /// Italicizes specific words in the given question text.
    /// </summary>
    /// <param name="questionText">The original question text.</param>
    /// <returns>The question text with specified words italicized.</returns>
    private string ItalicizeSpecificWords(string inputText)
    {
        foreach (string word in wordsToItalicize)
        {
            string pattern = $@"\b{word}\b";
            string replacement = $"<i>{word.ToUpper()}</i>";
            inputText = System.Text.RegularExpressions.Regex.Replace(
                inputText,
                pattern,
                replacement,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }
        return inputText;
    }


    /// <summary>
    /// Loads the current question and sets the UI for the question text, image, and answer options.
    /// </summary>
    void LoadQuestion()
    {
        // Check if all questions have been answered
        if (currentQuestionIndex < quizzes.Count)
        {
            timer = timerDuration; // Reset the timer for the new question
            Question q = quizzes[currentQuestionIndex]; // Get the current question
            // Italicize specific words in the question text and capitalized all
            string formattedQuestionText = ItalicizeSpecificWords(q.questionText.ToUpper());
            questionText.text = formattedQuestionText;


            // Update UI with the current question's text and image
            questionText.text = q.questionText;
            questionImage.sprite = q.questionImage;
            questionImage.gameObject.SetActive(q.questionImage != null);

            ResetButtonColors(); // Reset button colors before new question

            // Shuffle options for the current question
            List<string> shuffledOptions = new List<string>(q.options);
            ShuffleList(shuffledOptions); // Shuffle the options to randomize order
            int newCorrectIndex = shuffledOptions.IndexOf(q.options[q.correctAnswerIndex]); // Get new index of the correct answer

            // Assign button texts and listeners
            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (i < shuffledOptions.Count)
                {
                    // Set up option button for displayed option
                    optionButtons[i].gameObject.SetActive(true);
                    string optionText = shuffledOptions[i].ToUpper(); // Make entire option uppercase

                    // Italicize specific words even inside options
                    optionText = ItalicizeSpecificWords(optionText);

                    optionButtons[i].GetComponentInChildren<Text>().text = optionText;

                    int index = i; // Capture index for lambda expression
                    optionButtons[i].onClick.RemoveAllListeners(); // Remove any existing listeners
                    optionButtons[i].onClick.AddListener(() => CheckAnswer(index, newCorrectIndex)); // Add listener to check answer
                }
                else
                {
                    optionButtons[i].gameObject.SetActive(false); // Hide buttons if there are fewer options
                }
            }
        }
        else
        {
            ShowFinalScore(); // Show final score if all questions have been answered
        }
    }

    /// <summary>
    /// Checks if the player's selected answer is correct and updates the score and UI accordingly.
    /// </summary>
    /// <param name="selectedIndex">Index of the selected answer.</param>
    /// <param name="correctIndex">Index of the correct answer.</param>
    void CheckAnswer(int selectedIndex, int correctIndex)
    {
        if (!isQuizActive) return; // Early return if quiz is not active

        if (selectedIndex == correctIndex) // Check if the answer is correct
        {
            audioSource.PlayOneShot(correctSound); // Play the correct answer sound

            // Update score based on question difficulty
            score += quizzes[currentQuestionIndex].isHard ? 20 : 10; // Add appropriate points to score based on question difficulty

            optionButtons[selectedIndex].GetComponent<Image>().color = Color.green; // Change button color to green to indicate correct answer
        }
        else // Handling wrong answer
        {
            audioSource.PlayOneShot(wrongSound); // Play the wrong answer sound
            optionButtons[selectedIndex].GetComponent<Image>().color = Color.red; // Change button color to red to indicate wrong answer
            optionButtons[correctIndex].GetComponent<Image>().color = Color.green; // Show the correct answer
        }

        // Update score UI immediately
        UpdateScoreText();

        StartCoroutine(ShowContinuePanel()); // Show continue panel after a delay
    }

    /// <summary>
    /// Coroutine that shows the continue panel after a delay following an answer.
    /// </summary>
    IEnumerator ShowContinuePanel()
    {
        isQuizActive = false; // Mark the quiz as inactive
        yield return new WaitForSeconds(1.5f); // Wait for 1.5 seconds before showing continue panel
        continuePanel.SetActive(true); // Activate the continue panel
    }

    /// <summary>
    /// Continues to the next question or shows the final score if the quiz is over.
    /// </summary>
    void ContinueToNextQuestion()
    {
        continuePanel.SetActive(false); // Hide the continue panel
        currentQuestionIndex++; // Move to the next question

        if (currentQuestionIndex < quizzes.Count) // Check if there are more questions
        {
            LoadQuestion(); // Load the next question
            isQuizActive = true; // Mark the quiz as active
        }
        else
        {
            ShowFinalScore(); // Show final score if all questions have been answered
            resultPanel.SetActive(true); // Ensure the results panel is shown
            continueExitPanel.SetActive(false); // Hide the continue panel
        }
    }

    /// <summary>
    /// Coroutine that counts down the timer for the current question.
    /// </summary>
    IEnumerator TimerCountdown()
    {
        while (isQuizActive && timer > 0) // Count down while quiz active and timer not up
        {
            timer -= Time.deltaTime; // Decrease timer
            UpdateTimerText(); // Update timer UI
            yield return null; // Wait for the next frame
        }

        if (timer <= 0 && isQuizActive) // Check if timer has reached zero
        {
            isQuizActive = false; // Mark quiz as inactive
            ShowFinalScore(); // Show final score when the timer runs out
        }
    }

    /// <summary>
    /// Updates the timer UI text to show remaining time.
    /// </summary>
    void UpdateTimerText()
    {
        if (timerText != null) // Ensure timer text component is assigned
        {
            // Update the displayed timer text
            timerText.text = "Time: " + Mathf.Ceil(timer).ToString();
        }
    }

    /// <summary>
    /// Updates the player's score displayed in the UI and saves to PlayerPrefs.
    /// </summary>
    void UpdateScoreText()
    {
        if (scoreText != null) // Ensure score text component is assigned
        {
            scoreText.text = score.ToString(); // Update score display to reflect current score
        }

        PlayerPrefs.SetInt("PlayerScore", score); // Save score to PlayerPrefs
        PlayerPrefs.Save(); // Confirm saving PlayerPrefs changes
    }

    /// <summary>
    /// Shuffles the elements in a list using Fisher-Yates algorithm.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">List of elements to be shuffled.</param>
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i]; // Temporarily store the current item
            int randomIndex = Random.Range(i, list.Count); // Get a random index
            list[i] = list[randomIndex]; // Shuffle the values
            list[randomIndex] = temp; // Swap values
        }
    }

    /// <summary>
    /// Displays the final score and stops the quiz.
    /// </summary>
    void ShowFinalScore()
    {
        isQuizActive = false; // Mark the quiz as inactive
        StopAllCoroutines(); // Stop the timer countdown
        resultPanel.SetActive(true); // Show the results panel
        continueExitPanel.SetActive(false); // Hide unnecessary panels

        if (finalScoreText != null) // Check if final score text component is assigned
        {
            finalScoreText.text = "Your Score: " + score; // Display final score
        }
    }

    /// <summary>
    /// Exits the quiz and loads the specified scene, returning to the main menu.
    /// </summary>
    void ExitGame()
    {
        SceneManager.LoadScene(nextSceneName); // Load the next scene
    }

    /// <summary>
    /// Resets the colors of option buttons to default (white).
    /// </summary>
    void ResetButtonColors()
    {
        foreach (Button btn in optionButtons) // Iterate through each option button
        {
            btn.GetComponent<Image>().color = Color.white; // Reset button background color to white
        }
    }
}