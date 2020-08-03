using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class AnimationEvent : MonoBehaviour
{
    public UnityEvent OnEndAnimation;

    public UnityEvent Tapa;
    public void EndAnimation()
    {
        OnEndAnimation.Invoke();
    }
    public void TapaPregunta()
    {
        Tapa.Invoke();
    }
}
