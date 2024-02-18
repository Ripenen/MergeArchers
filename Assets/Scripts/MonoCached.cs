using System;
using System.Collections;
using UnityEngine;

public class MonoCached : MonoBehaviour
{
    public static event Action Tick;
    public static event Action LateTick;

    private static MonoCached _instance;

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        Tick?.Invoke();
    }

    private void LateUpdate()
    {
        LateTick?.Invoke();
    }

    public static Coroutine StartRoutine(IEnumerator routine)
    {
        return _instance.StartCoroutine(routine);
    }

    public static void Timer(MonoBehaviour behaviour, float time, Action action)
    {
        behaviour.StartCoroutine(TimerCoroutine());

        IEnumerator TimerCoroutine()
        {
            yield return new WaitForSeconds(time);

            action?.Invoke();
        }
    }
}