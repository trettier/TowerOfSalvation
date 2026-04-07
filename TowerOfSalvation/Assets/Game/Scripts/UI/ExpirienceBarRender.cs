using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ExpirienceBarRender : MonoBehaviour
{
    public Image full;
    public Image difference;
    public Image current;

    public TMP_Text value;

    private Level _level;
    private HealthPoints _hp;

    [SerializeField] private float smoothSpeed = 2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.3f, 0);
    [SerializeField] private float destroyDelay = 0.2f;

    private Coroutine _diffCoroutine;

    private Transform _character;
    public void Initialize(Transform character, Level level, HealthPoints hp)
    {
        _character = character;

        _level = level;
        _hp = hp;

        _level.expirience.Updated += Increase;
        _level.expirience.Full += OnExpirienceFull;
        _hp.Death += OnDeath;

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
        float normalized = _level.expirience.current / _level.expirience.max;

        current.fillAmount = normalized;
        difference.fillAmount = normalized;

        UpdateText();
    }

    private void Decrease()
    {
        float normalized = _level.expirience.current / _level.expirience.max;

        current.fillAmount = normalized;

        if (_diffCoroutine != null)
            StopCoroutine(_diffCoroutine);

        _diffCoroutine = StartCoroutine(SmoothDecrease(normalized));

        UpdateText();
    }

    private void OnExpirienceFull()
    {

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
        float normalized = _level.expirience.current / _level.expirience.max;

        // при хиле оба бара сразу обновляем
        current.fillAmount = normalized;
        difference.fillAmount = normalized;

        UpdateText();
    }

    private void UpdateText()
    {
        value.text = $"{Mathf.CeilToInt(_level.expirience.current)} / {Mathf.CeilToInt(_level.expirience.max)}";
    }

    private void OnDeath()
    {
        if (_level != null)
        {
            _level.expirience.Updated -= Increase;
            _level.expirience.Full -= OnExpirienceFull;
            _hp.Death -= OnDeath;
        }

        CoroutineHolder.instance.AfterSeconds(destroyDelay, () =>
        {
            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        if (_level == null)
            return;

        _level.expirience.Updated -= Increase;
        _level.expirience.Full -= OnExpirienceFull;
        _hp.Death -= OnDeath;
    }
}