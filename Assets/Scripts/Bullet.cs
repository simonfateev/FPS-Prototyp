using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed = 5f;
    private Rigidbody rb;
    private float bulletDamage;
    private bool alreadyHit;
    private List<GameObject> ignoreCollisionsWith;

    public GameObject bulletModel;
    public ParticleSystem hitEffect, hitEffectEnemy;

    void Awake()
    {
        Destroy(gameObject, 10f); // if they somehow survive for 10 seconds kill them

		rb = GetComponent<Rigidbody>();
        alreadyHit = false;
    }

    // Should be called when bullet is instatiated
    public void Setup(Vector3 direction, float bulletDamage, List<GameObject> ignoreCollisionsWith) {
        this.bulletDamage = bulletDamage;
        this.ignoreCollisionsWith = ignoreCollisionsWith;

        rb.AddForce(direction.normalized * bulletSpeed * 0.1f, ForceMode.Impulse);
        transform.rotation = Quaternion.LookRotation(direction);
	}

	public void OnCollisionEnter(Collision other)
	{
        if (!alreadyHit && !ignoreCollisionsWith.Contains(other.gameObject)) {
            alreadyHit = true;
            bulletModel.SetActive(false);
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;

            Character hitCharacter = other.collider.GetComponent<Character>();
            ContactPoint contactPoint = other.GetContact(0);
            if (hitCharacter != null)
            {
                hitEffectEnemy.transform.position = contactPoint.point;
                hitEffectEnemy.transform.forward = contactPoint.normal;
                hitEffectEnemy.Emit(5);
                hitCharacter.TakeDamage(bulletDamage);
                SoundManager.PlaySound(SoundManager.Sound.enemyhit, transform.position);
                Debug.Log("hit enemy");
            } else
            {
                hitEffect.transform.position = contactPoint.point;
                hitEffect.transform.forward = contactPoint.normal;
                hitEffect.Emit(10);
                SoundManager.PlaySound(SoundManager.Sound.bulletimpact, transform.position);
            }
            Destroy(gameObject, 2f);
        }
	}
}
