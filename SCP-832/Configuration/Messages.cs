using System;
using System.Collections.Generic;

namespace SCP_832.Configuration;

public sealed class Messages
{
    public Dictionary<string, string> Negative { get; set; } = new()
    {
        ["Handcuff"] = "Ой, тебя связали... \nРазвяжем через {duration} секунд!",
        ["TeleportToRandomScp.Unavailable"] = "Не получилось переместить вас к живому SCP.",
        ["TeleportToRandomScp.Success"] = "Вы были перемещены к случайному SCP.",
        ["ExplodeFlashbang"] = "<size=50>Вспышка!</size>",
        ["ExplodeFragGrenade"] = "<size=50>Вам выпал колобок!</size>\n\n<size=60>Бум?</size>",
        ["RandomDamage.Fatal"] = "Здоровье понижено на {damage}HP.\nНо вашего запаса не хватило...",
        ["RandomDamage.Success"] = "Здоровье понижено на {damage}HP.",
        ["ForceStop"] = "СТОП!\nОтдохни {duration} секунд, сейчас самое время.",
        ["DestroyRandomItemInInventory"] = "У вас что то пропало! Но что?",
        ["TemporaryReversedMovement"] = "Ой, управление перепуталось!\nПодождите {duration} cекунд.",
        ["ForceClassScp0492.PocketWithParent"] = "Ты живой?\nДети должны держаться с родителями!",
        ["ForceClassScp0492.Failed"] = "Хмм...\nПревращение не удалось.",
        ["ForceClassScp0492.Success"] = "Ты живой?",
        ["TemporaryCardiacArrest"] = "Последние {duration}?\nВыжить любой ценой!",
        ["TeleportToPocket"] = "Добро пожаловать в деморган!",
    };

    public Dictionary<string, string> Positive { get; set; } = new()
    {
        ["GiveRandomItem.Legendary"] = "<color=#fc7703>Легендарный</color> предмет.\n{item}? А ты счастливчик...",
        ["GiveRandomItem.Epic"] = "<color=#b603fc>Эпический</color> предмет.\n{item}? Отличный дроп!",
        ["GiveRandomItem.Rare"] = "<color=#06b300>Редкий</color> предмет.\n{item}? Сойдёт!",
        ["GiveRandomItem.Common"] = "<color=#6a7069>Обычный</color> предмет.\n{item}? Лишним ничего не будет!",
        ["RewardUnavailable"] = "Не удалось выдать награду.",
        ["GiveRandomCard"] = "Случайна ключ-карта прямо под тобой!",
        ["GiveRandomScp"] = "Случайный SCP предмет прямо под тобой!",
        ["GiveHealKit"] = "Ты обронил набор первой помощи!\nХотя бы самому себе.",
        ["GiveStaminaKit"] = "Много адреналина не бывает!",
        ["TemporaryInfinityStamina"] = "Почувствуй силу земли!\nВыносливость не тратиться следующие {duration} секунд!",
        ["TemporaryMovementBoost"] = "Всегда знал что ты атлет!\nТройное увеличение скорости на следующие {duration} секунд!",
        ["OverHeal"] = "Жизней много не бывает!\nЗдоровье увеличено на {amount}HP.",
        ["SuperShield"] = "Броня! Жалко что не вечная.",
        ["TemporaryCasperKit"] = "Каково быть призраком?\nВы проходите сквозь двери следующие {duration} секунд!",
        ["TemporaryNightVision"] = "Повышаем яркость на {duration} секунд!\nТак лучше?",
    };

    public Dictionary<string, string> Neutral { get; set; } = new()
    {
        ["None"] = "Вам выпало целое ничего!",
        ["SwapWithRandomPlayer.Tutorial"] = "Не бро, за эту роль нельзя переместиться.",
        ["SwapWithRandomPlayer.Pocket"] = "Тут связь не ловит.\nОставайся на своём месте.",
        ["SwapWithRandomPlayer.Unavailable"] = "Абонент не доступен!\nОставайся на своём месте.",
        ["SwapWithRandomPlayer.Self"] = "Поменялся сам с собой?\nДа ты везунчик...",
        ["SwapWithRandomPlayer.Success"] = "Ты поменялся позицией с\n{player}",
        ["TeleportInRandomRoom.Unavailable"] = "Абонент недоступен...",
        ["TeleportInRandomRoom.LightContainment"] = "Вжух!\nИ вот мы в Зоне Лёгкого Содержания!",
        ["TeleportInRandomRoom.HeavyContainment"] = "Вжух!\nИ вот мы в Зоне Тяжелого Содержания!",
        ["TeleportInRandomRoom.Entrance"] = "Вжух!\nИ вот мы во Входной Зоне",
    };

    public Dictionary<string, string> Other { get; set; } = new()
    {
        ["ZombieArmyJoined"] = "В вашей армии пополнение!\n{player}",
        ["SwapPartner"] = "Ты поменялся позицией с\n{player}",
        ["DamageReason"] = "Орёл или решка?",
        ["HintFormat"] = "<b><size=60%>{text}</size></b>",
    };

    public string Get(string group, string key)
    {
        var messages = group switch
        {
            "Negative" => Negative,
            "Positive" => Positive,
            "Neutral" => Neutral,
            _ => Other,
        };
        return messages != null && messages.TryGetValue(key, out var text) ? text ?? string.Empty : string.Empty;
    }

    public void FillMissingDefaults()
    {
        var defaults = new Messages();
        Negative = FillMissing(Negative, defaults.Negative);
        Positive = FillMissing(Positive, defaults.Positive);
        Neutral = FillMissing(Neutral, defaults.Neutral);
        Other = FillMissing(Other, defaults.Other);
    }

    public string Format(string group, string key, params (string Token, object Value)[] values)
    {
        string text = Get(group, key);
        foreach (var value in values)
            text = text.Replace("{" + value.Token + "}", Convert.ToString(value.Value));
        return text;
    }

    private static Dictionary<string, string> FillMissing(Dictionary<string, string> configured, Dictionary<string, string> defaults)
    {
        if (configured == null)
            return defaults;
        foreach (var entry in defaults)
            if (!configured.ContainsKey(entry.Key))
                configured.Add(entry.Key, entry.Value);
        return configured;
    }
}
