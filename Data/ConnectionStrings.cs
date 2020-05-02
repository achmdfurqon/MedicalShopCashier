using System;

namespace Data
{
    public sealed class ConnectionStrings
    {
        public string Value { get; set; }
        public ConnectionStrings(string value)
        {
            Value = value;
        }
    }
}
