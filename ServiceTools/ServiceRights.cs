using System;

namespace ServiceTools
{
  [Flags]
  public enum ServiceRights
  {
    QueryConfig = 1,
    ChangeConfig = 2,
    QueryStatus = 4,
    EnumerateDependants = 8,
    Start = 16, // 0x00000010
    Stop = 32, // 0x00000020
    PauseContinue = 64, // 0x00000040
    Interrogate = 128, // 0x00000080
    UserDefinedControl = 256, // 0x00000100
    Delete = 65536, // 0x00010000
    StandardRightsRequired = 983040, // 0x000F0000
    AllAccess = StandardRightsRequired | UserDefinedControl | Interrogate | PauseContinue | Stop | Start | EnumerateDependants | QueryStatus | ChangeConfig | QueryConfig, // 0x000F01FF
  }
}
