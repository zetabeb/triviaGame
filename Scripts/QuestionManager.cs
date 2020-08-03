using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Ezphera.TimerScore;
using System.IO;
using System.Text;

public class QuestionManager : MonoBehaviour
{
    //public List<GameObject> preguntas = new List<GameObject>();

    [Header("Questions")]
    [Tooltip("List of questions")]
    public List<MultiQuestion> questions;
    [Tooltip("Number of questions to show")]
    public int QuatityQuestions = 5;

    [Header("Instance Question")]
    public RectTransform inicio;
    [Tooltip("Where the questions will be instantiated")]
    public Transform questionContainer;
    [Tooltip("QuestionPrefab")]    
    public Ask questionPref;

    [Header("Data file in local PC")]
    [Tooltip("file with questions on the local computer")]
    public string dirLocal = "C:\\Users\\DRAGON ROJO\\Desktop\\Trivia.txt";

    [Header("Data file in Web")]
    [Tooltip("folder URL on Web")]
    public string dirWeb = "http://ezpheratech.com/demos/";
    [Tooltip("file name with extension (.txt, .ini, etc.)")]
    public string nameFile = "Trivia.txt";

    string longStringFromFile;
    private Ask actualQuestion;
    public static QuestionManager instance;
    int k = 0;

    [Header("Sounds")]
    [Tooltip("Start Sound")]
    public AudioClip letsPlay;
    [Tooltip("Sound Source")]
    public AudioSource audioSource;

    [Header("Notice")]
    public GameObject puntajeAcumulado;
    public GameObject resultadoPreg;
    public GameObject resultadoFinal;
    

    
    private void Awake()
    {
        instance = this;
        resultadoFinal.SetActive(false);
        if (resultadoPreg != null)resultadoPreg.SetActive(false);
        if (inicio) inicio.gameObject.SetActive(true);
        Debug.Log("Hay "+questions.Count+" preguntas");        
    }    
    
    List<int> questionsIndex =new List<int>();

    public void InstanciateQuestion(int questionIndex)
    {
        Debug.Log("Instanciamos la pregunta " + questionIndex);
        if (actualQuestion != null) Destroy(actualQuestion.gameObject);
        actualQuestion = Instantiate(questionPref, questionContainer);

        var AqRt = actualQuestion.gameObject.GetComponent<RectTransform>();
        AqRt.sizeDelta = inicio.sizeDelta;
        AqRt.anchoredPosition = inicio.anchoredPosition;
        AqRt.localScale = inicio.localScale;
        AqRt.pivot = inicio.pivot;

        actualQuestion.texto.text = questions[questionIndex].ask;
        int index = 0;

        foreach (ButtonsAnswers button in actualQuestion.buttonsAnswers)
        {
            button.answer.transform.Find("Respuesta").GetComponent<Text>().text = questions[questionIndex].Answer[index];
            button.isCorrect = questions[questionIndex].isCorrect == index;
            index++;
        }
    }

    public void LetsPlay()
    {
        audioSource.clip = letsPlay;
        audioSource.Play();
    }

    //public void getRandom()
    //{
    //    if (i <= QuatityAnswer)
    //    {

    //        //try
    //        //{
    //        //    AnswerBack.SetActive(false);
    //        //}

    //        //catch
    //        //{
    //        //    Debug.Log("Answer está vacío");
    //        //}
    //        nextAnswer();

    //        int randomIndex = Random.Range(0, preguntas.Count);
    //        preguntas[randomIndex].transform.position = transform.position;
    //        preguntas[randomIndex].transform.rotation = transform.rotation;
    //        preguntas[randomIndex].SetActive(true);

    //        //AnswerBack = preguntas[randomIndex];

    //        preguntas.RemoveAt(randomIndex);
    //        i++;
    //    }
    //    else
    //    {
    //        Invoke("Finish", 4f);
    //        //Finish();
    //    }

    //}

    public void nextAnswer()
    {
        var randomNum = Random.Range(0, questions.Count - 1);
        while (questionsIndex.Contains(randomNum))
        {
            randomNum = Random.Range(0, questions.Count - 1);
        }
        questionsIndex.Add(randomNum);

        if (k < QuatityQuestions)
        {
            //InstanciateQuestion(i++);
            InstanciateQuestion(randomNum);
            
            k++;
        }
        else
        {
            Finish();
        }
    }

