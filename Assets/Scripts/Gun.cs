
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Gun : MonoBehaviour, IInteractable
{
    [Header("Gun Stats")]
    public int damage;
    public float timeBetweenShots, spread, range;
    public int bulletsPerTap;
    public bool allowButtonHold;
    public GunType gunType;
    private GameObject bulletPrefab;

    [Header("Bools")]
    public bool shooting;
    public bool readyToShoot;
    public bool reloading;
    public bool hasInfiniteAmmo;

    [Header("References")]
    public Transform attackPoint;
    public RaycastHit rayHit;
    private Rigidbody rb;
    public GameObject selfPrefab;
    public string prefabName;
    private AudioSource gunsound;

    [Header("Graphics")]
    public GameObject muzzleFlash, bulletHoleGraphic;
    public CamShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI AmmunitionText;
    public ParticleSystem hitEffect, hitEffectEnemy;
    public TrailRenderer trailEffect;


    private void Awake()
    {
        readyToShoot = true;
        rb = GetComponent<Rigidbody>();
        selfPrefab = Resources.Load(prefabName) as GameObject;
        gunsound = GetComponent<AudioSource>();
        bulletPrefab = Resources.Load("Prefabs/Effects/Bullet") as GameObject;
    }

    public void BecomeEquipped()
    {
        rb.isKinematic = true;
    }

    // Returns whether it shot or not
    public bool Shoot(Vector3 directionToShoot, bool isRaycastShoot, float spreadModifier)
    {
        if (readyToShoot) {
            readyToShoot = false;
            Invoke("ResetShot", timeBetweenShots);

            //Spread
            float finalSpread = Mathf.Max(0, spread + spreadModifier);

            //Calculate Direction with Spread
            Vector3 finalDirection = directionToShoot + new Vector3(Random.Range(-finalSpread, finalSpread), Random.Range(-finalSpread, finalSpread), Random.Range(-finalSpread, finalSpread));

            gunsound.Play();

            if (isRaycastShoot)
            {
                RaycastShoot(finalDirection);
            }
            else
            {
                ProjectileShoot(finalDirection);
            }

            Instantiate(muzzleFlash, attackPoint);

            return true;
        } else {
            return false;
		}
    }

    private void RaycastShoot(Vector3 directionToShoot) {
        if (Physics.Raycast(attackPoint.position, directionToShoot, out rayHit, range))
        {
            Character characterHit = rayHit.transform.GetComponent<Character>();
            if (characterHit != null)
            {
                hitEffectEnemy.transform.position = rayHit.point;
                hitEffectEnemy.transform.forward = rayHit.normal;
                hitEffectEnemy.Emit(5);

                characterHit.TakeDamage(damage);
            }
            else
            {
                hitEffect.transform.position = rayHit.point;
                hitEffect.transform.forward = rayHit.normal;
                hitEffect.Emit(5);
                Debug.Log("bullet impact");
            }
        }

        var tracer = Instantiate(trailEffect, attackPoint.position, Quaternion.identity);
        tracer.AddPosition(attackPoint.position);

        GameObject newHole = Instantiate(bulletHoleGraphic, rayHit.point + rayHit.normal * 0.00f, Quaternion.identity);
        newHole.transform.LookAt(rayHit.point + rayHit.normal);
        newHole.transform.position += newHole.transform.forward / 1000;
        Destroy(newHole, 15f);

        tracer.transform.position = rayHit.point;
    }

    private void ProjectileShoot(Vector3 directionToShoot)
    {
        GameObject newBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
        Bullet bullet = newBullet.GetComponent<Bullet>();
        bullet.Setup(directionToShoot, damage, new List<GameObject>() { gameObject });
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

	public void OnInteract(InteractInfo interactInfo)
	{
        interactInfo.byPlayer.EquipGun(this, interactInfo.side);
        Destroy(gameObject);
    }
}
