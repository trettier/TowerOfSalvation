using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthBarRender : MonoBehaviour
{
    public Image full;
    public Image difference;
    public Image current;

    public TMP_Text value;

    private HealthPoints _healthPoints;

    [SerializeField] private float smoothSpeed = 2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    [SerializeField] private float destroyDelay = 0.2f;

    private Coroutine _diffCoroutine;

    private Transform _character;
    public void Initialize(Transform character, HealthPoints hp)
    {
        _character = character;

        _healthPoints = hp;

        _healthPoints.GetDamage += Decrease;
        _healthPoints.Death += OnDeath;
        _healthPoints.IncreaseEvent += Increase;

        UpdateInstant();
    }

    private void FixedUpdate()
    {
        if (_character == null) 
            Destroy(gameObject);
        else
            transform.position = _character.position + offset;
    }

    private void UpdateInstant()
    {
        float normalized = _healthPoints.current / _healthPoints.max;

        current.fillAmount = normalized;
        difference.fillAmount = normalized;

        UpdateText();
    }

    private void Decrease()
    {
        float normalized = _healthPoints.current / _healthPoints.max;

        current.fillAmount = normalized;

        if (_diffCoroutine != null)
            StopCoroutine(_diffCoroutine);

        _diffCoroutine = StartCoroutine(SmoothDecrease(normalized));

        UpdateText();
    }

    private IEnumerator SmoothDecrease(float target)
    {
        while (difference.fillAmount > target)
        {
            difference.fillAmount = Mathf.Lerp(
                difference.fillAmount,
                target,
                Time.deltaTime * smoothSpeed
            );

            if (Mathf.Abs(difference.fillAmount - target) < 0.01f)
            {
                difference.fillAmount = target;
                break;
            }

            yield return null;
        }
    }

    private void Increase()
    {
        float normalized = _healthPoints.current / _healthPoints.max;

        // при хиле оба бара сразу обновляем
        current.fillAmount = normalized;
        difference.fillAmount = normalized;

        UpdateText();
    }

    private void UpdateText()
    {
        value.text = $"{Mathf.CeilToInt(_healthPoints.current)} / {Mathf.CeilToInt(_healthPoints.max)}";
    }

    private void OnDeath()
    {
        if (_healthPoints != null)
        {
            _healthPoints.GetDamage -= Decrease;
            _healthPoints.Death -= OnDeath;
            _healthPoints.IncreaseEvent -= Increase;
        }

        CoroutineHolder.instance.AfterSeconds(destroyDelay, () =>
        {
            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        if (_healthPoints == null)
            return;

        _healthPoints.GetDamage -= Decrease;
        _healthPoints.Death -= OnDeath;
        _healthPoints.IncreaseEvent -= Increase;
    }
}