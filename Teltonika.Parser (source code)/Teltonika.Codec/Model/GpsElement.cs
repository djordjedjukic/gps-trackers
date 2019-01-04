namespace Teltonika.Codec.Model
{
    public struct GpsElement
    {
        public const short InvalidGpsSpeed = 255;

        public float X { get; private set; }
        public float Y { get; private set; }
        public short Altitude { get; private set; }
        public short Angle { get; private set; }
        public byte Satellites { get; private set; }
        public short Speed { get; private set; }

        public static readonly GpsElement Default = new GpsElement();

        public static GpsElement Create(float x, float y, short altitude, short speed, short angle, byte satellites)
        {
            return new GpsElement
            {
                X = x,
                Y = y,
                Altitude = altitude,
                Angle = angle,
                Satellites = satellites,
                Speed = speed
            };
        }

        public static bool IsLatValid(double latitude)
        {
            return -90 <= latitude && latitude <= 90;
        }

        public static bool IsLngValid(double longitude)
        {
            return -180 <= longitude && longitude <= 180;
        }


    }
}
