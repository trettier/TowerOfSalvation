using DG.Tweening;
using Goldmetal.UndeadSurvivor;
using System;
using TMPro;
using TowerOfSalvation.Effects;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Damage : MonoBehaviour
{

    public GameObject textPfb;
    public Vector3 textOffset = new Vector3(0, 1, 0);
    public Canvas canvas;

    [Serializable]
    public class Parameters
    {
        public float value;
        public float increment;
        public Critical crit;
        public Knockback knockback;

        public Parameters(float value, float increment, Critical critical, Knockback knockback)
        {
            this.value = value;
            this.increment = increment;
            this.crit = critical;
            this.knockback = knockback;
        }

        public void IncreaseValue(float value)
        {
            this.value += value;
        }

        public void IncreaseIncrement(float value)
        {
            this.increment += value;
        }
    }

    [Serializable]
    public class Critical
    {
        public float chance;
        public float multiplier;

        public Critical(float chance, float multiplier)
        {
            this.chance = chance;
            this.multiplier = multiplier;
        }
    }

    [Serializable]
    public class Knockback
    {
        public float value;

        public Knockback(float value)
        {
            this.value = value;
        }
    }

    private void Start()
    {
        DOTween.Init();
    }

    public void Deal(Projectile source, CharacterView target, Vector3 direction, GameObject vfx = null)
    {
        float damage = Calculate(source.model.damage);

        target.presenter.RecieveDamage(damage, source._owner.presenter.owner.presenter);
        target.presenter.RecieveKnockback(direction * source.model.damage.knockback.value);
        Visualize(target.transform, damage, vfx);
    }

    public void Deal(CharacterView source, CharacterView target, WeaponModel weapon, GameObject vfx = null)
    {
        float damage = Calculate(weapon.damage);
        Vector3 knockback = CalculateKnockback(source.transform, target.transform, weapon.damage.knockback);

        target.presenter.RecieveDamage(damage, source.presenter);
        target.presenter.RecieveKnockback(knockback);
        Visualize(target.transform, damage, vfx);
    }

    public static Vector3 CalculateKnockback(Transform source, Transform target, Knockback knockback)
    {
        return (target.position - source.position).normalized * knockback.value;
    }

    public float Calculate(Parameters parameters)
    {
        float damage = UnityEngine.Random.Range(parameters.value, parameters.value + parameters.increment);

        if (parameters.crit.chance * 100 > UnityEngine.Random.Range(0, 100))
        {
            return damage * parameters.crit.multiplier;
        }
        return damage;
    }

    public void Visualize(Transform target, float damage, GameObject vfx = null)
    {
        Vector3 startPosition = target.transform.position + textOffset;
        var text = Instantiate(textPfb, startPosition, Quaternion.identity, canvas.transform);
        text.GetComponent<TMP_Text>().text = ((int)damage).ToString();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(text.transform.DOMove(startPosition + textOffset, 0.5f));

        sequence.AppendInterval(0.6f);

        sequence.OnComplete(() => Destroy(text.gameObject));

        //if (vfx == null)
        //    return;

        Vector3 vfxPosition = target.transform.position + Vector3.up * 0.5f;

        VisualEffectsFactory.instance.SpawnEffect(VisualEffectsFactory.EffectType.Hit, vfxPosition);
        //Instantiate(controller.hit, vfxPosition, Quaternion.identity);
    }
}