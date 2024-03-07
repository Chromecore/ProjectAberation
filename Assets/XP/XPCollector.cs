using UnityEngine;
using System.Collections.Generic;

namespace Chromecore
{
    public class XPCollector : MonoBehaviour 
    {
        private ParticleSystem ps;
        private List<ParticleSystem.Particle> particles;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            particles = new();
        }

        private void OnParticleTrigger()
        {
            if (GameManager.gamePaused) return;
            int triggerParticles = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

            for (int i = 0; i < triggerParticles; i++)
            {
                ParticleSystem.Particle p = particles[i];
                p.remainingLifetime = 0;
                GameManager.score += 10;
                particles[i] = p;
            }

            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
        }
    }
}