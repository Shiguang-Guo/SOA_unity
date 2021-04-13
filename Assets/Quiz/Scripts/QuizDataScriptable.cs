using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionsData", menuName = "QuestionsData", order = 1)]
public class QuizDataScriptable : ScriptableObject
{
    public string categoryName;
    public string categoryChineseName;
    public List<Question> questions;
}
// 一个种类的问题集合