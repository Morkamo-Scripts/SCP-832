using System;
using System.IO;
using Exiled.API.Features;
using Exiled.Loader;

namespace SCP_832.Configuration;

internal static class MessageFile
{
    internal static Messages Load()
    {
        string directory = Path.Combine(Paths.Configs, "SCP-832");
        string path = Path.Combine(directory, $"messages-{Server.Port}.yml");
        try
        {
            if (File.Exists(path))
            {
                var messages = Loader.Deserializer.Deserialize<Messages>(File.ReadAllText(path)) ?? new Messages();
                messages.FillMissingDefaults();
                return messages;
            }

            Directory.CreateDirectory(directory);
            File.WriteAllText(path, Loader.Serializer.Serialize(new Messages()));
        }
        catch (Exception exception)
        {
            Log.Error($"SCP-832: could not load messages from {path}. Using defaults. {exception}");
        }

        return new Messages();
    }
}
