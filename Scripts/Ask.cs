using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ezphera;
using Ezphera.TimerScore;
public class Ask : MonoBehaviour
{
    public int score;
    public Text texto;
    Timer timer;
    public List<ButtonsAnswers> buttonsAnswers;
    public AudioClip tension1;
    public AudioClip tension2;
    public AudioClip tension3;
    public AudioClip finishTimeSound;
    public AudioClip answerCorrect;
    public AudioClip answerIncorrect;
    AudioSource soundSource;
    int InitialTime;
    bool finishTimeBool;

    private void OnEnable()
    {
        soundSource = GetComponent<AudioSource>();
        finishTimeBool = false;

        timer = GetComponent<Timer>();
        timer.OnEnd += Timer_OnEnd;
        InitialTime = int.Parse(timer.GetTimeText());
    
        foreach (ButtonsAnswers button in buttonsAnswers)
        {
            button.answer.onClick.AddListener(delegate 
            {
                Answer(button.isCorrect);
                button.answer.GetComponentInParent<Animator>().SetInteger("answer", button.isCorrect ? 1 : -1);
                Debug.Log("Llamar siguiente pregunta o resultados");
                foreach (ButtonsAnswers button2 in buttonsAnswers)
                {
                    button2.answer.interactable = false;
                    if (button2.isCorrect) button2.answer.GetComponentInParent<Animator>().SetInteger("answer", 1);

                }
            });
        }
    } 

    private void OnDisable()
    { 
        timer.OnEnd -= Timer_OnEnd;
    }

    private void Timer_OnEnd()
    {
        //Se acabo el tiempo... Deberia bloquear interaccion y dejar puntaje 0
        Debug.Log("Se acabó el tiempo");
        score = int.Parse(timer.GetTimeText());
        ScoreManager.instance.Add(score);
        QuestionManager.instance.ShowCorrectIncorrect(false);
    }

    private void Update()
    {
        float time = float.Parse(timer.GetTimeText());
        float fp = time / InitialTime; 
        //Debug.Log(GetScore());
        if (fp < 0.5f && fp > 0.3f)
        {
            if (tension1) {
                
                soundSource.clip = tension1;
                if (soundSource.isPlaying) return;
                soundSource.Play();
                
            }
            
        }
        if (fp < 0.3f && fp > 0.2f)
        {
            if (tension2)
            {
                soundSource.clip = tension2;
                if (soundSource.isPlaying) return;
                soundSource.Play();
            }
        }
        if (fp < 0.2f && fp > 0.01f)
        {
            if (tension3)
            {
                soundSource.clip = tension3;
                if (soundSource.isPlaying) return;
                soundSource.Play();
            }
        }
        if (fp < 0.01f)
        {
            if (finishTimeSound)
            {                
                soundSource.clip = finishTimeSound;
                if (soundSource.isPlaying || finishTimeBool) return;
                finishTimeBool = true;
                soundSource.Play();
            }
        }
    }
    public void Answer(bool isCorrect)
    {
        //timer = GetComponent<Timer>();
        if (isCorrect)
        {
            soundSource.clip = answerCorrect;
            soundSource.Play();

            timer.Pause();
            score = int.Parse(timer.GetTimeText());
            ScoreManager.instance.Add(score);
        }
        else
        {
            soundSource.clip = answerIncorrect;
            soundSource.Play();
            timer.Stop();
        }
        QuestionManager.instance.ShowCorrectIncorrect(isCorrect);
    }
}
[System.Serializable]
public class ButtonsAnswers
{
    public Button answer;
    public bool isCorrect;
}