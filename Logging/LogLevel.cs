using System;

namespace Logging
{
    [Flags]
    public enum LogLevels
    {
        Debug   = 0xFF,
        Info    = LogTypes.Error | LogTypes.Info | LogTypes.Success,
        Error   = LogTypes.Error | LogTypes.Success
    }
}
