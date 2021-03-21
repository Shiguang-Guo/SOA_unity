using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
#pragma warning disable 649
    //ref to the QuizGameUI script
    [SerializeField] private QuizGameUI quizGameUI;

    //ref to the scriptableobject file
    [SerializeField] private List<QuizDataScriptable> quizDataList; // 各种类型问题的集合的一个list
    [SerializeField] private List<GameMode> modeDataList; // 游戏模式的list
    [SerializeField] private float timeInSeconds; // 剩余时间
#pragma warning restore 649

    private string currentCategory = "";

    private int correctAnswerCount = 0; // 正确答案数量

    //questions data
    private List<Question> questions;

    //current question data
    private Question selectedQuetion = new Question();
    private int gameScore;
    private int lifesRemaining;
    private float currentTime;
    private QuizDataScriptable dataScriptable;

    private GameStatus gameStatus = GameStatus.NEXT;

    public GameStatus GameStatus => gameStatus;

    public List<QuizDataScriptable> QuizData => quizDataList;

    public List<GameMode> ModeData => modeDataList;
    
    // 相当于给private变量设置了修改接口

    public void StartGame(int categoryIndex, string category)
    {
        currentCategory = category; // 设置游戏类型
        correctAnswerCount = 0;
        gameScore = 0;
        lifesRemaining = 3; // 初始化游戏信息
        currentTime = timeInSeconds;
        //set the questions data
        questions = new List<Question>();
        dataScriptable = quizDataList[categoryIndex];
        questions.AddRange(dataScriptable.questions); // 看起来像是一个从库函数里面抽取一个列表的函数
        //select the question
        SelectQuestion();
        gameStatus = GameStatus.PLAYING; // 修改游戏状态
    }

    /// <summary>
    /// Method used to randomly select the question form questions data
    /// </summary>
    private void SelectQuestion()
    {
        //get the random number
        int val = UnityEngine.Random.Range(0, questions.Count);
        //set the selectedQuetion
        selectedQuetion = questions[val];
        //send the question to quizGameUI
        quizGameUI.SetQuestion(selectedQuetion);

        questions.RemoveAt(val);
    }

    private void Update() // 看起来像是一个会被定期调用的函数，用来更新游戏时间
    {
        if (gameStatus == GameStatus.PLAYING)
        {
            currentTime -= Time.deltaTime;
            SetTime(currentTime);
        }
    }

    void SetTime(float value) // 结合Update函数进行游戏进度的监控
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime); //set the time value
        quizGameUI.TimerText.text = time.ToString("mm':'ss"); //convert time to Time format

        if (currentTime <= 0)
        {
            //Game Over
            GameEnd();
        }
    }

    /// <summary>
    /// Method called to check the answer is correct or not
    /// </summary>
    /// <param name="selectedOption">answer string</param>
    /// <returns></returns>
    public bool Answer(string selectedOption)
    {
        //set default to false
        bool correct = false;
        //if selected answer is similar to the correctAns
        if (selectedQuetion.correctAns == selectedOption)
        {
            //Yes, Ans is correct
            correctAnswerCount++;
            correct = true;
            gameScore += 50;
            quizGameUI.ScoreText.text = "Score:" + gameScore;
        }
        else
        {
            //No, Ans is wrong
            //Reduce Life
            lifesRemaining--;
            quizGameUI.ReduceLife(lifesRemaining); // 扣生命值

            if (lifesRemaining == 0)
            {
                GameEnd();
            }
        }

        if (gameStatus == GameStatus.PLAYING) // 某一题回答结束 生命没有用完 则加载下一题
        {
            if (questions.Count > 0)
            {
                //call SelectQuestion method again after 1s
                Invoke("SelectQuestion", 0.4f);
            }
            else
            {
                GameEnd();
            }
        }

        //return the value of correct bool
        return correct; 
    }

    private void GameEnd()
    {
        gameStatus = GameStatus.NEXT;
        quizGameUI.GameOverPanel.SetActive(true);

        //fi you want to save only the highest score then compare the current score with saved score and if more save the new score
        //eg:- if correctAnswerCount > PlayerPrefs.GetInt(currentCategory) then call below line

        //Save the score
        PlayerPrefs.SetInt(currentCategory, correctAnswerCount); //save the score for this category
    }
}

//Data structure for storing the questions data
[System.Serializable]
public class Question
{
    public string questionInfo; //question text
    public QuestionType questionType; //type
    public Sprite questionImage; //image for Image Type
    public AudioClip audioClip; //audio for audio type
    public UnityEngine.Video.VideoClip videoClip; //video for video type
    public List<string> options; //options to select
    public string correctAns; //correct option
}

[System.Serializable]
public enum QuestionType
{
    TEXT,
    IMAGE,
    AUDIO,
    VIDEO
}

[SerializeField]
public enum GameStatus
{
    PLAYING,
    NEXT
}