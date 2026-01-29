using Exiled.API.Interfaces;
using SCP_832.Items;

namespace SCP_832
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public string ServerIdenifier { get; set; } = "Classic";

        public Variant832A Scp832 { get; set; } = new();
    }
}