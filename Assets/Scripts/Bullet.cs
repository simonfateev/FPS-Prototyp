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
            Character hitCharacter = other.collider.GetComponent<Character>();
            if (hitCharacter != null)
            {
                hitCharacter.TakeDamage(bulletDamage);
            }
            Destroy(gameObject);
        }
	}
}
