using System;
using Eiko.YaSDK;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizableText : MonoBehaviour
    {
        [SerializeField] private string _ruText;
        [SerializeField] private string _enText;

        private string _addition = string.Empty;
        
        private void Start()
        {
            var text = YandexSDK.instance.Lang == "ru" ? _ruText + _addition : _enText + _addition;

            GetComponent<TextMeshProUGUI>().text = text;
        }

        public void AddText(string addition)
        {
            _addition = addition;
        }

        public void Refresh()
        {
            var text = YandexSDK.instance.Lang == "ru" ? _ruText + _addition : _enText + _addition;

            GetComponent<TextMeshProUGUI>().text = text;
        }

        public void SetText(string ruText, string enText)
        {
            _ruText = ruText;
            _enText = enText;
        }
    }
}