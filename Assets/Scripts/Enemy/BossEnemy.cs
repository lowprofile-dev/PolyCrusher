﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Event handler for a killed boss.
/// </summary>
/// <param name="boss">Boss who died.</param>
public delegate void BossKilledEventHandler(BossEnemy boss);

public class BossEnemy : BaseEnemy
{
    #region Inner Class
    /// <summary>
    /// Information class for the AI Phases.
    /// </summary>
    [System.Serializable]
    public class PhaseInformation
    {
        /// <summary>
        /// Specifies if the phase is enabled.
        /// </summary>
        public bool phaseEnabled;

        /// <summary>
        /// The probability of the phase.
        /// </summary>
        [Range(0f, 1f)]
        public float phaseProbability;

        [Tooltip("The time after the phase should be exited. (in seconds)")]
        [Range(0f, 30f)]
        public float phaseTime = 5f;
    }
    #endregion

    #region Member variables
    [Space(5)]
    [Header("Range attack values")]

    [SerializeField]
    [Tooltip("The damage of the ranged attack.")]
    protected int rangedAttackDamage = 10;

    [SerializeField]
    [Tooltip("The interval of the ranged attack")]
    protected float rangedAttackInterval = 0.5f;

    [SerializeField]
    [Tooltip("Range of the ranged attack")]
    protected float rangedAttackRange = 10f;

    [Space(5)]
    [Tooltip("Phase 1: Melee, Phase 2: Melee and Ranged, Phase 3: Melee, Ranged, Special Ability with mob spawn.")]
    [Header("Boss ability settings")]

    [SerializeField]
    protected PhaseInformation meleePhase;

    [Space(4)]
    [SerializeField]
    [Tooltip("Ranged attack phase")]
    protected PhaseInformation rangedPhase;

    [Space(4)]
    [SerializeField]
    [Tooltip("Special ability phase")]
    protected PhaseInformation specialPhase;

    [Space(4)]
    [Tooltip("Specifies if the boss is able to sprint or not.")]
    [SerializeField]
    protected bool sprintEnabled = false;

    [Space(4)]
    [Tooltip("Specifies if mobs can be spawned by the boss or not.")]
    [SerializeField]
    protected bool mobSpawnEnabled = false;

    [Space(4)]
    [Tooltip("Time which the boss stays in idle after he chooses another state.")]
    [SerializeField]
    protected float idleTime = 2f;

    [Space(5)]
    [Header("Attack visualization and settings")]
    [SerializeField]
    protected GameObject meleeAreaOfDamage;

    [Tooltip("Time of the area of damage.")]
    [SerializeField]
    protected float areaOfDamageTime = 1.5f;

    [SerializeField]
    [Tooltip("The damage radius of the attack.")]
    protected float areoOfDamageRadius = 5f;

    [Space(4)]
    [SerializeField]
    [Tooltip("The prefab reference to the boss bullet.")]
    protected GameObject rangedBullet;

    [SerializeField]
    [Tooltip("Number of bullets of the ranged")]
    protected int numberOfBullets = 8;
    
    [SerializeField]
    [Tooltip("Angle of the bullet spread.")]
    protected float spreadAngle = 70f;

    [Space(4)]
    [SerializeField]
    [Tooltip("Prefab of the meteor attack.")]
    protected GameObject meteorAttackPrefab;

    [SerializeField]
    [Tooltip("The spawn height of the meteorits.")]
    protected float meteorSpawnHeight = 10f;
    #endregion

    #region Properties
    /// <summary>
    /// Gets melee phase.
    /// </summary>
    public PhaseInformation MeleePhase
    {
        get { return this.meleePhase; }
    }

    /// <summary>
    /// Gets the ranged phase.
    /// </summary>
    public PhaseInformation RangedPhase
    {
        get { return this.rangedPhase; }
    }

    /// <summary>
    /// Gets the special phase
    /// </summary>
    public PhaseInformation SpecialPhase
    {
        get { return this.specialPhase; }
    }

    /// <summary>
    /// Gets sprint enabled.
    /// </summary>
    public bool SprintEnabled
    {
        get { return this.sprintEnabled; }
    }

    /// <summary>
    /// Gets mob spawn enabled.
    /// </summary>
    public bool MobSpawnEnabled
    {
        get { return this.mobSpawnEnabled; }
    }

    /// <summary>
    /// Gets the idle time.
    /// </summary>
    public float IdleTime
    {
        get { return this.idleTime; }
    }

    /// <summary>
    /// Gets the reference of the area of damage prefab.
    /// </summary>
    public GameObject MeleeAreaOfDamage
    {
        get { return this.meleeAreaOfDamage; }
    }

    /// <summary>
    /// Gets the area of damage time.
    /// </summary>
    public float AreaOfDamageTime
    {
        get { return this.areaOfDamageTime; }
    }

    /// <summary>
    /// Gets the area of damage radius.
    /// </summary>
    public float AreoOfDamageRadius
    {
        get { return this.areoOfDamageRadius; }
    }

