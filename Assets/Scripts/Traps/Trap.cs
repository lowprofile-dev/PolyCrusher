﻿using UnityEngine;
using System.Collections;
using System;

public class Trap : MonoBehaviour,ITriggerable {

    #region Class Members

    //how long the trap is active
    [SerializeField]
    public float trapActiveTime = 0.5f;

    //the triggers that are connected to the trap
    [SerializeField]
    protected Trigger[] triggers;

    //the player meshes that are used for the poly explosion
    [SerializeField]
    public GameObject[] playerMeshes;
    
    //specifies if the trap gives boss damage
    [SerializeField]
    public int bossDamage = 0;

    //trap can only be triggered if this is false
    protected bool isActive = false;

    #endregion


    #region Class Methods

    //trigger method that manages kill & trap animation, will be overwritten for every individual kind of trap
    public virtual void Trigger(Collider other){}

    //keeps the trap from triggering too often
    protected virtual IEnumerator WaitForActive()
    {
        yield return new WaitForSeconds(trapActiveTime);
        ResetTrap();
    }

    //resets the trigger manually after trap was triggered - deathTraps can't be exited so onTriggerExit won't be called
    protected IEnumerator WaitForReset()
    {
        yield return new WaitForSeconds(trapActiveTime);
        triggers[0].resetTrigger();
    }

    //resets trap
    protected virtual void ResetTrap()
    {
        isActive = false;
    }

    //sets trap active false on awake
    public void Awake()
    {
        ResetTrap();
    }

    //calls the trigger method if all triggers are active with reference on the collider that entered the very FIRST trigger
    public void Update()
    {

        int counter = 0;
        for(int i = 0; i<triggers.Length; i++)
        {
            if (triggers[i].collided != null)
            {
                counter++;
            }
        }

        if(counter == triggers.Length)
        {
            if (isActive == false)
            {
                isActive = true;
                StartCoroutine(WaitForActive());
                Trigger(triggers[0].collided);
            }
        }
    }

    #endregion
}
