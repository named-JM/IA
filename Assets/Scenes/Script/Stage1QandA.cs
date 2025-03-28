using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizEasyManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers;
        public int correctAnswerIndex;
        public Sprite questionImage;  // For displaying images in the white area
    }

    // UI References
    public Text questionText;
    public Button[] answerButtons;
    public Text timerText;
    public Text scoreText;
    public Image questionImageDisplay; // Reference to the white box area
    public Text titleText;  // Reference to "Quiz Easy" text

    // Game variables
    private int currentQuestionIndex;
    private int score = 0;
    private float timeRemaining = 60;  // 60 seconds timer
    private bool timerIsRunning = false;
    public List<Question> questions;

    void Start()
    {
        // Set up the title
        if (titleText != null)
            titleText.text = "Quiz Easy";

        LoadQuestions();
        ResetQuiz();
    }

    void Update()
    {
        // Update timer
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                ShowQuizComplete();
            }
        }
    }

    void ResetQuiz()
    {
        currentQuestionIndex = 0;
        score = 0;
        timeRemaining = 60;
        timerIsRunning = true;
        UpdateScoreDisplay();
        UpdateTimerDisplay();
        LoadQuestion();
    }

    void UpdateTimerDisplay()
    {
        int seconds = Mathf.FloorToInt(timeRemaining);
        timerText.text = "Time: " + seconds.ToString();
    }

    void UpdateScoreDisplay()
    {
        scoreText.text = score.ToString();
    }

    void LoadQuestions()
    {
        questions = new List<Question>
        {
            new Question
            {
                questionText = "What is the old name of Indang?",
                answers = new string[] { "Indang Cacarong", "Indang Cavite", "Indung", "Indan" },
                correctAnswerIndex = 0
            },
            new Question
            {
                questionText = "It is a wedding venue that located in?",
                answers = new string[] { "GUYAM MALAKI", "BANABA CERCA", "DAINE II", "CALUMPANG CERCA" },
                correctAnswerIndex = 0
            },
            new Question
            {
                questionText = "The California breeze is a?",
                answers = new string[] { "RESORT", "HOTEL", "CAMPGROUND", "RENTAL HOUSE" },
                correctAnswerIndex = 2
            },
            new Question
            {
                questionText = "This is a hidden gem of Indang Cavite known as?",
                answers = new string[] { "THE RAVINE", "THE RAVINE BY ONE SELAH", "THE RAVENA", "THE RAVINE BY SNOOP DOG" },
                correctAnswerIndex = 1
            },
            new Question
            {
                questionText = "It is 13 miles from People's Park in the Sky. This property offers access to a terrace, free private parking, and free WiFi.",
                answers = new string[] { "REA'S GARDEN", "CARASUCHI VILLA GARDEN AND BUNGALOWS", "ROMILLIAS FARM", "LVG PARADISE RESORT AND EVENTS" },
                correctAnswerIndex = 3
            },
            new Question
            {
                questionText = "The Sadati forest is located in?",
                answers = new string[] { "Alpayan, Eastwest Road, Daine 1, Indang, Cavite", "Daine 1 Barangay Hall", "B. Avilla St. Indang", "East – West Road, Indang, Cavite" },
                correctAnswerIndex = 0
            },
            new Question
            {
                questionText = "What year was Driftwoods Action Park established?",
                answers = new string[] { "2010", "2020", "2014", "2025" },
                correctAnswerIndex = 2
            },
            new Question
            {
                questionText = "It is a relaxing getaway, fun activities, and scenic views called_____?",
                answers = new string[] { "La Casa De Serenidad", "Canopy Farm PH", "Buklod Cabins", "Isabelita Resort" },
                correctAnswerIndex = 0
            },
            new Question
            {
                questionText = "The municipality of Indang Cavite is known for?",
                answers = new string[] { "Fishing equipment and boat making", "Christmas lights and decorations", "Valenciana festival and karakol dancing", "Agricultural productivity and tourism" },
                correctAnswerIndex = 3
            },
            new Question
            {
                questionText = "This was established as a mission station of Father Angelo Armano in 1611 under the advocacy of_______?",
                answers = new string[] { "St. Gregory the Great", "St. Miguel", "St. Gregory Academy", "St. Jude" },
                correctAnswerIndex = 0
            },
            new Question
            {
                questionText = "What is the name of this place?",
                answers = new string[] { "Tropang Alak Singko Agus-os", "Barrio Lejos Grill and Resto Bar", "Wawu's Restobar", "None of the above" },
                correctAnswerIndex = 1
            },
            new Question
            {
                questionText = "Does The Ranch by Fresca Farm offer samgyupsal on the farm?",
                answers = new string[] { "Yes", "Maybe", "No", "I don't know" },
                correctAnswerIndex = 0
            },
            new Question
            {
                questionText = "What can you do at Purplestar Camp?",
                answers = new string[] { "Fishing", "Camping", "Nothing", "Fighting" },
                correctAnswerIndex = 1
            },
            new Question
            {
                questionText = "This is a poultry farm located in Indang Cavite.",
                answers = new string[] { "Backyard Menifarm", "Parello's Poultry Farm", "Sulucan Poultry Farm", "Rosy Chicks Poultry Farm" },
                correctAnswerIndex = 3
            },
            new Question
            {
                questionText = "What is the name of this statue?",
                answers = new string[] { "Indang Town Plaza Monument", "Statue of Liberty", "Samonte Park Statue", "Dr. Jose Rizal Monument" },
                correctAnswerIndex = 0
            },
            new Question
            {
                questionText = "What products does Housegem Mushroom Farm offer?",
                answers = new string[] { "Mango", "Apple", "Orange", "Mushroom" },
                correctAnswerIndex = 3
            }
        };
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex < questions.Count)
        {
            Question q = questions[currentQuestionIndex];
            questionText.text = q.questionText;

            // If you have an image for the question, display it
            if (q.questionImage != null && questionImageDisplay != null)
            {
                questionImageDisplay.sprite = q.questionImage;
                questionImageDisplay.gameObject.SetActive(true);
            }
            else if (questionImageDisplay != null)
            {
                // Set a default white image or make it blank
                questionImageDisplay.color = Color.white;
                questionImageDisplay.gameObject.SetActive(true);
            }

            // Update answer buttons
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (i < q.answers.Length)
                {
                    answerButtons[i].gameObject.SetActive(true);
                    answerButtons[i].GetComponentInChildren<Text>().text = q.answers[i];
                    int index = i;  // Avoid closure issue
                    answerButtons[i].onClick.RemoveAllListeners();
                    answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
                }
                else
                {
                    // Hide extra buttons if we have fewer than 4 answers
                    answerButtons[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            ShowQuizComplete();
        }
    }

    void ShowQuizComplete()
    {
        timerIsRunning = false;
        questionText.text = "Quiz Complete!\nYour Score: " + score;

        // Hide answer buttons
        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        // Optionally add a restart button
        // You could activate a pre-created restart button here
    }

    void CheckAnswer(int index)
    {
        if (index == questions[currentQuestionIndex].correctAnswerIndex)
        {
            Debug.Log("Correct!");
            score += 10;  // Add points for correct answer
            UpdateScoreDisplay();
        }
        else
        {
            Debug.Log("Wrong!");
        }

        currentQuestionIndex++;
        LoadQuestion();
    }
}