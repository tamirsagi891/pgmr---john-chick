using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingItemSpawner : MonoBehaviour
{
    [SerializeField] private float t; // interval between each spawn
    [SerializeField] private GameObject projectilePrefab; // the prefab to spawn
    [SerializeField] private int poolSize = 10; // size of the object pool
    [SerializeField] private float playerDistance = 30; // the distance within which the player must be for the object to spawn

    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    private Transform playerTransform;
    private List<GameObject> objectPool;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize the object pool
        objectPool = new List<GameObject>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, transform);
            projectileInstance.SetActive(false);
            objectPool.Add(projectileInstance);
        }

        StartCoroutine(SpawnProjectile());
    }

    IEnumerator Shake()
    {
        float elapsed = 0.0f;

        Vector2 originalPosition = transform.position;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector2(originalPosition.x + x, originalPosition.y + y);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;
    }

    IEnumerator SpawnProjectile()
    {
        while (true)
        {
            yield return new WaitForSeconds(t);

            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (distance <= playerDistance)
            {
                StartCoroutine(Shake());

                // Get an inactive projectile from the pool and activate it
                foreach (GameObject projectile in objectPool)
                {
                    if (!projectile.activeInHierarchy)
                    {
                        projectile.transform.position = transform.position;
                        projectile.GetComponent<FallingItem>().ResetProjectile();
                        projectile.SetActive(true);
                        break;
                    }
                }
            }
        }
    }
}
