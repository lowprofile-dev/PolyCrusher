﻿using UnityEngine;

public class CharacterMenuManager : MenuManager
{
    [SerializeField]
    private PlayerSlot playerSlot;

    [SerializeField]
    private CharacterSelectionHelper selectionHelper;

    public PlayerSlot PlayerSlot
    {
        get { return this.playerSlot; }
    }

    protected override void Start()
    {

        PlayerActionChanged += HandlePlayerActionChanged;
        InitializeMenuManager();
        InitializePlayerControlActions();

        if (selectionHelper == null)
            Debug.LogError("<b>Selection helper is not assigned!</b>");
    }

    private void HandlePlayerActionChanged(PlayerControlActions playerAction)
    {
        if (playerAction.IsNullAction())
        {
            foreach (var pair in MenuComponents)
            {
                RectTransform r = pair.Value.GetComponent<RectTransform>();
                r.localScale = Vector3.zero;
            }
            Debug.Log("<b>No Player registered! Hide Menu.</b>");
        }
        else
        {
            // Fade in Menu -> Controller registered
            StartCoroutine(TriggerMenuSpawnTween(() => {
                InitializeComponentSize();
            }));

            Debug.Log("<b>" + PlayerSlot + " registered!</b>");
        }
    }

    private void InitializeComponentSize()
    {
        foreach (var pair in components)
        {
            NavigationInformation info = pair.Value.GetComponent<NavigationInformation>();
            RectTransform rect = pair.Value.GetComponent<RectTransform>();
            if (pair.Key != Selector.Current)
                rect.localScale = Vector3.zero;
            else
                LeanTween.scale(rect, info.OriginalScale, menuSpawnTweenTime);
        }
    }

    protected override void InitializePlayerControlActions()
    {
        SetPlayerControlActions(PlayerControlActions.CreateNullBinding());
    }

    protected override void InitializeSelector()
    {
        TransitionHandlerInterface[] pickedTransitions = MenuReloadedUtil.MapTransitionEnumToHandler(transitions);
        ElementPressedHandler[] pickedPressedHandler = MenuReloadedUtil.MapElementPressedEnumToHandler(pressedHandlerEnum);

        selector = new CharacterSelector(startIndex, components, pickedTransitions, pickedPressedHandler, selectionHelper, this, false);
    }

    /// <summary>
    /// Deselects if possible.
    /// </summary>
    public void Deselect()
    {
        // Selector must be a 'CharacterSelector' therefore this cast should be safe
        ((CharacterSelector)selector).Deselect();
    }
}