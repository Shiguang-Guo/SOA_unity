using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using Random = System.Random;

public class QuizGameUI : MonoBehaviour // 主界面
{
#pragma warning disable 649
    [SerializeField] private QuizManager quizManager; //ref to the QuizManager script
    [SerializeField] private CategoryBtnScript categoryBtnPrefab;
    [SerializeField] private GameModeBtnScript modeBtnPrefab;
    [SerializeField] private LeaderLineScript leaderLinePrefab;
    [SerializeField] private GameObject scrollHolder, modelcontent;
    [SerializeField] private GameObject leaderboardcontent;
    [SerializeField] private Text scoreText, timerText;
    [SerializeField] private List<Image> lifeImageList;
    [SerializeField] private GameObject gameOverPanel, mainMenu, gamePanel, modelSelect, knowledgebase, againstmachine, leaderboard;
    [SerializeField] private Button retbtn, searchbtn, machine_submit_btn, machine_next_btn, machine_ret_btn, leaderboardbtn, localbtn;
    [SerializeField] private Text ansText, machineQuesText;
    [SerializeField] private Color correctCol, wrongCol, normalCol; //color of buttons
    [SerializeField] private Image questionImg; //image component to show image
    [SerializeField] private UnityEngine.Video.VideoPlayer questionVideo; //to show video
    [SerializeField] private AudioSource questionAudio; //audio source for audio clip
    [SerializeField] private Text questionInfoText; //text to show question
    [SerializeField] private List<Button> options; //options button reference
    [SerializeField] private InputField user_question, machine_user_ans, machine_bert_ans;
    [SerializeField] private Dropdown lang_model;
    [SerializeField] private MachineBattleQues machineBattleQues;
    [SerializeField] private InputField submit_input;
    [SerializeField] private List<LeaderLine> LeaderBoardData;


#pragma warning restore 649

    private float audioLength; //store audio length
    private Question question; //store current question data
    private bool answered = false; //bool to keep track if answered or not

    public Text TimerText => timerText; //getter
    public Text ScoreText => scoreText; //getter
    public GameObject GameOverPanel => gameOverPanel; //getter

    public MachineQues current_ques;


    [DllImport("__Internal")]
    private static extern void NotImplenmation();

    [DllImport("__Internal")]
    private static extern string getRequest(string ques, string model);

    [DllImport("__Internal")]
    private static extern string getLeaderBoard();

    [DllImport("__Internal")]
    private static extern void submitRanking(string user, string mode, string score);


    private void Start()
    {
        CreateGameModeBtns();
        FetchLeaderBoardData();
        CreateLeaderBoardLines();
    }

    private void CreateLeaderBoardLines()
    {
        foreach(var line in LeaderBoardData)
        {
            LeaderLineScript leaderline = Instantiate(leaderLinePrefab, leaderboardcontent.transform);
            leaderline.SetLine(line.UserName, line.ModeName, line.Score);
        }
    }

    private void CreateGameModeBtns()
    {
        foreach (var mode in quizManager.ModeData)
        {
            GameModeBtnScript modeBtn = Instantiate(modeBtnPrefab, modelcontent.transform);
            modeBtn.SetButton(mode.modeChineseName);
            modeBtn.Btn.onClick.AddListener(() => GameModeBtn(mode.modeName, mode.isopen));
        }
    }

    private void FetchLeaderBoardData()
    {
        LeaderBoardData = new List<LeaderLine>();
        string data = getLeaderBoard();
        string[] data_list = data.Split('\n');
        //Debug.Log("Fetching Leaderboard");
        foreach (var line in data_list) {
            if (line == "") break;
            string[] line_list = line.Split('\t');
            LeaderLine newline = new LeaderLine(line_list[0], line_list[1], line_list[2]);
            LeaderBoardData.Add(newline);
            // Debug.Log(line_list[0]);
            // Debug.Log(line_list[1]);
            // Debug.Log(line_list[2]);
        }
        // LeaderLine head = new LeaderLine("zsj", "history", "10");
        // LeaderLine a = new LeaderLine("gsg", "geography", "100");
        // LeaderBoardData.Add(head);
        // LeaderBoardData.Add(a);
    }
    private void StartLocalGame()
    {
        //add the listner to all the buttons
        for (int i = 0; i < options.Count; i++)
        {
            Button localBtn = options[i];
            localBtn.onClick.AddListener(() => OnClick(localBtn));
        }
        CreateCategoryButtons();
    }

