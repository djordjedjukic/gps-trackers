using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Teltonika.DataParser.Client
{
    public enum DataType
    {
        [Display(Name="Codec ID")]
        CodecId,
        [Display(Name = "AVL Data Count")]
        AvlDataCount,
        Timestamp,
        Priority,
        Latitude,
        Longitude,
        Altitude,
        Angle,
        Satellites,
        Speed,
        [Display(Name = "Event ID")]
        EventIoId,
        [Display(Name = "Element count")]
        IoCount,
        [Display(Name = "1b Element count")]
        IoCount1B,
        [Display(Name = "ID")]
        IoId1B,
        [Display(Name = "Value")]
        IoValue1B,
        [Display(Name = "2b Element count")]
        IoCount2B,
        [Display(Name = "ID")]
        IoId2B,
        [Display(Name = "Value")]
        IoValue2B,
        [Display(Name = "4b Element count")]
        IoCount4B,
        [Display(Name = "ID")]
        IoId4B,
        [Display(Name = "Value")]
        IoValue4B,
        [Display(Name = "8b Element Count")]
        IoCount8B,
        [Display(Name = "ID")]
        IoId8B,
        [Display(Name = "Value")]
        IoValue8B,
        // Tcp types
        Preamble,
        [Display(Name = "AVL Data Length")]
        AvlDataArrayLength,
        Crc,
        // Codec7 types
        [Display(Name = "Priority")]
        PriorityGh,
        [Display(Name = "Timestamp")]
        TimestampGh,
        [Display(Name = "Global Mask")]
        GlobalMask,
        [Display(Name = "Gps Element Mask")]
        GpsElementMask,
        [Display(Name = "Longitude")]
        LongitudeGh,
        [Display(Name = "Latitude")]
        LatitudeGh,
        [Display(Name = "Angle")]
        AngleGh,
        [Display(Name = "Speed")]
        SpeedGh,
        // GpsIO elements for Codec7
        [Display(Name = "Local Area Code and Cell ID")]
        CellIdAndLocalAreaGh,
        [Display(Name = "Signal Quality")]
        SignalQualityGh,
        [Display(Name = "Operator Code")]
        OperatorCodeGh,
        // Udp types
        Length,
        [Display(Name = "Packet ID")]
        PacketId,
        [Display(Name = "Packet Type")]
        PacketType,
        [Display(Name = "AVL packet ID")]
        AvlPacketId,
        [Display(Name = "Imei length")]
        ImeiLength,
        Imei
    }

}