    /// <summary>
    /// Gets the ranged attack range.
    /// </summary>
    public float RangedAttackRange
    {
        get { return this.rangedAttackDamage; }
    }

    /// <summary>
    /// Gets the ranged attack interval.
    /// </summary>
    public float RangedAttackInterval
    {
        get { return this.rangedAttackInterval; }
    }

    /// <summary>
    /// Gets the ranged bullet.
    /// </summary>
    public GameObject RangedBullet
    {
        get { return this.rangedBullet; }
    }

    /// <summary>
    /// Gets the number of bullets.
    /// </summary>
    public int NumberOfBullets
    {
        get { return this.numberOfBullets; }
    }

    /// <summary>
    /// Gets the spread angle of the bullets.
    /// </summary>
    public float SpreadAngle
    {
        get { return this.spreadAngle; }
    }

    /// <summary>
    /// Gets the meteor attack prefab.
    /// </summary>
    public GameObject MeteorAttackPrefab
    {
        get { return this.meteorAttackPrefab; }
    }

    /// <summary>
    /// Gets the meteor spawn height.
    /// </summary>
    public float MeteorSpawnHeight
    {
        get { return this.meteorSpawnHeight; }
    }
    #endregion

    // Event for a killed boss.
    public static event BossKilledEventHandler BossKilled;

    /// <summary>
    /// Initializes the FSM.
    /// </summary>
    protected override void MakeFSM()
    {
        // Set up FSM.
        BossIdle idle = new BossIdle(this);
        idle.AddTransition(Transition.DecisionMelee, StateID.BossAttackMelee);
        idle.AddTransition(Transition.DecisionRanged, StateID.BossAttackRanged);
        idle.AddTransition(Transition.DecisionSpecial, StateID.BossAttackSpecial);

        BossAttackMelee attackMelee = new BossAttackMelee(MeleePhase.phaseTime, StateID.BossAttackMelee);
        attackMelee.AddTransition(Transition.AttackFinished, StateID.BossIdle);
        attackMelee.AddTransition(Transition.LostPlayerAttackRange, StateID.BossWalk);
        
        BossWalk attackPhaseWalk = new BossWalk(AttackRange, StateID.BossWalk);
        attackPhaseWalk.AddTransition(Transition.ReachedDestination, StateID.BossAttackMelee);


        BossAttackRanged attackRanged = new BossAttackRanged(RangedPhase.phaseTime, StateID.BossAttackRanged);
        attackRanged.AddTransition(Transition.AttackFinished, StateID.BossIdle);
        attackRanged.AddTransition(Transition.LostPlayerAttackRange, StateID.BossWalkRanged);

        BossWalk attackRangedWalk = new BossWalk(RangedAttackRange, StateID.BossWalkRanged);
        attackRangedWalk.AddTransition(Transition.ReachedDestination, StateID.BossAttackRanged);


        BossAttackSpecial attackSpecial = new BossAttackSpecial(SpecialPhase.phaseTime, StateID.BossAttackSpecial);
        attackSpecial.AddTransition(Transition.AttackFinished, StateID.BossIdle);
        attackSpecial.AddTransition(Transition.LostPlayerAttackRange, StateID.BossWalkSpecial);

        BossWalk walkSpecial = new BossWalk(RangedAttackRange, StateID.BossWalkSpecial);
        walkSpecial.AddTransition(Transition.ReachedDestination, StateID.BossAttackSpecial);

        fsm = new FSMSystem();
        fsm.AddState(idle);
        fsm.AddState(attackMelee);
        fsm.AddState(attackPhaseWalk);
        fsm.AddState(attackRanged);
        fsm.AddState(attackRangedWalk);
        fsm.AddState(attackSpecial);
        fsm.AddState(walkSpecial);
    }

    /// <summary>
    /// Melee Attack behaviour.
    /// </summary>
    public override void Attack()
    {
        
    }

    /// <summary>
    /// Ranged Attack behaviour.
    /// </summary>
    public override void Shoot()
    {
        
    }

    protected override void DestroyEnemy()
    {
        //Disable
        targetPlayer = null;
        GetComponent<NavMeshAgent>().Stop();
        GetComponent<NavMeshAgent>().updateRotation = false;
        GetComponent<NavMeshAgent>().obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        //Animation
        if (anim != null)
            anim.SetBool("Death", true);

        //Event.
        OnBossKilled();

        //Scale Fade out
        StartCoroutine(transform.ScaleFrom(Vector3.zero, lifeTimeAfterDeath, AnimCurveContainer.AnimCurve.downscale.Evaluate));

        //Destroy
        Destroy(this.gameObject, lifeTimeAfterDeath);
    }

    /// <summary>
    /// Event method for the Boss kill event.
    /// </summary>
    protected virtual void OnBossKilled()
    {
        if (BossKilled != null)
            BossKilled(this);
    }
}
