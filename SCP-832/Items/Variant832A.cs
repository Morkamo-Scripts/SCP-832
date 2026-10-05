using System;
using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using SCP_832.Components.Enums;
using Random = UnityEngine.Random;
using Player = Exiled.API.Features.Player;

namespace SCP_832.Items;

public partial class Variant832A : CustomItem
{
    public override uint Id { get; set; } = 11;
    public override string Name { get; set; } = "SCP-832";
    public override string Description { get; set; } = string.Empty;
    public override float Weight { get; set; } = 1;
    public override ItemType Type { get; set; } = ItemType.Coin;
    public override SpawnProperties SpawnProperties { get; set; }

    private static readonly NegativeEvents[] NegativeValues = (NegativeEvents[])Enum.GetValues(typeof(NegativeEvents));
    private static readonly PositiveEvents[] PositiveValues = (PositiveEvents[])Enum.GetValues(typeof(PositiveEvents));
    private static readonly NeutralEvents[] NeutralValues = (NeutralEvents[])Enum.GetValues(typeof(NeutralEvents));

    private string CallRandomEvent(Player player)
    {
        int category = Random.Range(0, 3);
        if (category == 0 && Random.Range(0, 3) == 2)
            return ApplyPositiveEvent(player, PositiveEvents.GiveRandomItem);

        return category switch
        {
            0 => ApplyNegativeEvent(player, NegativeValues[Random.Range(0, NegativeValues.Length)]),
            1 => ApplyPositiveEvent(player, PositiveValues[Random.Range(0, PositiveValues.Length)]),
            _ => ApplyNeutralEvent(player, NeutralValues[Random.Range(0, NeutralValues.Length)]),
        };
    }

    private static string Message(string group, string key, params (string Token, object Value)[] values)
        => Plugin.Instance.Messages.Format(group, key, values);

    private static T PickRandom<T>(IEnumerable<T> source, Func<T, bool> predicate = null) where T : class
    {
        T selected = null;
        int count = 0;
        foreach (T candidate in source)
            if ((predicate == null || predicate(candidate)) && Random.Range(0, ++count) == 0)
                selected = candidate;
        return selected;
    }
}
