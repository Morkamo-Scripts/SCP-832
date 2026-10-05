using System;
using System.ComponentModel;

namespace SCP_832.Configuration;

public sealed class ParticleSettings
{
    public bool Enabled { get; set; } = true;

    [Description("Не более этого числа активных частиц на один предмет или зомби. Допустимо 1–8.")]
    public int MaxPerEmitter { get; set; } = 8;

    [Description("Интервал появления частиц на каждом объекте, в секундах. Минимум 0.2.")]
    public float SpawnInterval { get; set; } = 0.3f;

    public float Lifetime { get; set; } = 1.2f;

    [Description("Интервал общего обновления анимации.")]
    public float UpdateInterval { get; set; } = 0.01f;

    public float Size { get; set; } = 0.08f;
    public float Radius { get; set; } = 0.35f;
    public float Intensity { get; set; } = 5f;

    internal int EmitterLimit => Math.Max(1, Math.Min(MaxPerEmitter, 8));
    internal float SpawnDelay => Clamp(SpawnInterval, 0.3f, 0.1f, 10f);
    internal float ParticleLifetime => Clamp(Lifetime, 1.2f, 0.1f, 5f);
    internal float TickInterval => Clamp(UpdateInterval, 0.1f, 0.01f, 1f);
    internal float ParticleSize => Clamp(Size, 0.08f, 0.01f, 0.3f);
    internal float FieldRadius => Clamp(Radius, 0.35f, 0.05f, 2f);
    internal float Brightness => Clamp(Intensity, 5f, 0.1f, 20f);

    private static float Clamp(float value, float fallback, float minimum, float maximum)
        => float.IsNaN(value) || float.IsInfinity(value) ? fallback : Math.Max(minimum, Math.Min(value, maximum));
}
