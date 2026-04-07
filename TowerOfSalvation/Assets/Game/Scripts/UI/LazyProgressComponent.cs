using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LazyProgressComponent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _progressLabel;

    [SerializeField]
    private Image _progressImage;

    [SerializeField]
    private AnimationCurve _fillCurve;

    public Int32 MaxValue { get; set; }

    public Int32 TargetValue
    {
        get => _targetValue;
        set
        {
            if (!enabled || !gameObject.activeSelf)
            {
                _previousValue = _targetValue;
            }

            _targetValue = value;
            enabled = true;
            if (_previousValue < _targetValue && !String.IsNullOrEmpty(_format))
            {
                _progressUpdatedTimestamp = Time.time;
            }
            else
            {
                _previousValue = _targetValue;
            }
        }
    }

    public Single Progress => _progressImage.fillAmount;

    protected GameObject ProgressLabel => _progressLabel.gameObject;

    private Int32 _targetValue;
    private String _format;
    private Single _progressUpdatedTimestamp;
    private Int32 _previousValue;

    public void SetupValues(Int32 initialValue, Int32 targetValue)
    {
        TargetValue = targetValue;
        _progressUpdatedTimestamp = Time.time;
        _previousValue = initialValue;
        Update();
    }

    protected virtual void Awake()
    {
        _format = _progressLabel.text;
        Update();
    }

    protected virtual void Update()
    {
        var value = Mathf.Lerp(_previousValue, TargetValue, _fillCurve.Evaluate(Time.time - _progressUpdatedTimestamp));
        _progressImage.fillAmount = value / MaxValue;
        _progressLabel.text = String.Format(_format, (Int32)value, MaxValue);
        if (value < TargetValue)
        {
            return;
        }

        enabled = false;
    }

    private void OnDisable()
    {
        _previousValue = TargetValue;
    }
}