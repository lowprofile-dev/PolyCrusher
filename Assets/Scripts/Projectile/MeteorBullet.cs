﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Meteor bullet behaviour.
/// </summary>
public class MeteorBullet : Bullet
{
    [Space(4)]
    [Header("Area of Damage instance.")]
    [SerializeField]
    protected GameObject areaOfDamage;


    /// <summary>
    /// Spawns the area of damage.
    /// </summary>
    protected void SpawnAreaOfDamage()
    {
        if (areaOfDamage != null)
        {
            //Spawn directly on the NavMesh
            NavMeshHit hit;

            // Sample bullet position on NavMesh.
            bool posFound = NavMesh.SamplePosition(transform.position, out hit, 7f, NavMesh.AllAreas);

            // Only instantiate if position was found
            if (posFound)
            {
                if (this.OwnerScript.GetComponent<BossEnemy>() != null)
                {
                    // Instantiate
                    GameObject o = Instantiate(areaOfDamage) as GameObject;
                    o.transform.position = hit.position;
                    o.GetComponent<BossMeleeScript>().InitMeleeScript(this.OwnerScript.GetComponent<BossEnemy>());
                }
            }
        }
    }

    /// <summary>
    /// On trigger enter bullet behaviour.
    /// </summary>
    /// <param name="other">Collider</param>
    void OnTriggerEnter(Collider other)
    {
        // Check if the hit target was hit.
        if (other.tag == targetTag)
        {
            MonoBehaviour m = other.gameObject.GetComponent<MonoBehaviour>();

            if (m != null && m is IDamageable)
            {
                // Apply damage.
                ((IDamageable)m).TakeDamage(Damage, this.OwnerScript);

                // Death area.
                SpawnAreaOfDamage();

                SpawnDeathParticle(transform.position);
                ApplyExplosionForce(other.gameObject, transform.position);



                Destroy(this.gameObject);
            }
        }

        if (other.tag == "Terrain" || other.gameObject.layer == LayerMask.NameToLayer("Props"))
        {
            // Death area.
            SpawnAreaOfDamage();

            SpawnDeathParticle(transform.position);
            Destroy(this.gameObject);
        }
    }
}
