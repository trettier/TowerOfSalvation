using UnityEngine;
public class Death : CharacterState
{
    public override void Enter(CharacterPresenter presenter)
    {
        base.Enter(presenter);

        view.animator.SetBool("move", false);
        view.animator.SetBool("death", true);
        view.spriteRenderer.sortingLayerName = "floor";
        view.spriteRenderer.sortingOrder = 2;
        view.sound.StepStop();
        view.sound.Dead();

        view.gameObject.tag = "dead";
        view.GetComponent<CapsuleCollider2D>().enabled = false;
        view.vision.enabled = false;

        view.weapon.OnDeath();
        view.OnDeath();
    }
}