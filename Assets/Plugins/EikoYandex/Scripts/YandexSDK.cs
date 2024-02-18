#if UNITY_EDITOR
using Eiko.YaSDK.Editor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Eiko.YaSDK
{
    public partial class YandexSDK : MonoBehaviour {

    #if UNITY_EDITOR
        [HideInInspector]
        public CanvasAddEditor editorCanvas;
    #endif
        public const int ReloadAdsSeconds = 30;
        public const string key = "AddsOff";
        public bool AdsEnabled { get; private set; }

        public static YandexSDK instance;
        [DllImport("__Internal")]
        private static extern void GetUserData();
        [DllImport("__Internal")]
        private static extern void ShowFullscreenAd();
        [DllImport("__Internal")]
        private static extern void SetData(string key,string value);
        [DllImport("__Internal")]
        private static extern void SetScore(string key, int value);
        [DllImport("__Internal")]
        private static extern void InitPlayerData();
        
        [DllImport("__Internal")]
        private static extern void SetLeaderBoard(int value);
        /// <summary>
        /// Returns an int value which is sent to index.html
        /// </summary>
        /// <param name="placement"></param>
        /// <returns></returns>
        [DllImport("__Internal")]
        private static extern int ShowRewardedAd(string placement);
        //[DllImport("__Internal")]
        //private static extern void GerReward();
        [DllImport("__Internal")]
        private static extern void AuthenticateUser();
        [DllImport("__Internal")]
        private static extern void InitPurchases();
        [DllImport("__Internal")]
        private static extern void Purchase(string id);
        [DllImport("__Internal")]
        private static extern string GetLang();

        [DllImport("__Internal")]
        private static extern void Review();
        [DllImport("__Internal")]
        private static extern void GetPurchases();

        public event Action addsOnReloaded;
        public event Action onUserDataReceived;

        public event Action onInterstitialShown;
        public event Action<string> onInterstitialFailed;
        /// <summary>
        /// Пользователь открыл рекламу
        /// </summary>
        public event Action<int> onRewardedAdOpened;
        /// <summary>
        /// Пользователь должен получить награду за просмотр рекламы
        /// </summary>
        public event Action<string> onRewardedAdReward;
        /// <summary>
        /// Пользователь закрыл рекламу
        /// </summary>
        public event Action<int> onRewardedAdClosed;
        /// <summary>
        /// Вызов/просмотр рекламы повлёк за собой ошибку
        /// </summary>
        public event Action<string> onRewardedAdError;
        /// <summary>
        /// Покупка успешно совершена
        /// </summary>
       

        public event Action onClose;
        public Queue<int> rewardedAdPlacementsAsInt = new Queue<int>();
        public Queue<string> rewardedAdsPlacements = new Queue<string>();
        private Action<ReviewCallback> actionReview;
        public bool addsAvailable;
        private bool IsReviewed = false;
        public UserData user;

        public static YandexSDK Instance => FindObjectOfType<YandexSDK>();
       
        public string Lang = "ru";

        public void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
#if !UNITY_EDITOR && UNITY_WEBGL
                Lang = GetLang();
#endif
            }
            else {
                Destroy(gameObject);
            }
            StartCoroutine(WaitAddReload());
#if UNITY_EDITOR
            editorCanvas =  Instantiate(editorCanvas);
#endif
            AdsEnabled = 0 == PlayerPrefs.GetInt(key, 0);
        }

        /// <summary>
        /// Call this to ask user to authenticate
        /// </summary>
        public void Authenticate() {
#if !UNITY_EDITOR && UNITY_WEBGL
            AuthenticateUser();
#endif
        }

        /// <summary>
        /// Call this to show interstitial ad. Don't call frequently. There is a 3 minute delay after each show.
        /// </summary>
        public void ShowInterstitial() {
#if UNITY_ANDROID
            return;
#endif
            if(addsAvailable)
            {
                AppMetricaWeb.Event("otherAd");
                StartCoroutine(WaitAddReload());
#if !UNITY_EDITOR && UNITY_WEBGL
                ShowFullscreenAd();
#elif UNITY_EDITOR
                editorCanvas.OpenFullScreen();
#endif
                TempTimeScale = Time.timeScale;
                Time.timeScale = 0;
                soundBefore = AudioListener.pause;
                AudioListener.pause = true;
            }
            else
            {
                Debug.LogWarning("Ad not ready!");
            }
        }

        /// <summary>
        /// Call this to show rewarded ad
        /// </summary>
        /// <param name="placement"></param>
        public void ShowRewarded(string placement, Action rewa = null, Action rewardetClose=null) {
            revarded = rewa;
            revardedClose = rewardetClose;
#if !UNITY_EDITOR && UNITY_WEBGL
            int placemantId = ShowRewardedAd(placement);
#else
            int placemantId = 0;
#endif
            rewardedAdPlacementsAsInt.Enqueue(placemantId);
            rewardedAdsPlacements.Enqueue(placement);
            TempTimeScale = Time.timeScale;
            Time.timeScale = 0;
            soundBefore = AudioListener.pause;
            AudioListener.pause = true;
            #if UNITY_ANDROID || UNITY_EDITOR
            OnRewarded(placemantId);
            #endif
#if false
            editorCanvas.OpenReward(placemantId);
            
#endif
        }
        public Action revarded;
        public Action revardedClose;
        /// <summary>
        /// Отключает межстраничную рекламу
        /// </summary>
        public void AdsOff()
        {
            AdsEnabled = false;
            StopAllCoroutines();
            Debug.Log("AdsOff");
            addsAvailable = false;
        }
        public void SetInLeaderBoard(int value)
        {
#if !UNITY_EDITOR&& UNITY_WEBGL
            SetLeaderBoard(value);
#endif
        }
        
        /// <summary>
        /// Call this to receive user data
        /// </summary>
        public void RequestUserData() {
#if !UNITY_EDITOR && UNITY_WEBGL
            GetUserData();
#endif
        }
    
       
    
        public void StoreUserData(string data) {
            user = JsonUtility.FromJson<UserData>(data);
            onUserDataReceived?.Invoke();
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnInterstitialShown() {
            Time.timeScale = TempTimeScale;
            onInterstitialShown?.Invoke();
            AudioListener.pause = soundBefore;
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="error"></param>
        public void OnInterstitialError(string error) {
            Time.timeScale = TempTimeScale;
            onInterstitialFailed?.Invoke(error);
            AudioListener.pause = soundBefore;
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="placement"></param>
        public void OnRewardedOpen(int placement) {
            onRewardedAdOpened?.Invoke(placement);
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="placement"></param>
        public void OnRewarded(int placement) {
            Time.timeScale = 1;
            if (placement == rewardedAdPlacementsAsInt.Dequeue()) {
                onRewardedAdReward?.Invoke(rewardedAdsPlacements.Dequeue());
            }
            revarded?.Invoke();
            AudioListener.pause = soundBefore;
        }
        private float TempTimeScale;
        private bool soundBefore;

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="placement"></param>
        public void OnRewardedClose(int placement) {
            AudioListener.pause = soundBefore;
            Time.timeScale = TempTimeScale;
            onRewardedAdClosed?.Invoke(placement);
            revardedClose?.Invoke();
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="placement"></param>
        public void OnRewardedError(string placement) {
            Time.timeScale = TempTimeScale;
            AudioListener.pause = soundBefore;
            onRewardedAdError?.Invoke(placement);
            rewardedAdsPlacements.Clear();
            rewardedAdPlacementsAsInt.Clear();
        }

        
    
        /// <summary>
        /// Browser tab has been closed
        /// </summary>
        /// <param name="error"></param>
        public void OnClose() {
            onClose?.Invoke();
        }
        
        public IEnumerator WaitAddReload()
        {
            addsAvailable = false;
            yield return new WaitForSecondsRealtime(ReloadAdsSeconds);
            addsAvailable = true;
            addsOnReloaded?.Invoke();
        }
        public void ShowReview(Action<ReviewCallback> action = null)
        {
            actionReview = action;
            if (IsReviewed)
            {
                OnReview(JsonUtility.ToJson(
                new ReviewCallback()
                {
                    CanReview = false,
                    FeedbackSent = false,
                    Reason = IsReviewed ? "GAME_RATED" : "Success"
                }));
                
                return;
            }
#if !UNITY_EDITOR && UNITY_WEBGL
            Review();
#elif UNITY_EDITOR
            editorCanvas.ShowReview();
#endif
        }
        public void OnReview(string callback)
        {
            ReviewCallback review = JsonUtility.FromJson<ReviewCallback>(callback);
            if(review.FeedbackSent)
            {
                IsReviewed = true;
            }
            actionReview?.Invoke(review);            
        }
        public event Action<GetDataCallback> onDataRecived;
        public event Action noAutorized;
        public void OnGetData(string json)
        {
            GetDataCallback callback;
            if (!string.IsNullOrEmpty(json))
            {
                callback = JsonUtility.FromJson<GetDataCallback>(json); 
                if(callback.data==null)
                {
                    callback.data = new KeyValuePairStringCallback[0];
                }
                if(callback.score == null)
                {
                    callback.score = new KeyValuePairIntCallback[0];
                }
            }
            else
            {
                callback = new GetDataCallback();
                callback.data = new KeyValuePairStringCallback[0];
                callback.score = new KeyValuePairIntCallback[0];
            }
            onDataRecived?.Invoke(callback);
        }
        public void NoAutorized()
        {
            noAutorized?.Invoke();
        }
        
        public void SetPlayerData(string key, string value)
        {
#if !UNITY_EDITOR
            SetData(key, value);
#endif
        }
        
        public void SetPlayerScore(string key, int value)
        {
#if !UNITY_EDITOR
            SetScore(key, value);
#endif
        }

        public void InitData()
        {
#if !UNITY_EDITOR && !UNITY_ANDROID
            InitPlayerData();
#else
            StartCoroutine(InitDataEmit());
#endif
        }
#if UNITY_EDITOR || UNITY_ANDROID
        private IEnumerator InitDataEmit()
        {
            yield return new WaitForSeconds(1);
            NoAutorized();
        }
#endif
    }
    [Serializable]
    public class GetDataCallback
    {
        public KeyValuePairStringCallback[] data;
        public KeyValuePairIntCallback[] score;
    }
    [Serializable]
    public class KeyValuePairStringCallback
    { 
        public string key;
        public string value;
    }
    [Serializable]
    public class KeyValuePairIntCallback
    {
        public string key;
        public int value;
    }

    public struct ReviewCallback
    {
        public bool CanReview;
        public string Reason;
        public bool FeedbackSent;
    }

    public struct UserData {
        public string id;
        public string name;
        public string avatarUrlSmall;
        public string avatarUrlMedium;
        public string avatarUrlLarge;
    }
    
}