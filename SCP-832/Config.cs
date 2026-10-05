using System.ComponentModel;
using Exiled.API.Interfaces;
using SCP_832.Configuration;
using SCP_832.Items;

namespace SCP_832;

public sealed class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; }

    [Description("Настройки SCP-832, таблицы наград и комнаты для телепортации.")]
    public Variant832A Scp832 { get; set; } = new();

    [Description("Подсветка и процедурные частицы вокруг предметов и монетных зомби.")]
    public bool EnableHighlights { get; set; } = true;

    [Description("Предел источников подсветки. При заполнении новый заменяет самый старый. 0 отключает подсветку.")]
    public int MaxHighlights { get; set; } = 64;

    [Description("Ограниченные процедурные частицы. Можно отключить отдельно от света.")]
    public ParticleSettings Particles { get; set; } = new();

    public float HintDuration { get; set; } = 4f;
    public float HintPosition { get; set; } = 300f;
}
