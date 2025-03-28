using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public Sprite questionImage;
        public string[] options;
        public int correctAnswerIndex;
    }

    [Header("Quiz Setup")]
    public List<Question> quizzes = new List<Question>();
    public float timerDuration = 30f;
    public string nextSceneName = "MainMenu";

    [Header("UI Elements")]
    public Text questionText;
    public Image questionImage;
    public Button[] optionButtons;
    public Text timerText;
    public Text scoreText;
    public GameObject resultPanel;
    public Text finalScoreText;
    public Button exitButton;

    // Game state variables
    private int currentQuestionIndex = 0;
    private int score = 0;
    private float timer;
    private bool isQuizActive = true;
    void Start()
    {
        // Error checking
        if (quizzes.Count == 0)
        {
            Debug.LogError("No questions assigned to the QuizManager!");
            return;
        }

        // Load the previous total score from PlayerPrefs
        score = PlayerPrefs.GetInt("PlayerScore", 0);

        // Shuffle the questions list
        ShuffleList(quizzes);

        // Initialize game state
        timer = timerDuration;
        currentQuestionIndex = 0;
        isQuizActive = true;

        // Initialize UI
        UpdateScoreText();
        resultPanel.SetActive(false);

        // Set up exit button listener
        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(ExitGame);
        }

        // Start the quiz
        LoadQuestion();
        StartCoroutine(TimerCountdown());
    }

    void Update()
    {
        // You can add additional update logic here if needed
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex < quizzes.Count)
        {
            // Reset timer for new question
            timer = timerDuration;

            // Get current question
            Question q = quizzes[currentQuestionIndex];

            // Set question text
            if (questionText != null)
                questionText.text = q.questionText;

            // Set question image if available
            if (questionImage != null)
            {
                questionImage.sprite = q.questionImage;
                questionImage.gameObject.SetActive(q.questionImage != null);
            }

            // Reset all button colors
            ResetButtonColors();

            // Shuffle options while keeping track of correct answer
            List<string> shuffledOptions = new List<string>(q.options);
            ShuffleList(shuffledOptions);

            int newCorrectIndex = shuffledOptions.IndexOf(q.options[q.correctAnswerIndex]);

            // Set up option buttons
            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (i < shuffledOptions.Count)
                {
                    optionButtons[i].gameObject.SetActive(true);
                    optionButtons[i].GetComponentInChildren<Text>().text = shuffledOptions[i];

                    int index = i;
                    optionButtons[i].onClick.RemoveAllListeners();
                    optionButtons[i].onClick.AddListener(() => CheckAnswer(index, newCorrectIndex));
                }
                else
                {
                    optionButtons[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // All questions completed
            ShowFinalScore();
        }
    }

    void CheckAnswer(int selectedIndex, int correctIndex)
    {
        if (!isQuizActive) return;

        if (selectedIndex == correctIndex)
        {
            // Correct answer
            Debug.Log("✅ Correct Answer!");
            score++;
            UpdateScoreText();
            optionButtons[selectedIndex].GetComponent<Image>().color = Color.green;
        }
        else
        {
            // Wrong answer
            Debug.Log("❌ Wrong Answer!");
            optionButtons[selectedIndex].GetComponent<Image>().color = Color.red;

            // Highlight correct answer
            optionButtons[correctIndex].GetComponent<Image>().color = Color.green;
        }

        // Move to next question after delay
        StartCoroutine(NextQuestionDelay());
    }

    IEnumerator NextQuestionDelay()
    {
        isQuizActive = false;
        yield return new WaitForSeconds(1.5f);

        currentQuestionIndex++;

        if (currentQuestionIndex < quizzes.Count)
        {
            LoadQuestion();
            isQuizActive = true;
        }
        else
        {
            ShowFinalScore();
        }
    }

    IEnumerator TimerCountdown()
    {
        while (isQuizActive && timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerText();
            yield return null;
        }

        if (timer <= 0 && isQuizActive)
        {
            Debug.Log("⏳ Time's up!");
            ShowFinalScore();
        }
    }

    void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.Ceil(timer).ToString();
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        // Save the accumulated score to PlayerPrefs
        PlayerPrefs.SetInt("PlayerScore", score);
        PlayerPrefs.Save();

        // Update DisplayScore UI
        if (DisplayScore.instance != null)
        {
            DisplayScore.instance.UpdateScoreUI();
        }
    }
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }



    void ShowFinalScore()
    {
        isQuizActive = false;
        StopAllCoroutines();

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Your Score: " + score;
        }
    }

    void ExitGame()
    {
        Debug.Log("Exiting to " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }

    void ResetButtonColors()
    {
        foreach (Button btn in optionButtons)
        {
            btn.GetComponent<Image>().color = Color.white;
        }
    }
}