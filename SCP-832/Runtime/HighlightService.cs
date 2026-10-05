using System.Collections.Generic;
using MEC;
using SCP_832.Configuration;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace SCP_832.Runtime;

// Lights and all particle emitters share one bounded update loop.
internal sealed class HighlightService
{
    private readonly Dictionary<GameObject, Light> lights = new();
    private readonly LinkedList<GameObject> creationOrder = new();
    private readonly Dictionary<GameObject, LinkedListNode<GameObject>> orderNodes = new();
    private readonly List<GameObject> destroyedTargets = new();
    private readonly ParticleService particles = new();
    private readonly ParticleSettings defaultParticles = new();
    private CoroutineHandle updater;

    internal void Add(GameObject target, Color color)
    {
        var config = Plugin.Instance.Config;
        if (!config.EnableHighlights || config.MaxHighlights <= 0 || target == null || lights.ContainsKey(target))
            return;
        // Evict in creation order; repeated requests for an existing target do not refresh its age.
        while (lights.Count >= config.MaxHighlights)
            Remove(creationOrder.First.Value);
        Light light = Light.Create(position: target.transform.position, spawn: false);
        light.Color = color;
        light.ShadowType = LightShadows.None;
        light.Intensity = 0.8f;
        light.Range = 1f;
        light.Spawn();
        lights.Add(target, light);
        orderNodes.Add(target, creationOrder.AddLast(target));
        particles.Add(target, color);
        if (!updater.IsRunning)
            updater = Timing.RunCoroutine(UpdatePositions());
    }

    internal void Remove(GameObject target)
    {
        if (!lights.TryGetValue(target, out var light))
            return;
        lights.Remove(target);
        creationOrder.Remove(orderNodes[target]);
        orderNodes.Remove(target);
        particles.Remove(target);
        if (light.Base != null)
            light.Destroy();
    }

    private IEnumerator<float> UpdatePositions()
    {
        float nextLightUpdate = 0f;
        while (lights.Count > 0)
        {
            var config = Plugin.Instance.Config;
            var settings = config.Particles ?? defaultParticles;
            float interval = settings.Enabled ? settings.TickInterval : 0.5f;
            if (!config.EnableHighlights)
            {
                Clear();
                yield break;
            }
            float now = Time.time;
            particles.Tick(now, settings);
            if (now < nextLightUpdate)
            {
                yield return Timing.WaitForSeconds(interval);
                continue;
            }
            nextLightUpdate = now + 0.5f;
            destroyedTargets.Clear();
            foreach (var entry in lights)
            {
                if (entry.Key == null || entry.Value.Base == null)
                    destroyedTargets.Add(entry.Key);
                else if ((entry.Value.Position - entry.Key.transform.position).sqrMagnitude > 0.0025f)
                    entry.Value.Position = entry.Key.transform.position;
            }
            foreach (var target in destroyedTargets)
                Remove(target);
            destroyedTargets.Clear();
            yield return Timing.WaitForSeconds(interval);
        }
        particles.Clear();
    }

    internal void Clear()
    {
        Timing.KillCoroutines(updater);
        particles.Clear();
        foreach (var light in lights.Values)
            if (light.Base != null)
                light.Destroy();
        lights.Clear();
        creationOrder.Clear();
        orderNodes.Clear();
        destroyedTargets.Clear();
        updater = default;
    }
}
