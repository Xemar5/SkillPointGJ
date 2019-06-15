using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticlesManager : MonoBehaviour
{
    private static BloodParticlesManager instance;

    [SerializeField]
    private ParticleSystem particlesPrefab;
    [SerializeField]
    private float maxDuration;

    private Queue<ParticleSystem> pooled = new Queue<ParticleSystem>();

    private void Awake()
    {
        instance = this;
    }


    public static void Play(Vector2 position, Vector2 direction)
    {
        instance.StartCoroutine(instance.PlayParticle(position, direction));
    }

    private IEnumerator PlayParticle(Vector2 position, Vector2 direction)
    {
        ParticleSystem system;
        if (pooled.Count > 0)
        {
            system = pooled.Dequeue();
            system.gameObject.SetActive(true);
        }
        else
        {
            system = Instantiate(particlesPrefab);
        }

        system.transform.position = position;
        system.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        system.Play();
        yield return new WaitForSeconds(maxDuration);
        system.gameObject.SetActive(false);
        pooled.Enqueue(system);
    }


}
