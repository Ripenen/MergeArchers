using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eiko.YaSDK
{
    public partial class YandexSDK
    {

        #region Purchase
        public event Action<Purchase> onPurchaseSuccess;
        /// <summary>
        /// ѕокупка не удалась: в консоли разработчика не добавлен товар с таким id,
        /// пользователь не авторизовалс€, передумал и закрыл окно оплаты,
        /// истекло отведенное на покупку врем€, не хватило денег и т. д.
        /// </summary>
        public event Action<string> onPurchaseFailed;
        public event Action onPurchaseInitialize;
        public event Action<string> onPurchaseInitializeFailed;
        public void OnPurchaseInitialize()
        {
            onPurchaseInitialize?.Invoke();
        }

        public void OnPurchaseInitializeFailed(string error)
        {
            onPurchaseInitializeFailed?.Invoke(error);
        }
        public event Action onGetPurchaseFailed;
        public void OnGetPurchaseFailed()
        {
            onGetPurchaseFailed?.Invoke();
        }
        public event Action<GetPurchasesCallback> GettedPurchase;
        public event Action<string> GettedPurchaseFail;
        public void OnGetPurchases(string json)
        {
            var callback = JsonUtility.FromJson<GetPurchasesCallback>(json);
            GettedPurchase?.Invoke(callback);
        }
        public void OnGetPurchasesFail(string err)
        {
            GettedPurchaseFail?.Invoke(err);
        }
        public void TryGetPurchases()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
                    GetPurchases();
#else
            OnGetPurchases(JsonUtility.ToJson(new GetPurchasesCallback() { signature = "", purchases = new Purchase[0] }));
#endif
        }
        #endregion
        public void OnPurchaseSuccess(string json)
        {
            var purchase = JsonUtility.FromJson<Purchase>(json);
            onPurchaseSuccess?.Invoke(purchase);
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="error"></param>
        public void OnPurchaseFailed(string error)
        {
            onPurchaseFailed?.Invoke(error);
        }
        public void InitializePurchases()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            InitPurchases();
#else
            OnPurchaseInitialize();
#endif

        }

        public void ProcessPurchase(string id)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            Purchase(id);
#else
            OnPurchaseSuccess(JsonUtility.ToJson(new Purchase() { productID = id }));
#endif
        }

    }
    [Serializable]
    public class GetPurchasesCallback
    {
        public Purchase[] purchases;
        public string signature;
    }
    [Serializable]
    public class Purchase
    {
        public string productID;
        public string purchaseToken;
        public string developerPayload;
        public string signature;
    }
}