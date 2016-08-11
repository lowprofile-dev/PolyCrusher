﻿using UnityEngine;
using System.Collections;
using InControl;

public class PlayerControlActions : PlayerActionSet
{

    public PlayerAction Join;
    public PlayerAction Back;
    public PlayerAction Pause;
    public PlayerAction Ability;
    public PlayerAction LeftLeft;
    public PlayerAction LeftRight;
    public PlayerAction LeftUp;
    public PlayerAction LeftDown;
    public PlayerAction RightLeft;
    public PlayerAction RightRight;
    public PlayerAction RightUp;
    public PlayerAction RightDown;
    public PlayerOneAxisAction LeftHorizontal;
    public PlayerOneAxisAction LeftVertical;
    public PlayerOneAxisAction RightHorizontal;
    public PlayerOneAxisAction RightVertical;

    // Use this for initialization
    public PlayerControlActions () {
        Join = CreatePlayerAction("Join");
        Back = CreatePlayerAction("Back");
        Pause = CreatePlayerAction("Pause");
        Ability = CreatePlayerAction("Ability");
        LeftLeft = CreatePlayerAction("LeftLeft");
        LeftRight = CreatePlayerAction("LeftRight");
        LeftUp = CreatePlayerAction("LeftUp");
        LeftDown = CreatePlayerAction("LeftDown");
        RightLeft = CreatePlayerAction("RightLeft");
        RightRight = CreatePlayerAction("RightRight");
        RightUp = CreatePlayerAction("RightUp");
        RightDown = CreatePlayerAction("RightDown");
        LeftHorizontal = CreateOneAxisPlayerAction(LeftLeft, LeftRight);
        LeftVertical = CreateOneAxisPlayerAction(LeftUp, LeftDown);
        RightHorizontal = CreateOneAxisPlayerAction(RightLeft, RightRight);
        RightVertical = CreateOneAxisPlayerAction(RightUp, RightDown);

    }

    //Keyboard Controls
    public static PlayerControlActions CreateWithKeyboardBindings()
    {
        PlayerControlActions p = new PlayerControlActions();
        p.Join.AddDefaultBinding(Key.Return);
        p.Back.AddDefaultBinding(Key.Backspace);
        p.Pause.AddDefaultBinding(Key.Escape);
        p.Ability.AddDefaultBinding(Key.Space);
        p.LeftLeft.AddDefaultBinding(Key.A);
        p.LeftRight.AddDefaultBinding(Key.D);
        p.LeftUp.AddDefaultBinding(Key.W);
        p.LeftDown.AddDefaultBinding(Key.S);
        p.RightLeft.AddDefaultBinding(Key.LeftArrow);
        p.RightRight.AddDefaultBinding(Key.RightArrow);
        p.RightUp.AddDefaultBinding(Key.UpArrow);
        p.RightDown.AddDefaultBinding(Key.DownArrow);
        return p;
    }


    //GamePad Controls
    public static PlayerControlActions CreateWithGamePadBindings()
    {
        PlayerControlActions p = new PlayerControlActions();
        
        p.Join.AddDefaultBinding(InputControlType.Action1);
        p.Back.AddDefaultBinding(InputControlType.Action2);
        //xbox one specific
        p.Pause.AddDefaultBinding(InputControlType.Menu);
        //xbox 360 specific
        p.Pause.AddDefaultBinding(InputControlType.Start);       
        p.Ability.AddDefaultBinding(InputControlType.LeftBumper);       
        p.LeftLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
        p.LeftRight.AddDefaultBinding(InputControlType.LeftStickRight);
        p.LeftUp.AddDefaultBinding(InputControlType.LeftStickUp);
        p.LeftDown.AddDefaultBinding(InputControlType.LeftStickDown);
        p.RightLeft.AddDefaultBinding(InputControlType.RightStickLeft);
        p.RightRight.AddDefaultBinding(InputControlType.RightStickRight);
        p.RightUp.AddDefaultBinding(InputControlType.RightStickUp);
        p.RightDown.AddDefaultBinding(InputControlType.RightStickDown);
        return p;
    }

    
}