    /// <summary>
    /// Method which populate the question on the screen
    /// </summary>
    /// <param name="question"></param>
    public void SetQuestion(Question question)
    {
        //set the question
        this.question = question;
        //check for questionType
        switch (question.questionType)
        {
            case QuestionType.TEXT:
                questionImg.transform.parent.gameObject.SetActive(false); //deactivate image holder
                break;
            case QuestionType.IMAGE:
                questionImg.transform.parent.gameObject.SetActive(true); //activate image holder
                questionVideo.transform.gameObject.SetActive(false); //deactivate questionVideo
                questionImg.transform.gameObject.SetActive(true); //activate questionImg
                questionAudio.transform.gameObject.SetActive(false); //deactivate questionAudio

                questionImg.sprite = question.questionImage; //set the image sprite
                break;
            case QuestionType.AUDIO:
                questionVideo.transform.parent.gameObject.SetActive(true); //activate image holder
                questionVideo.transform.gameObject.SetActive(false); //deactivate questionVideo
                questionImg.transform.gameObject.SetActive(false); //deactivate questionImg
                questionAudio.transform.gameObject.SetActive(true); //activate questionAudio

                audioLength = question.audioClip.length; //set audio clip
                StartCoroutine(PlayAudio()); //start Coroutine
                break;
            case QuestionType.VIDEO:
                questionVideo.transform.parent.gameObject.SetActive(true); //activate image holder
                questionVideo.transform.gameObject.SetActive(true); //activate questionVideo
                questionImg.transform.gameObject.SetActive(false); //deactivate questionImg
                questionAudio.transform.gameObject.SetActive(false); //deactivate questionAudio

                questionVideo.clip = question.videoClip; //set video clip
                questionVideo.Play(); //play video
                break;
        }

        questionInfoText.text = question.questionInfo; //set the question text

        //suffle the list of options
        List<string> ansOptions = ShuffleList.ShuffleListItems<string>(question.options);

        //assign options to respective option buttons
        for (int i = 0; i < options.Count; i++)
        {
            //set the child text
            options[i].GetComponentInChildren<Text>().text = ansOptions[i];
            options[i].name = ansOptions[i]; //set the name of button
            options[i].image.color = normalCol; //set color of button to normal
        }

        answered = false;
    }

    public void ReduceLife(int remainingLife)
    {
        lifeImageList[remainingLife].color = Color.red;
    }

    /// <summary>
    /// IEnumerator to repeate the audio after some time
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayAudio()
    {
        //if questionType is audio
        if (question.questionType == QuestionType.AUDIO)
        {
            //PlayOneShot
            questionAudio.PlayOneShot(question.audioClip);
            //wait for few seconds
            yield return new WaitForSeconds(audioLength + 0.5f);
            //play again
            StartCoroutine(PlayAudio());
        }
        else //if questionType is not audio
        {
            //stop the Coroutine
            StopCoroutine(PlayAudio());
            //return null
            yield return null;
        }
    }

    /// <summary>
    /// Method assigned to the buttons
    /// </summary>
    /// <param name="btn">ref to the button object</param>
    void OnClick(Button btn)
    {
        if (quizManager.GameStatus == GameStatus.PLAYING)
        {
            //if answered is false
            if (!answered)
            {
                //set answered true
                answered = true;
                //get the bool value
                bool val = quizManager.Answer(btn.name);

                //if its true
                if (val)
                {
                    //set color to correct
                    //btn.image.color = correctCol;
                    StartCoroutine(BlinkImg(btn.image));
                }
                else
                {
                    //else set it to wrong color
                    btn.image.color = wrongCol;
                }
            }
        }
    }

