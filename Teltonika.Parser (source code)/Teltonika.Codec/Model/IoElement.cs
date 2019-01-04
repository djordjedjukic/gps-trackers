using System.Collections;
using System.Collections.Generic;

namespace Teltonika.Codec.Model
{
    public struct IoElement
    {
        public int EventId { get; private set; }
        public int PropertiesCount { get; private set; }
        public IEnumerable<IoProperty> Properties { get; private set; }
        
        public static IoElement Create(int eventId, int propertyCount, IEnumerable<IoProperty> properties)
        {
            return new IoElement
            {
                EventId = eventId,
                PropertiesCount = propertyCount,
                Properties = properties
            };
        }
    }
}
