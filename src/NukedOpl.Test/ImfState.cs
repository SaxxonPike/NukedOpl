namespace NukedOpl.Test;

public class ImfState
{
    public Opl3 Opl3 { get; init; }
    public Opl3Chip Chip { get; init; }
    public int TicksPerSecond { get; init; }
    public int Timer { get; set; }
    public int Delay { get; set; }
    public bool Stopped { get; set; }
    public int Tick { get; set; }
}