    /// <summary>
    /// Method to create Category Buttons dynamically
    /// </summary>
    void CreateCategoryButtons()
    {
        //we loop through all the available catgories in our QuizManager
        for (int i = 0; i < quizManager.QuizData.Count; i++)
        {
            //Create new CategoryBtn
            CategoryBtnScript categoryBtn = Instantiate(categoryBtnPrefab, scrollHolder.transform);
            //Set the button default values
            categoryBtn.SetButton(quizManager.QuizData[i].categoryChineseName, quizManager.QuizData[i].questions.Count);
            int index = i;
            //Add listner to button which calls CategoryBtn method
            categoryBtn.Btn.onClick.AddListener(() => CategoryBtn(index, quizManager.QuizData[index].categoryName));
        }
    }

    private void GameModeBtn(string modename, bool status)
    {
        if (status == false)
        {
            NotImplenmation();
            return;
        }

        if (modename == "Local") // 如果选择本地模式，开始游戏
        {
            StartLocalGame();
            modelSelect.SetActive(false);
            mainMenu.SetActive(true);
            localbtn.onClick.AddListener(() => LocalRet2Main());
        }
        else if (modename == "Knowledge Base")
        {
            modelSelect.SetActive(false);
            knowledgebase.SetActive(true);
            retbtn.onClick.AddListener(() => Ret2main());
            searchbtn.onClick.AddListener((() => Search()));
        }
        else if (modename == "againstMachine")
        {
            StartMachineBattle();
            modelSelect.SetActive(false);
            againstmachine.SetActive(true);
        }

        else if (modename == "LeaderBoard")
        {
            Debug.Log("ldbd");
            modelSelect.SetActive(false);
            leaderboard.SetActive(true);
            leaderboardbtn.onClick.AddListener(() => LeaderBoardRet2main());
        }
    }

    public void StartMachineBattle()
    {
        NextQues();
        machine_submit_btn.onClick.AddListener((() => SubmitAns()));
        machine_next_btn.onClick.AddListener((() => NextQues()));
        machine_ret_btn.onClick.AddListener((() => MachineRet2main()));
    }

    public void MachineRet2main()
    {
        modelSelect.SetActive(true);
        againstmachine.SetActive(false);
    }

    public MachineQues Getques()
    {
        Random rd = new Random();
        int index = rd.Next(machineBattleQues.Quess.Count);
        return machineBattleQues.Quess[index];
    }

    public void NextQues()
    {
        current_ques = Getques();
        machineQuesText.text = current_ques.question;
        machine_user_ans.text = "";
        machine_bert_ans.text = "Emmm";
        machine_user_ans.image.color = Color.white;
        machine_bert_ans.image.color = Color.white;
    }

    public void SubmitAns()
    {
        if (machine_user_ans.text == current_ques.correct_ans)
        {
            machine_user_ans.image.color = Color.green;
        }
        else
        {
            machine_user_ans.image.color = Color.red;
        }

        machine_bert_ans.text = current_ques.bert_ans;
        if (machine_bert_ans.text == current_ques.correct_ans)
        {
            machine_bert_ans.image.color = Color.green;
        }
        else
        {
            machine_bert_ans.image.color = Color.red;
        }
    }


    public void Search()
    {
        string ques = user_question.text;
        string model = lang_model.options[lang_model.value].text;
        string ans = getRequest(ques, model);
        ansText.text = ans;
    }

    public void Ret2main()
    {
        modelSelect.SetActive(true);
        knowledgebase.SetActive(false);
    }

    public void LeaderBoardRet2main()
    {
        leaderboard.SetActive(false);
        modelSelect.SetActive(true);
    }
    public void LocalRet2Main()
    {
        modelSelect.SetActive(true);
        mainMenu.SetActive(false);
    }

    //Method called by Category Button
    private void CategoryBtn(int index, string category)
    {
        quizManager.StartGame(index, category); //start the game
        mainMenu.SetActive(false); //deactivate mainMenu
        gamePanel.SetActive(true); //activate game panel
    }

    //this give blink effect [if needed use or dont use]
    IEnumerator BlinkImg(Image img)
    {
        for (int i = 0; i < 2; i++)
        {
            img.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            img.color = correctCol;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RestryButton()
    {
        string score = quizManager.getScore();
        string mode = quizManager.getMode();
        string user = submit_input.text;
        // Debug.Log(score);
        // Debug.Log(mode);
        // Debug.Log(user);
        submitRanking(user, mode, score);
        FetchLeaderBoardData();
        CreateLeaderBoardLines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}