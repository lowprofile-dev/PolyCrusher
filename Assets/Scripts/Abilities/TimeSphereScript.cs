﻿using UnityEngine;
using System.Collections;

public class TimeSphereScript : MonoBehaviour {

	[SerializeField]
	private float activeTime = 7.5f;

	[SerializeField]
	private float slowFactor = 4f;

	void Start()
    {

		StartCoroutine(WaitForDestroy());
		Destroy(gameObject, activeTime);

        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        StartCoroutine(transform.ScaleTo(originalScale, 0.9f, AnimCurveContainer.AnimCurve.pingPong.Evaluate));
	}
	
	void OnTriggerEnter(Collider collider){
		if(collider.GetComponent<MonoBehaviour>() is BaseEnemy && collider.tag == "Enemy"){
	        BaseEnemy enemy = collider.GetComponent<BaseEnemy>();
			enemy.MovementSpeed /= slowFactor;
		}

		if(collider.GetComponent<MonoBehaviour>() is Bullet && collider.tag == "EnemyBullet"){
			Bullet enemyBullet = collider.GetComponent<Bullet>();
			enemyBullet.BulletSpeed /= slowFactor;
		}

		if(collider.GetComponent<MonoBehaviour>() is Rocket && collider.tag == "EnemyBullet"){
			Rocket enemyBullet = collider.GetComponent<Rocket>();
			enemyBullet.BulletSpeed /= slowFactor;
			enemyBullet.Sensitivity /= slowFactor;
		}
	}

	void OnTriggerExit(Collider collider){
		if(collider.tag == "Enemy"){
            BaseEnemy enemy = collider.GetComponent<BaseEnemy>();
			enemy.MovementSpeed *= slowFactor;
		}

		if(collider.GetComponent<MonoBehaviour>() is Bullet && collider.tag == "EnemyBullet"){
			Bullet enemyBullet = collider.GetComponent<Bullet>();
			enemyBullet.BulletSpeed *= slowFactor;
		}

		if(collider.GetComponent<MonoBehaviour>() is Rocket && collider.tag == "EnemyBullet"){
			Rocket enemyBullet = collider.GetComponent<Rocket>();
			enemyBullet.BulletSpeed *= slowFactor;
			enemyBullet.Sensitivity *= slowFactor;
		}
	}

	// Wait for activeTime seconds
	protected IEnumerator WaitForDestroy()
	{
		yield return new WaitForSeconds(activeTime - 0.05f);

		// Gets the collider of the sphere
		SphereCollider myCollider = transform.GetComponent<SphereCollider>();
		
		// Set the collider to nearly 0 
		myCollider.radius = 0.01f;
		// AND set the collider beneath the ground to trigger OnTriggerExit()
		myCollider.center = new Vector3(0f,-25f,0f);

	}
}
