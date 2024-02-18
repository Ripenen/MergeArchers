using Eiko.YaSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurchaseProcess
{
    public static PurchaseProcess instance;
    public List<Purchase> purchases;
    private string signature;
    public bool isIint = true;
    public PurchaseProcess()
    {
        instance = this;
    }
    private WaitPurchizeInit ini;
    public WaitPurchizeInit InitPurchases()
    {
        var inst = YandexSDK.Instance;
        inst.onPurchaseSuccess += Instance_onPurchaseSuccess;
        inst.GettedPurchase += Instance_GettedPurchase;
        inst.GettedPurchaseFail += Instance_GettedPurchaseFail; ;
        inst.onPurchaseInitialize += Instance_onPurchaseInitialize;
        inst.onPurchaseFailed += Instance_onPurchaseFailed; ;
        ini = new WaitPurchizeInit();
        inst.InitializePurchases();
        return ini;
    }

    private void Instance_GettedPurchaseFail(string obj)
    {
        ini.EndWait(true);
    }

    private void Instance_onPurchaseFailed(string obj)
    {
        isIint = false;
        ini.EndWait(false);
    }

    private void Instance_onPurchaseInitialize()
    {
        Debug.Log("OnPurchizeInitialize");
        isIint = true;
        GetPurchases();
    }

    private void Instance_GettedPurchase(GetPurchasesCallback obj)
    {
        Debug.Log("OnGettedPurchize");
        purchases = obj.purchases.ToList();
        signature = obj.signature;
        ini.EndWait(true);
    }
    private Action successAction;
    public void ProcessPurchase(string id, Action action=null)
    {
        successAction = action;
        YandexSDK.Instance.ProcessPurchase(id);
    }
    private void Instance_onPurchaseSuccess(Purchase obj)
    {
        purchases.Add(obj);
        successAction?.Invoke();
    }

    private void GetPurchases()
    {
        YandexSDK.Instance.TryGetPurchases();

    }
    public static bool Has(string id)
    {
        return null != instance.purchases.FirstOrDefault(x=>x.productID==id);
    }
}
public class WaitPurchizeInit : CustomYieldInstruction
{
    public bool Result { get; private set; }
    private bool _keepWaiting = true;
    public void EndWait(bool Result)
    {
        Debug.Log("EndWait");
        _keepWaiting = false;
        this.Result = Result;
    }
    public override bool keepWaiting =>_keepWaiting;
}