using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AccretionDisk : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;
    public float orbitSpeed = 5f;
    public int numberOfParticles = 1000;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        InitializeParticleSystem();
    }

    void InitializeParticleSystem()
    {
        var main = particleSystem.main;
        main.maxParticles = numberOfParticles;
        main.startSpeed = 0;
        main.startLifetime = Mathf.Infinity;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = particleSystem.emission;
        emission.rateOverTime = 0; // Stop automatic emission
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, numberOfParticles) });

        particleSystem.Play();
        CreateOrbitingParticles();
    }

    void CreateOrbitingParticles()
    {
        particles = new ParticleSystem.Particle[numberOfParticles];
        particleSystem.GetParticles(particles);

        for (int i = 0; i < particles.Length; i++)
        {
            float angle = i * (360f / particles.Length);
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            particles[i].position = transform.position + direction * (particleSystem.shape.radius + Random.Range(-0.5f, 0.5f));
            particles[i].startSize = 0.1f;
            particles[i].startColor = Color.white;
        }

        particleSystem.SetParticles(particles, particles.Length);
    }

    void Update()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            Vector3 direction = (Vector3)particles[i].position - transform.position;
            particles[i].position += Quaternion.Euler(0, 0, 90) * direction.normalized * (orbitSpeed * Time.deltaTime) / direction.magnitude;
        }

        particleSystem.SetParticles(particles, particles.Length);
    }
}