    #region addQuestionLocal
    public void addQuestionLocal()
    {
        RemoveList();
        Stream texto = new FileStream(dirLocal, FileMode.Open, FileAccess.Read);
        long quantity = texto.Length; //Contador de letras (char) 
        int intFrase = 0;

        for (int i = 0; i < quantity; i++)
        { //Contar cantidad de preguntas            
            texto.Seek(i, SeekOrigin.Begin);
            int valor = texto.ReadByte();
            char letra = (char)valor;
            if (letra == ';') { intFrase++; } //intFrase es la cantidad de preguntas
        }

        string[] frase = new string[intFrase]; //frase[] es el conjunto de pregunta (ask, isCorrect, Answer[]) 
        int n = 0;
        var resp = new string[] { }; //inicializar var

        for (int i = 0; i < quantity; i++)
        {
            texto.Seek(i, SeekOrigin.Begin);
            int valor = texto.ReadByte();
            char letra = (char)valor;

            if (letra != '\n')
            {
                frase[n] += letra; //El formato de preguntas{Pregunta(ask), N° correcta(IsCorrect), cantidad respuestas(m), respuesta1, ..., respuesta(m)}
            }

            if (letra == ';') //Las preguntas se separan por ;
            {
                frase[n] = frase[n].Substring(0, frase[n].Length - 1); //frase[n] -= letra; //quiero borrar la última letra que sería el ;
                int m = 2;
                char separator = ',';
                string[] items = frase[n].Split(separator); //las respuestas se separan por ,

                string ask = ConvertUTF8(items[0]);

                Debug.Log("Ask: " + ask);
                Debug.Log("Correct: " + items[1]);

                for (int j = 0; j < int.Parse(items[2]); j++)
                {
                    m++;
                    Debug.Log("Answer: " + items[m]);
                }
                string item1;
                string item2;
                string item3;
                string item4;
                string item5;

                switch ((m - 2))
                {
                    case 2: //2 respuestas
                        item1 = ConvertUTF8(items[3]);
                        item2 = ConvertUTF8(items[4]);
                        resp = new string[] { item1, item2 };
                        break;
                    case 3: //3 respuestas
                        item1 = ConvertUTF8(items[3]);
                        item2 = ConvertUTF8(items[4]);
                        item3 = ConvertUTF8(items[5]);
                        resp = new string[] { item1, item2, item3 };
                        break;
                    case 4: //4 respuestas
                        item1 = ConvertUTF8(items[3]);
                        item2 = ConvertUTF8(items[4]);
                        item3 = ConvertUTF8(items[5]);
                        item4 = ConvertUTF8(items[6]);
                        resp = new string[] { item1, item2, item3, item4 };
                        break;
                    case 5: //5 respuestas
                        item1 = ConvertUTF8(items[3]);
                        item2 = ConvertUTF8(items[4]);
                        item3 = ConvertUTF8(items[5]);
                        item4 = ConvertUTF8(items[6]);
                        item5 = ConvertUTF8(items[7]);
                        resp = new string[] { item1, item2, item3, item4, item5 };
                        break;
                }

                questions.Add(new MultiQuestion()
                {
                    ask = ask,
                    isCorrect = int.Parse(items[1]),
                    Answer = resp
                });

                n++;
            }
        }
        texto.Close();
    }
    #endregion

    #region addQuestionWeb
    private IEnumerator CheckFileURL()
    {
        WWW w = new WWW(dirWeb + nameFile);
        yield return w;
        if (w.error != null)
        {
            Debug.Log("Error NO SE ENCONTRO .. " + w.error);
            // for example, often 'Error .. 404 Not Found'
        }
        else
        {
            Debug.Log("Found ... ==>" + w.text + "<==");
            longStringFromFile = w.text;

            Debug.Log("cargando...." + nameFile);

            addQuestionWeb(longStringFromFile);
        }
    }


