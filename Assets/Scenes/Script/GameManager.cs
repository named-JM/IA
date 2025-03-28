using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Image[] questionImages; // Array of the 4 images for the current question
    public Button letterButtonPrefab;  // Prefab for letter buttons
    public Transform letterButtonContainer;  // Container for letter buttons
    public Transform answerSlotContainer1;  // Container for answer slots
    public Transform answerSlotContainer2;  // Container for answer slots
    public Transform answerSlotContainer3;  // Container for answer slots


    public int score = 0;
    public Text scoreText;             // Score Text
    public GameObject gameOverPanel;   // Game Over Panel
    public Text finalScoreText;        // Final Score Text

    public QuestionData[] allQuestions;  // Array of questions to choose from
    private QuestionData currentQuestion; // Current question
    private int questionIndex = 0;

    public GameObject inputFieldPrefab;

    public Button submitButton;
    private int questionsAnswered = 0;
    private int totalQuestions;

    public AudioClip correctAnswerClip;
    public AudioClip wrongAnswerClip;
    private AudioSource audioSource;
    public AudioClip buttonClickClip;

    //add on

    public GameObject triviaModal;  // Assign Trivia Panel
    public Text triviaText;         // Assign Trivia Text
    public Button closeTriviaButton; // Assign Close Button

    public GameObject hintPanel;  // Panel for hint confirmation
    public Button confirmHintButton; // Yes Button
    public Button cancelHintButton; // No Button
    public Text hintText; // Text on panel

    public GameObject noHintPanel; // Panel to show insufficient points message
    public Text noHintText; // Text inside the panel
    public Button noHintOkButton; // OK button


    // Dictionary to track which button corresponds to which input field
    private Dictionary<InputField, Button> inputFieldButtonMap = new Dictionary<InputField, Button>();




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ShuffleArray(allQuestions);
        totalQuestions = allQuestions.Length;

        score = PlayerPrefs.GetInt("PlayerScore", 0);  // Initialize score
        scoreText.text = score.ToString();
        submitButton.onClick.AddListener(CheckAnswer);
        closeTriviaButton.onClick.AddListener(CloseTrivia);
        triviaModal.SetActive(false); // Hide initially

        // Existing initialization
        confirmHintButton.onClick.AddListener(ConfirmHintPurchase);
        cancelHintButton.onClick.AddListener(CloseHintPanel);
        hintPanel.SetActive(false); // Hide hint panel initially

        noHintPanel.SetActive(false); // Hide the panel initially
        noHintOkButton.onClick.AddListener(CloseNoHintPanel);
        ResetGame();  // Start the first round
    }

    private void ResetGame()
    {
        if (questionsAnswered >= totalQuestions)
        {
            ShowGameOverPanel();
            return;
        }

        currentQuestion = GetNextQuestion();
        if (currentQuestion != null)
        {
            SetQuestionImages();
            SetupAnswerSlots();
            InstantiateLetterButtons();
        }
    }
    private HashSet<InputField> lockedHintFields = new HashSet<InputField>(); // Stores locked input fields

    private void ConfirmHintPurchase()
    {
        if (score >= 10)
        {
            hintPanel.SetActive(false);
            ApplyHint(); // Call ApplyHint() once to reveal a letter
        }
        else
        {
            noHintText.text = "Not enough points to use a hint!";
            noHintPanel.SetActive(true);
            hintPanel.SetActive(false);
        }
    }




    private void CloseHintPanel()
    {
        hintPanel.SetActive(false);
    }
    private void ApplyHint()
    {
        string correctAnswer = currentQuestion.correctWord.Replace(" ", "").ToUpper();
        char[] correctChars = correctAnswer.ToCharArray();

        List<InputField> emptySlots = new List<InputField>();
        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 })
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                if (string.IsNullOrEmpty(inputField.text))
                {
                    emptySlots.Add(inputField);
                }
            }
        }

        if (emptySlots.Count > 0)
        {
            // Pick a random empty slot
            InputField chosenSlot = emptySlots[Random.Range(0, emptySlots.Count)];

            // Find the correct index of this slot
            int index = GetSlotIndex(chosenSlot);

            if (index != -1) // Ensure we found the correct index
            {
                chosenSlot.text = correctChars[index].ToString();
                chosenSlot.interactable = false;
                lockedHintFields.Add(chosenSlot);

                // Disable the corresponding letter button
                foreach (Button button in letterButtonContainer.GetComponentsInChildren<Button>())
                {
                    if (button.GetComponentInChildren<Text>().text == correctChars[index].ToString() && button.interactable)
                    {
                        button.interactable = false;
                        break;
                    }
                }

                // Deduct score for using the hint
                UpdateScore(-10);
            }
        }
    }

    // Helper function to find the index of an input slot in the answer
    private int GetSlotIndex(InputField slot)
    {
        List<InputField> allSlots = new List<InputField>();
        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 })
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                allSlots.Add(inputField);
            }
        }

        return allSlots.IndexOf(slot);
    }

    private void CloseNoHintPanel()
    {
        noHintPanel.SetActive(false);
    }

    public void UseHint()
    {
        if (score < 10)
        {
            noHintText.text = "Not enough points to use a hint!";
            noHintPanel.SetActive(true);
            return;
        }

        // Open hint confirmation panel
        hintPanel.SetActive(true);
        hintText.text = "Use 10 points to reveal a letter?";
    }

    private QuestionData GetNextQuestion()
    {
        if (questionIndex < allQuestions.Length)
        {
            return allQuestions[questionIndex++];
        }
        return null; // No more questions
    }

    private void SetQuestionImages()
    {
        for (int i = 0; i < questionImages.Length; i++)
        {
            questionImages[i].sprite = currentQuestion.images[i];
        }
    }

    private void SetupAnswerSlots()
    {
        DestroyAnswerSlots(answerSlotContainer1);
        DestroyAnswerSlots(answerSlotContainer2);
        DestroyAnswerSlots(answerSlotContainer3);

        string[] words = currentQuestion.correctWord.Split(' ');

        if (words.Length > 0) SetupSlotsForWord(words[0], answerSlotContainer1);
        if (words.Length > 1) SetupSlotsForWord(words[1], answerSlotContainer2);
        if (words.Length > 2) SetupSlotsForWord(words[2], answerSlotContainer3);
    }

    private void DestroyAnswerSlots(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    
private void SetupSlotsForWord(string word, Transform container)
{
    foreach (char _ in word)
    {
        GameObject slot = Instantiate(inputFieldPrefab, container);
        InputField inputField = slot.GetComponent<InputField>();
        inputField.text = "";
        inputField.characterLimit = 1;
        inputField.image.color = Color.black;
        inputField.textComponent.color = Color.white;
        inputField.textComponent.alignment = TextAnchor.MiddleCenter;

        RectTransform rectTransform = inputField.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(50, 50);
        rectTransform.localPosition = Vector3.zero;

        inputField.interactable = false;

        // Add EventTrigger to detect click
        EventTrigger trigger = inputField.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown; // Detect click/tap
        entry.callback.AddListener((data) => { ClearInputField(inputField); });

        trigger.triggers.Add(entry);
    }
}

    // Function to clear the input field when clicked
    private void ClearInputField(InputField inputField)
    {
        if (lockedHintFields.Contains(inputField))
        {
            return; // Don't allow clearing if this field was filled by a hint
        }

        inputField.text = "";

        if (inputFieldButtonMap.ContainsKey(inputField))
        {
            Button associatedButton = inputFieldButtonMap[inputField];
            if (associatedButton != null)
            {
                associatedButton.interactable = true;
            }

            inputFieldButtonMap.Remove(inputField);
        }
    }





    private void InstantiateLetterButtons()
    {
        foreach (Transform child in letterButtonContainer)
        {
            Destroy(child.gameObject);
        }

        string correctAnswer = currentQuestion.correctWord.Replace(" ", "").ToUpper();

        List<string> lettersForButtons = correctAnswer.ToCharArray()
            .Select(c => c.ToString())
            .ToList();

        List<string> randomLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            .ToCharArray()
            .Select(c => c.ToString())
            .Where(letter => !lettersForButtons.Contains(letter))
            .OrderBy(_ => Random.value)
            .Take(5)
            .ToList();

        lettersForButtons.AddRange(randomLetters);
        ShuffleList(lettersForButtons);

        foreach (string letter in lettersForButtons)
        {
            GameObject newButton = Instantiate(letterButtonPrefab.gameObject, letterButtonContainer);
            Button button = newButton.GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = letter;
            button.onClick.AddListener(() => OnLetterButtonClick(button));
        }
    }

    private void ShuffleList(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private void ShuffleArray(QuestionData[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            QuestionData temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
    public void OnLetterButtonClick(Button clickedButton)
    {
        if (buttonClickClip != null)
        {
            audioSource.PlayOneShot(buttonClickClip);
        }

        string letter = clickedButton.GetComponentInChildren<Text>().text;

        if (!AddLetterToAnswer(letter, answerSlotContainer1, clickedButton) &&
            !AddLetterToAnswer(letter, answerSlotContainer2, clickedButton) &&
            !AddLetterToAnswer(letter, answerSlotContainer3, clickedButton))
        {
            return;
        }

        clickedButton.interactable = false; // Disable button once used
    }



    private bool AddLetterToAnswer(string letter, Transform container, Button clickedButton)
    {
        foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                inputField.text = letter;
                inputField.interactable = false;

                // Store the button reference
                inputFieldButtonMap[inputField] = clickedButton;

                return true;
            }
        }
        return false;
    }

    private void ShowTrivia()
    {
        triviaText.text = currentQuestion.trivia; // Assuming QuestionData has a trivia field
        triviaModal.SetActive(true);
    }
    private void CloseTrivia()
    {
        triviaModal.SetActive(false);
    }


    private void CheckAnswer()
    {
        string playerInput = "";

        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 })
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                playerInput += inputField.text.Trim();
            }
        }

        string normalizedPlayerInput = playerInput.ToUpper().Replace(" ", "");
        string normalizedCorrectWord = currentQuestion.correctWord.ToUpper().Replace(" ", "");

        if (normalizedPlayerInput.Equals(normalizedCorrectWord))
        {
            PlayAudio(correctAnswerClip);
            UpdateScore(10);
            questionsAnswered++;
            ShowTrivia();
            ResetGame();
        }
        else
        {
            PlayAudio(wrongAnswerClip);
            HighlightIncorrectAnswer(); // Add this function
            ClearAnswerSlots();
        }
    }

    private void HighlightIncorrectAnswer()
    {
        PlayAudio(wrongAnswerClip);
        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 })
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                inputField.image.color = Color.red; // Change the border color to red
            }
        }

        // Reset color after a delay (optional)
        StartCoroutine(ResetInputFieldColors());
    }
    private IEnumerator ResetInputFieldColors()
    {
        PlayAudio(wrongAnswerClip);
        yield return new WaitForSeconds(0.5f); // Wait 1.5 seconds

        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 })
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                inputField.image.color = Color.black; // Reset border color
            }
        }
    }


    private void PlayAudio(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }


    private void ClearAnswerSlots()
    {
        foreach (Transform container in new[] { answerSlotContainer1, answerSlotContainer2, answerSlotContainer3 })
        {
            foreach (InputField inputField in container.GetComponentsInChildren<InputField>())
            {
                inputField.text = "";
            }
        }

        foreach (Button button in letterButtonContainer.GetComponentsInChildren<Button>())
        {
            button.interactable = true;
        }
    }

    public void UpdateScore(int increment)
    {
        score += increment;
        scoreText.text = score.ToString();
        PlayerPrefs.SetInt("PlayerScore", score);
        PlayerPrefs.Save();
    }

    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = $"Final Score: {score}";
    }
}