using System;
using System.Collections.Generic;
using Eiko.YaSDK;
using UnityEngine;

public static class AdHandler
{
    private static readonly Dictionary<string, Action> Actions = new();

    public static void Init()
    {
        YandexSDK.Instance.onRewardedAdReward += HandleReward;
    }

    private static void HandleReward(string obj)
    {
        if (Actions.ContainsKey(obj))
        {
            Actions[obj]?.Invoke();
            Time.timeScale = 1;
        }
    }

    public static void Dispose()
    {
        YandexSDK.Instance.onRewardedAdReward -= HandleReward;
    }

    public static void TryShowRewardAd(string id, Player player)
    {
        if (player.SkipAdCount > 0)
        {
            player.SkipAdCount -= 1;
            
            if (Actions.ContainsKey(id))
            {
                Actions[id]?.Invoke();
            }
            
            return;
        }
        
        YandexSDK.instance.ShowRewarded(id);
    }

    public static void AddHandler(Action handler, string id)
    {
        if (Actions.ContainsKey(id))
        {
            Actions.Remove(id);
        }
            
        Actions.Add(id, handler);
    }
}