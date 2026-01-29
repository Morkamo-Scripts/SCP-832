namespace SCP_832.Components.Events.Components;

public interface ICancelableEvent
{
    public bool IsAllowed { get; set; }
}