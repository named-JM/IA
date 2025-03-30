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
        public bool isHard;
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
    public GameObject continueExitPanel;
    public Text finalScoreText;

    // Continue panel after every question answered
    public GameObject continuePanel;
    public Button continueQuestionButton;
    public Button exitQuestionButton;

    [Header("Audio Clips")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    private int currentQuestionIndex = 0;
    private int score = 0;
    private float timer;
    private bool isQuizActive = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (quizzes.Count == 0)
        {
            Debug.LogError("No questions assigned to the QuizManager!");
            return;
        }

        score = PlayerPrefs.GetInt("PlayerScore", 0);
        ShuffleList(quizzes);

        timer = timerDuration;
        currentQuestionIndex = 0;
        isQuizActive = true;

        UpdateScoreText();
        resultPanel.SetActive(false);
        continueExitPanel.SetActive(false);
        continuePanel.SetActive(false);

        if (continueQuestionButton != null)
        {
            continueQuestionButton.onClick.RemoveAllListeners();
            continueQuestionButton.onClick.AddListener(ContinueToNextQuestion);
        }

        if (exitQuestionButton != null)
        {
            exitQuestionButton.onClick.RemoveAllListeners();
            exitQuestionButton.onClick.AddListener(ExitGame);
        }

        LoadQuestion();
        StartCoroutine(TimerCountdown());
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex < quizzes.Count)
        {
            timer = timerDuration;
            Question q = quizzes[currentQuestionIndex];

            questionText.text = q.questionText;
            questionImage.sprite = q.questionImage;
            questionImage.gameObject.SetActive(q.questionImage != null);

            ResetButtonColors();

            List<string> shuffledOptions = new List<string>(q.options);
            ShuffleList(shuffledOptions);
            int newCorrectIndex = shuffledOptions.IndexOf(q.options[q.correctAnswerIndex]);

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
            ShowFinalScore();
        }
    }

    void CheckAnswer(int selectedIndex, int correctIndex)
    {
        if (!isQuizActive) return;

        if (selectedIndex == correctIndex)
        {
            audioSource.PlayOneShot(correctSound);

            // Fixing score calculation
            score += quizzes[currentQuestionIndex].isHard ? 20 : 10;

            optionButtons[selectedIndex].GetComponent<Image>().color = Color.green;
        }
        else
        {
            audioSource.PlayOneShot(wrongSound);
            optionButtons[selectedIndex].GetComponent<Image>().color = Color.red;
            optionButtons[correctIndex].GetComponent<Image>().color = Color.green;
        }

        // Update score UI immediately
        UpdateScoreText();

        StartCoroutine(ShowContinuePanel());
    }


    IEnumerator ShowContinuePanel()
    {
        isQuizActive = false;
        yield return new WaitForSeconds(1.5f);
        continuePanel.SetActive(true);
    }

    void ContinueToNextQuestion()
    {
        continuePanel.SetActive(false);
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

        PlayerPrefs.SetInt("PlayerScore", score);
        PlayerPrefs.Save();
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
        resultPanel.SetActive(false);
        continueExitPanel.SetActive(true);

        if (finalScoreText != null)
        {
            finalScoreText.text = "Your Score: " + score;
        }
    }

    void ExitGame()
    {
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
