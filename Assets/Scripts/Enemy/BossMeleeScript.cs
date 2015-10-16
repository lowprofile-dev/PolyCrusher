﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Uses the area of damage object and deals damage when it reaches it's max size.
/// All players in the circle get a damage and will be pushed outside.
/// </summary>
public class BossMeleeScript : MonoBehaviour
{
    // Radius of the damage area.
    public float damageRadius;

    // Damage of the attack.
    public int damage;

    // Time until damage will taken.
    public float activationTime;

    // Current timer value.
    private float currentTime;

    // Specifies if the script has been initialized.
    public bool attackStarted;

    // Layer of the players
    private int playerLayer = 8;

    // Owner
    BossEnemy owner;

    // Use this for initialization
    void Start ()
    {
        transform.localScale = Vector3.zero;
        //this.attackStarted = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (this.attackStarted)
        {
            // TODO: attack started cant be set to true
            if (currentTime >= activationTime)
            {
                
                DealDamage();
                currentTime = 0;
                //StartCoroutine(transform.ScaleTo(Vector3.zero, 0.2f, AnimCurveContainer.AnimCurve.downscale.Evaluate));
                //Destroy(this, 0.28f);
                Destroy(this.gameObject);
            }

            currentTime += Time.deltaTime;
        }
	}

    /// <summary>
    /// Initializes the behaviour.
    /// </summary>
    public void InitMeleeScript(float damageRadius, float activationTime, BossEnemy owener, int damage)
    {
        this.attackStarted = true;
        Debug.Log("Attack started: " + attackStarted);
        this.damageRadius = damageRadius;
        this.activationTime = activationTime;
        this.owner = owener;
        this.damage = damage;

        StartCoroutine(transform.ScaleTo(new Vector3(damageRadius, damageRadius, 0.3f), activationTime, Ease.CubeOut));
    }

    /// <summary>
    /// Deals damage to all players inside the radius.
    /// </summary>
    private void DealDamage()
    {
        Transform[] players = GetAllPlayersInRadius(damageRadius);

        for (int i = 0; i < players.Length; i++)
        {
            MonoBehaviour m = players[i].GetComponent<MonoBehaviour>();

            if (m is BasePlayer)
            {
                m.GetComponent<BasePlayer>().TakeDamage(damage, owner);
                m.GetComponent<Rigidbody>().AddExplosionForce(owner.PushAwayForce, transform.position, damageRadius);
            }
        }

        // TODO: Play sound
        // TODO: Spawn Particle
    }

    private Transform[] GetAllPlayersInRadius(float radius)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, 1 << playerLayer);
        Transform[] players = new Transform[hits.Length];

        for (int i = 0; i < hits.Length; i++)
            players[i] = hits[i].transform;

        return players;
    }
}
