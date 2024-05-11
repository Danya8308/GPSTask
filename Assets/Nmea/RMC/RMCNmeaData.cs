using System;

public struct RMCNmeaData
{
    public DateTime SendDate;
    public float Latitude;
    public float Longitude;

    public override string ToString()
    {
        return string.Join(" | ", SendDate, Latitude, Longitude);
    }
}