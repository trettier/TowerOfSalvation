using System;
using UnityEngine;

public class MouseInteractions : MonoBehaviour
{
    private CharacterInteractionState currentState;
    public CharacterSelection selection;

    private void Start()
    {
        ChangeState(new IdleState());
    }

    private void Update()
    {
        UpdateCursor();
        var newSelection = GetCharacterUnderCursor();
        if (selection != newSelection)
            ChangeSelection(newSelection);

        if (currentState != null) 
            currentState.Update();
    }

    public void ChangeState(CharacterInteractionState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Init(this);
        currentState.Enter();
    }

    public Vector2 CursorPosition { get; private set; }

    public void UpdateCursor()
    {
        CursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public CharacterSelection GetCharacterUnderCursor()
    {
        RaycastHit2D hit = Physics2D.Raycast(CursorPosition, Vector2.zero, 1, LayerMask.GetMask("character"));
        return hit.collider ? hit.collider.GetComponent<CharacterSelection>() : null;
    }

    public CharactersZone GetZoneUnder(CharacterSelection selection)
    {
        RaycastHit2D hit = Physics2D.Raycast(selection.transform.position, Vector2.zero, 1, LayerMask.GetMask("zone"));
        return hit.collider ? hit.collider.GetComponent<CharactersZone>() : null;
    }

    public CharacterPanel ShowTooltip(CharacterSelection selection)
    {
        return G.instance.uiFactory.CreateCharacterPanel(selection.view.presenter);
    }

    public void ChangeSelection(CharacterSelection newSelection)
    {
        if (selection != null)
            selection.Deselect();
        selection = newSelection;
        if (selection != null)
            selection.Select();
    }
}
public abstract class CharacterInteractionState
{
    protected MouseInteractions context;

    public void Init(MouseInteractions context)
    {
        this.context = context;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

public class IdleState : CharacterInteractionState
{
    public override void Update()
    {
        if (context.selection != null)
        {
            context.ChangeState(new HoverState());
        }
    }
}

public class HoverState : CharacterInteractionState
{
    private CharacterSelection _selection;
    private float _hoverTimer;
    private float _hoverDelay = 0.5f;

    private CharacterPanel _tooltip;

    private bool isTooltipAvailable = true;

    public override void Enter()
    {
        _selection = context.selection;
        _hoverTimer = 0;
    }

    public override void Update()
    {

        // Hover таймер
        if (_tooltip == null)
        {
            if (context.selection != _selection)
            {
                context.ChangeState(new IdleState());
                return;
            }

            _hoverTimer += Time.deltaTime;

            if (_hoverTimer >= _hoverDelay && isTooltipAvailable)
            {
                _tooltip = context.ShowTooltip(_selection);
                _tooltip.Exit += OnTooltipDestroy;
                _hoverTimer = 0; // чтобы не спамило
                isTooltipAvailable = false;
            }
        }

        //// Нажатие мыши
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnMousePressed();
        }
    }

    public void OnTooltipDestroy()
    {
        if (_tooltip != null)
            _tooltip.Exit -= OnTooltipDestroy;

        isTooltipAvailable = true;
    }

    private void OnMousePressed()
    {
        if (context.selection != null)
        {
            context.ChangeState(new DragState());
        }
    }
}

public class DragState : CharacterInteractionState
{
    private CharacterSelection selection;
    private CharactersZone originZone;
    private Vector2 offset;

    public override void Enter()
    {
        selection = context.selection;
        originZone = context.GetZoneUnder(selection);
        offset = (Vector2)selection.transform.position - context.CursorPosition;
    }

    public override void Update()
    {
        selection.view.Drag(context.CursorPosition + offset);

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Release();
            context.ChangeState(new IdleState());
        }
    }

    private void Release()
    {
        var newZone = context.GetZoneUnder(selection);

        if (newZone == null)
        {
            originZone.ReturnCharacter(selection.view);
            return;
        }

        if (originZone == newZone)
        {
            newZone.ChangePosition(selection.view);
            return;
        }

        if (!CharactersZone.TryChangeZone(originZone, newZone, selection.view))
        {
            originZone.ReturnCharacter(selection.view);
        }
    }
}