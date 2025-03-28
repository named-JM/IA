using UnityEngine;

[CreateAssetMenu(fileName = "NewQuiz", menuName = "Quiz/New Quiz")]
public class QuizData : ScriptableObject
{
    public string question;
    public Sprite questionImage;
    public string[] options;
    public int correctAnswerIndex;
}
