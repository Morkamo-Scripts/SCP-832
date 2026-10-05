using System;
using System.Collections.Generic;
using Exiled.API.Features.Toys;
using SCP_832.Configuration;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCP_832.Runtime;

// Network primitives are recycled, never created by a coroutine per emitter/particle.
internal sealed class ParticleService
{
    private sealed class Emitter
    {
        internal GameObject Target;
        internal Color Color;
        internal int ActiveCount;
        internal float NextSpawn;
    }

    private sealed class Particle
    {
        internal Primitive Toy;
        internal Emitter Owner;
        internal float Started;
        internal float Lifetime;
        internal float Size;
    }

    private readonly List<Emitter> emitters = new();
    private readonly List<Particle> particles = new();
    private int nextEmitter;

    internal void Add(GameObject target, Color color)
    {
        foreach (var emitter in emitters)
            if (emitter.Target == target)
                return;
        emitters.Add(new Emitter { Target = target, Color = color });
    }

    internal void Remove(GameObject target)
    {
        for (int i = emitters.Count - 1; i >= 0; i--)
        {
            var emitter = emitters[i];
            if (emitter.Target != target)
                continue;
            foreach (var particle in particles)
                if (particle.Owner == emitter)
                    Release(particle);
            emitters.RemoveAt(i);
        }
    }

    internal void Tick(float now, ParticleSettings settings)
    {
        if (!settings.Enabled)
        {
            ClearParticles();
            return;
        }

        for (int i = particles.Count - 1; i >= 0; i--)
        {
            var particle = particles[i];
            if (particle.Toy.Base == null)
            {
                Release(particle);
                particles.RemoveAt(i);
                continue;
            }
            if (particle.Owner == null)
                continue;
            float age = now - particle.Started;
            if (particle.Owner.Target == null || age >= particle.Lifetime
                || particle.Owner.ActiveCount > settings.EmitterLimit)
                Release(particle);
            else
                particle.Toy.Scale = Vector3.one * (particle.Size * (float)Math.Sin(Math.PI * age / particle.Lifetime));
        }

        // Round-robin keeps early emitters from monopolizing the shared pool.
        // At most four activations and two new network spawns in one update.
        int activated = 0;
        int created = 0;
        for (int inspected = 0; inspected < emitters.Count && activated < 4; inspected++)
        {
            if (nextEmitter >= emitters.Count)
                nextEmitter = 0;
            var emitter = emitters[nextEmitter++];
            if (emitter.Target == null || emitter.ActiveCount >= settings.EmitterLimit || now < emitter.NextSpawn)
                continue;

            Particle available = null;
            foreach (var particle in particles)
                if (particle.Owner == null)
                {
                    available = particle;
                    break;
                }

            bool spawn = available == null;
            if (spawn)
            {
                if (created >= 2)
                    break;
                available = new Particle { Toy = Primitive.Create(PrimitiveType.Cube, spawn: false) };
                available.Toy.Collidable = false;
                available.Toy.IsStatic = false;
                particles.Add(available);
                created++;
            }

            available.Owner = emitter;
            available.Started = now;
            available.Lifetime = settings.ParticleLifetime;
            available.Size = settings.ParticleSize;
            Vector3 offset = Random.insideUnitSphere * settings.FieldRadius;
            offset.y = Math.Abs(offset.y) + 0.05f;
            available.Toy.Position = emitter.Target.transform.position + offset;
            available.Toy.Rotation = Random.rotation;
            available.Toy.Color = (emitter.Color * settings.Brightness) with { a = 0.6f };
            available.Toy.Scale = Vector3.one * (available.Size * 0.1f);
            available.Toy.Base.syncInterval = settings.TickInterval;
            available.Toy.Visible = true;
            if (spawn)
                available.Toy.Spawn();
            emitter.ActiveCount++;
            emitter.NextSpawn = now + settings.SpawnDelay;
            activated++;
        }
    }

    private static void Release(Particle particle)
    {
        if (particle.Owner != null)
            particle.Owner.ActiveCount--;
        particle.Owner = null;
        if (particle.Toy.Base != null)
            particle.Toy.Visible = false;
    }

    private static void Destroy(Particle particle)
    {
        Release(particle);
        if (particle.Toy.Base != null)
            particle.Toy.Destroy();
    }

    private void ClearParticles()
    {
        foreach (var particle in particles)
            Destroy(particle);
        particles.Clear();
    }

    internal void Clear()
    {
        ClearParticles();
        emitters.Clear();
        nextEmitter = 0;
    }
}
