#if UNITY_EDITOR
using Eiko.YaSDK;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Eiko.YaSDK.Editor
{

    public class CanvasAddEditor : MonoBehaviour
    {
        public GameObject baner;
        public GameObject fullScreen;
        public GameObject revarded;
        public GameObject Review;
        public Text time;
        private int placement;
        public bool keepWaiting = false;
        private static bool IsReviewed;
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
        }
        public void CloseFullScreen()
        {
            fullScreen.SetActive(false);
            YandexSDK.instance.OnInterstitialShown();
        }
        public void CloseReward()
        {
            revarded.SetActive(false);
            YandexSDK.instance.OnRewardedClose(placement); 
            
            
                
        }
        public void OpenFullScreen()
        {
            fullScreen.SetActive(true);
        }
        public void OpenReward(int i)
        {
            this.placement = i;
            revarded.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(WaitReward());
        }
        public IEnumerator WaitReward()
        {
            keepWaiting = true;
            for (int i = 5; i > 0; i--)
            {
                time.text = "Seconds " + i;
                yield return new WaitForSecondsRealtime(1);
            }
            time.text = "Award received";
            keepWaiting = false;
            YandexSDK.instance.OnRewarded(placement);
            Debug.Log("Onrevardet");
        }
        public void ShowReview()
        {
            Review.SetActive(true);
        }
        public void ReviewClosed()
        {
            Review.SetActive(false);
            string Json;
            
            Json = JsonUtility.ToJson(
                new ReviewCallback() 
                {
                    CanReview = !IsReviewed,
                    FeedbackSent = !IsReviewed, 
                    Reason = IsReviewed? "GAME_RATED" : "Success" 
                });
            IsReviewed = true;
            YandexSDK.instance.OnReview(Json);
        }
    }
}
#endif