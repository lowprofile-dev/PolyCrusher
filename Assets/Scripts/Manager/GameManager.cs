﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Eventhandler for the wave start.
/// </summary>
public delegate void WaveStartedEventHandler();

/// <summary>
/// Eventhandler for the wave end.
/// </summary>
public delegate void WaveEndedEventHandler();

/// <summary>
/// Implements the wave system and the spawning decision for the enemies.
/// The game manager is implemented with the Singleton-Pattern.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Inner classes
    /// <summary>
    /// Spawn information for the enemies.
    // </summary>
    [System.Serializable]
    public class SpawnInformation
    {
        // Enemy to spawn.
        public GameObject enemy;

        // Probability to spawn. (Range: 0 - 1);
        public float spawnProbability;

        // Ressource value for the enemy.
        public int enemyRessourceValue = 1;

        // Minimum wave to spawn.
        public float minWave;

        // The name of the enemy.
        public string enemyName = "Enemy";

        // The actual health. This value will be used to increase the health per wave.
        protected int actualHealth;

        // The actual health. This value will be used to increase the health per wave.
        protected int actualDamage;


        /// <summary>
        /// Gets or sets the actual health.
        /// </summary>
        public int ActualHealth
        {
            get { return this.actualHealth; }
            set { this.actualHealth = value; }
        }

        /// <summary>
        /// Gets or sets the actual damage.
        /// </summary>
        public int ActualDamage
        {
            get { return this.actualDamage; }
            set { this.actualDamage = value; }
        }
    }
    #endregion

    #region Class Members

    [Header("==========Spawn Information============")]
    // Spawn information.
    [SerializeField]
    protected SpawnInformation[] spawnInfo;

    // Represenation of the game manager
    public static GameManager gameManagerInstance;

    // The spawn points of the enemies.
    protected GameObject[] enemySpawnPoints;

    // Event handler for the wave start.
    public static event WaveStartedEventHandler WaveStarted;

    // Event handler for the wave end.
    public static event WaveEndedEventHandler WaveEnded;

    // The wave count.
    protected int wave = 0;

    [Header("==========Ressource Settings============")]
    // The max enemy ressources
    [SerializeField]
    protected int enemyRessourcePool = 10;

    // Current ressource pool of the enemies.
    [SerializeField]
    protected int currentEnemyRessourceValue;

    // The accumulated ressource value.
    protected int accumulatedRessourceValue;

    [Header("==========Enemy Count Settings==========")]
    // Describes how many enemies are allowed to be active at the same time.
    [SerializeField]
    protected int maxEnemyActiveCount = 10;

    // The factor for the enemy active count increase.
    protected float maxEnemyActiveCountFactor = 1.3f;

    // The current enemy count.
    [SerializeField]
    protected int currentEnemyCount = 0;

    // Time between the waves.
    [SerializeField]
    protected float timeBetweenWave = 5f;

    // Determines if the wave is active or if there is a pause.
    protected bool waveActive = false;

    #region Increase per wave variables

    [Header("=========Wave Increase Settings=========")]
    [Tooltip("The increase factor of the enemy ressources for every wave. Value should be between 0 and 1!")]
    [SerializeField]
    protected float enemyRessourceIncreaseFactor = 0.2f;

    [Tooltip("The increase factor of the enemy count for every wave. Value should be between 0 and 1!")]
    [SerializeField]
    protected float enemyCountIncreaseFactor = 0.2f;

    [Space(10f)]

    [Tooltip("The decrease factor of the time between the waves. Value should be between 0 and 1!")]
    [SerializeField]
    protected float timeBetweeenWaveDecreaseFactor = 0.05f;

    [Space(10f)]

    [Tooltip("The increase factor of the enemy health for every wave. Value should be between 0 and 1!")]
    [SerializeField]
    protected float enemyHealthIncreaseFactor = 0.1f;

    [Tooltip("The increase factor of the enemy damage for every wave. Value should be between 0 and 1!")]
    [SerializeField]
    protected float enemyDamageIncreaseFactor = 0.1f;
    #endregion

    #endregion

    #region Properties
    /// <summary>
    /// Public reference of the game manager.
    /// </summary>
    public static GameManager GameManagerInstance
    {
        get
        {
            //If the instance isn't set yet, it will be set (Happens only the first time!)
            if (gameManagerInstance == null)
                gameManagerInstance = GameObject.FindObjectOfType<GameManager>();

            return gameManagerInstance;
        }
    }

    /// <summary>
    /// Gets the actual wave.
    /// </summary>
    public int Wave
    {
        get { return this.wave; }
    }

    /// <summary>
    /// Gets or sets the enemy count pool.
    /// </summary>
    public int EnemyRessourcePool
    {
        get { return this.enemyRessourcePool; }
        set { this.enemyRessourcePool = value; }
    }

    /// <summary>
    /// Gets or sets the enemy active count.
    /// </summary>
    public int MaxEnemyActiveCount
    {
        get { return this.maxEnemyActiveCount; }
        set { this.maxEnemyActiveCount = value; }
    }
    
    /// <summary>
    /// Gets or sets the enemy active count factor.
    /// </summary>
    public float MaxEnemyActiveCountFactor
    {
        get { return this.maxEnemyActiveCountFactor; }
        set { this.maxEnemyActiveCountFactor = value; }
    }

    /// <summary>
    /// Gets or sets the current enemy ressource pool.
    /// </summary>
    public int CurrentEnemyRessourceValue
    {
        get { return this.currentEnemyRessourceValue; }
        set
        {
            this.currentEnemyRessourceValue = value;

            if (this.currentEnemyRessourceValue == 0 && this.currentEnemyCount == 0)
            {
                Debug.Log("GameManager: No ressources and 0 enemies left -> Wave ended.");
                EndWave();
            }
        }
    }

    /// <summary>
    /// Gets or sets the current enemy count.
    /// </summary>
    public int CurrentEnemyCount
    {
        get { return this.currentEnemyCount; }
        protected set 
        {
            this.currentEnemyCount = value;

            if (this.currentEnemyRessourceValue == 0 && this.currentEnemyCount == 0)
            {
                Debug.Log("GameManager: No ressources and 0 enemies left -> Wave ended.");
                EndWave();
            }
        }
    }

    /// <summary>
    /// Gets the wave active state.
    /// </summary>
    public bool WaveActive
    {
        get { return this.waveActive; }
        protected set
        {
            this.waveActive = value;
        }
    }

    /// <summary>
    /// Gets the spawn information.
    /// </summary>
    public SpawnInformation[] SpawnInfo
    {
        get { return this.spawnInfo; }
    }

    /// <summary>
    /// Gets the accumulated ressource value.
    /// </summary>
    public int AccumulatedRessourceValue
    {
        get { return this.accumulatedRessourceValue; }
    }

    #endregion

    #region Methods
    void Awake()
    {
        LevelEndManager.levelExitEvent += ResetValues;
    }

    // Use this for initialization
	void Start () 
    {
        // Search for enemy spawn points.
        enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawn");

        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
            enemySpawnPoints = null;

        // Register event methods.
        EnemySpawn.EnemySpawned += EnemySpawned;
        BaseEnemy.EnemyKilled += EnemyDied;

        //Proces Enemy information.
        ProcessEnemyData();

        //Trigger first waiting sequence. This will start the first wave.
        StartCoroutine(WaitForNextWave());

        // Init variables
        accumulatedRessourceValue = 0;
	}

    /// <summary>
    /// Starts a new wave.
    /// </summary>
    protected void StartNextWave()
    {
        WaveActive = true;
        this.wave++;

        //Debug.
        Debug.Log("GameManager: Wave " + wave + " start!");

        // If the wave is beyond 1, increase the settings to make the game harder.
        if (Wave > 1)
        {
            CalculateNextWaveValues();
        }

        CurrentEnemyRessourceValue = EnemyRessourcePool;

        OnWaveStarted();
    }

    /// <summary>
    /// Increases specific values to increase the game difficulty.
    /// </summary>
    protected void CalculateNextWaveValues()
    {
        // Increase the enemy ressources.
        EnemyRessourcePool += (int) (EnemyRessourcePool * enemyRessourceIncreaseFactor);

        // Increase the enemy count.
        MaxEnemyActiveCount += (int) (MaxEnemyActiveCount * enemyCountIncreaseFactor);

        // Decrease time between wave.
        timeBetweenWave *= (1f - timeBetweeenWaveDecreaseFactor);

        // Increase enemy damage and health.
        for (int i = 0; i < spawnInfo.Length; i++)
        {
            spawnInfo[i].ActualHealth += (int) (spawnInfo[i].ActualHealth * enemyHealthIncreaseFactor);
            spawnInfo[i].ActualDamage += (int) (spawnInfo[i].ActualDamage * enemyDamageIncreaseFactor);
        }

        // Set the accumulated ressource value back to 0.
        accumulatedRessourceValue = 0;
    }

    /// <summary>
    /// Processes the enemy data and saves the initial health and damage values into the spawn information.
    /// </summary>
    protected void ProcessEnemyData()
    {
        for (int i = 0; i < spawnInfo.Length; i++)
        {
            spawnInfo[i].ActualHealth = spawnInfo[i].enemy.GetComponent<BaseEnemy>().MaxHealth;
            spawnInfo[i].ActualDamage = spawnInfo[i].enemy.GetComponent<BaseEnemy>().MeleeAttackDamage;
            //Debug.Log(spawnInfo[i].ActualDamage);
        }
    }

    /// <summary>
    /// Ends a wave.
    /// </summary>
    protected void EndWave()
    {
        WaveActive = false;
        StartCoroutine(WaitForNextWave());

        OnWaveEnded();
    }

    /// <summary>
    /// Event method for the wave start.
    /// </summary>
    protected virtual void OnWaveStarted()
    {
        if (WaveStarted != null)
            WaveStarted();
    }

    /// <summary>
    /// Event method for the wave end.
    /// </summary>
    protected virtual void OnWaveEnded()
    {
        if (WaveEnded != null)
            WaveEnded();
    }

    /// <summary>
    /// Increases the current enemy count.
    /// Event handler method.
    /// </summary>
    protected void EnemySpawned()
    {
        this.CurrentEnemyCount++;
    }

    /// <summary>
    /// Decreases the enemy count.
    /// </summary>
    protected void EnemyDied(BaseEnemy enemy)
    {
        this.CurrentEnemyCount--;

        // Add the ressource value
        for (int i = 0; i < spawnInfo.Length; i++)
        {
            MonoBehaviour m = spawnInfo[i].enemy.GetComponent<MonoBehaviour>();

            if (m != null && m is BaseEnemy)
            {
                BaseEnemy e = m as BaseEnemy;

                // Add the ressource value if the name is equal.
                if (e.EnemyName == enemy.EnemyName)
                    this.accumulatedRessourceValue += spawnInfo[i].enemyRessourceValue;
            }
        }
    }

    /// <summary>
    /// Waits for the next wave.
    /// </summary>
    protected IEnumerator WaitForNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWave);
        StartNextWave();
    }

    /// <summary>
    /// Resets all neccessary values.
    /// </summary>
    protected void ResetValues()
    {
        WaveStarted = null;
        WaveEnded = null;
    }
    #endregion
}