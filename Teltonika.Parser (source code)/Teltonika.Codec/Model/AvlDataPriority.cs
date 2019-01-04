namespace Teltonika.Codec.Model
{
    public enum AvlDataPriority : byte
    {
        Low = 0,
        High = 1,
        Panic = 2,
        Security = 3
    }

    public enum GhAvlDataPriority
    {
        Periodical = 1,
        Alarm = 10
    }
}
