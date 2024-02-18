using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlider : MonoBehaviour
{
    [SerializeField] private Image[] _rewardSectors;
    [SerializeField] private RectTransform _cursor;
    [SerializeField] private TextMeshProUGUI _scaledReward;
    [SerializeField] private int _angleSpeed;
    
    public int ScaledReward { get; private set; }

    private int _baseReward;
    private float _angle;
    private int _axis = -1;

    public void Setup(int reward)
    {
        _baseReward = reward;
        ScaledReward = (int)(reward * 1.5f);

        StartCoroutine(SliderMove());
    }

    private IEnumerator SliderMove()
    {
        _cursor.Rotate(0, 0, -10);
        
        while (enabled)
        {
            _angle += _angleSpeed * Time.deltaTime;

            if (_angle >= 175)
            {
                _angle = 0;
                _axis *= -1;
            }
            
            _cursor.Rotate(0, 0, _angleSpeed * _axis * Time.deltaTime);
            
            var multiply = 1.5f;

            if (_cursor.rotation.eulerAngles.z - 360 < 0.8f * -180 && _cursor.rotation.eulerAngles.z - 360 < 0)
                multiply = 3;
            else if (_cursor.rotation.eulerAngles.z - 360 < 0.45f * -180)
                multiply = 2;

            ScaledReward = (int)(_baseReward * multiply);

            _scaledReward.text = ScaledReward.ToString();
            
            yield return new WaitForEndOfFrame();
        }
    }
}