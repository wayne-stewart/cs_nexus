using System;

namespace Logging
{
    [Flags]
    public enum LogTypes
    {
        Debug   = 0x01,
        Error   = 0x01 << 1,
        Info    = 0x01 << 2,
        Success = 0x01 << 3
    }
}