    public void addQuestionWeb(string file)
    {
        RemoveList();


        int quantity = file.Length; //Contador de letras (char) 
        int intFrase = 0;
        char[] chars = file.ToCharArray();

        for (int i = 0; i < quantity; i++)
        {
            file.Substring(0, quantity);

            char letra = chars[i];
            if (letra == ';') { intFrase++; } //intFrase es la cantidad de preguntas
        }

        string[] frase = new string[intFrase]; //frase[] es el conjunto de pregunta (ask, isCorrect, Answer[]) 
        int n = 0;
        var resp = new string[] { }; //inicializar var

        for (int i = 0; i < quantity; i++)
        {
            file.Substring(0, quantity);
            char letra = chars[i];

            if (letra != '\n')
            {
                frase[n] += letra; //El formato de preguntas{Pregunta(ask), N° correcta(IsCorrect), cantidad respuestas(m), respuesta1, ..., respuesta(m)}
            }

            if (letra == ';') //Las preguntas se separan por ;
            {
                frase[n] = frase[n].Substring(0, frase[n].Length - 1); //frase[n] -= letra; //quiero borrar la última letra que sería el ;
                int m = 2;
                char separator = ',';
                string[] items = frase[n].Split(separator); //las respuestas se separan por ,

                string ask = ConvertUTF8(items[0]);

                Debug.Log("Ask: " + ask);
                Debug.Log("Correct: " + items[1]);

                for (int j = 0; j < int.Parse(items[2]); j++)
                {
                    m++;
                    Debug.Log("Answer: " + items[m]);
                }
                string item1;
                string item2;
                string item3;
                string item4;
                string item5;

                switch ((m - 2))
                {
                    case 2: //2 respuestas
                        item1 = ConvertUTF8(items[3]);
                        item2 = ConvertUTF8(items[4]);
                        resp = new string[] { item1, item2 };
                        break;
                    case 3: //3 respuestas
                        item1 = ConvertUTF8(items[3]);
                        item2 = ConvertUTF8(items[4]);
                        item3 = ConvertUTF8(items[5]);
                        resp = new string[] { item1, item2, item3 };
                        break;
                    case 4: //4 respuestas
                        item1 = ConvertUTF8(items[3]);
                        item2 = ConvertUTF8(items[4]);
                        item3 = ConvertUTF8(items[5]);
                        item4 = ConvertUTF8(items[6]);
                        resp = new string[] { item1, item2, item3, item4 };
                        break;
                    case 5: //5 respuestas
                        item1 = ConvertUTF8(items[3]);
                        item2 = ConvertUTF8(items[4]);
                        item3 = ConvertUTF8(items[5]);
                        item4 = ConvertUTF8(items[6]);
                        item5 = ConvertUTF8(items[7]);
                        resp = new string[] { item1, item2, item3, item4, item5 };
                        break;
                }

                questions.Add(new MultiQuestion()
                {
                    ask = ask,
                    isCorrect = int.Parse(items[1]),
                    Answer = resp
                });

                n++;
            }
        }
    }
    #endregion

    public void ShowCorrectIncorrect(bool isCorrect)
    {
        resultadoPreg.SetActive(true);
        //resultadoPreg.transform.Find("ResultTitle").GetComponent<Text>().text = isCorrect ? "¡AWESOME!" : "¡UPPSS!";
        resultadoPreg.transform.Find("bgCorrect").GetComponent<Image>().enabled = isCorrect;
        resultadoPreg.transform.Find("bgIncorrect").GetComponent<Image>().enabled = !isCorrect;
        resultadoPreg.transform.Find("puntaje").GetComponent<Text>().text = ScoreManager.instance.GetScore().ToString();
        if (puntajeAcumulado) puntajeAcumulado.transform.Find("puntaje").GetComponent<Text>().text = ScoreManager.instance.GetScore().ToString();
    }

    public void SaveRanking()
    {

    }

    void Finish()
    {
        Debug.Log("finaliza el juego");
        resultadoFinal.SetActive(true);
        resultadoFinal.transform.Find("puntaje").GetComponent<Text>().text = ScoreManager.instance.GetScore().ToString();
        SaveRanking(); //ERRORRRRR
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Trivia_HP");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void RemoveList()
    {
        int q = (questions.Count)-1;
        for (int i = q ;  i >= 0; i--)
        {
            Debug.Log(i);
            questions.RemoveAt(i);
        }
    }

    string ConvertUTF8(string TextUnicode)
    {
        string TextUTF8 = TextUnicode;
        //Conversión de Unicode a UTF8
        byte[] utf8Bytes = new byte[TextUTF8.Length];
        for (int i = 0; i < TextUTF8.Length; ++i)
        {
            //Debug.Assert( 0 <= utf8String[i] && utf8String[i] <= 255, "the char must be in byte's range");
            utf8Bytes[i] = (byte)TextUTF8[i];
        }
        TextUTF8 = Encoding.UTF8.GetString(utf8Bytes, 0, TextUTF8.Length);
        return TextUTF8;
    }

}

[System.Serializable]
public class MultiQuestion
{
    [TextArea]
    public string ask;
    [Tooltip("Recuerda que el array comienza de 0")]
    public int isCorrect;
    [TextArea]
    public string[] Answer;   
}
