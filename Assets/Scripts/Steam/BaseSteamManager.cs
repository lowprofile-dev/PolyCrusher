﻿using UnityEngine;
using System.Collections;

//
// ISteamManager is the base class of SteamManager and SteamManagerDummy and regulates the initialization of those.
// The dummy class is initialized instead of the manager class when Unity is running in editor mode (and WIN Standalone atm for all following test builds).
// If testing the SteamManager, please enable your Steam Standalone and comment lines 17 and 19-21.
//
[DisallowMultipleComponent]
public class BaseSteamManager : MonoBehaviour {

    protected static BaseSteamManager instance;
    public static BaseSteamManager Instance
    {
        get
        {
            #if !UNITY_EDITOR && !UNITY_STANDALONE_WIN
                return instance ?? new GameObject("SteamManager").AddComponent<SteamManager>();
            #else
                return instance ?? new GameObject("SteamManagerDummy").AddComponent<SteamManagerDummy>();
            #endif
        }
    }

    protected static bool everInitialized;

    protected bool initialized;
    public static bool Initialized
    {
        get
        {
            return Instance.initialized;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (everInitialized)
        {
            throw new System.Exception("Tried to Initialize the SteamManager twice in one session!");
        }

        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (instance != this)
        {
            return;
        }

        instance = null;

        if (!initialized)
        {
            return;
        }
    }

    protected virtual void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (!initialized)
        {
            return;
        }
    }

    public virtual void LogAchievementEvent (Event e) {}
	
	public virtual void LogAchievementData (AchievementID id) {}

    public virtual string GetSteamName(){ return ""; }

    public virtual string GetSteamID() { return ""; }

    public virtual int GetRank() { return 0; }
}

//
// This is the SteamManagerDummy class, used in editor mode.
//
[DisallowMultipleComponent]
class SteamManagerDummy : BaseSteamManager
{
    protected override void Awake()
    {
        base.Awake();

        initialized = true;
        everInitialized = true;
    }
}