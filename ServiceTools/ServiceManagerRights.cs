using System;

namespace ServiceTools
{
  [Flags]
  public enum ServiceManagerRights
  {
    Connect = 1,
    CreateService = 2,
    EnumerateService = 4,
    Lock = 8,
    QueryLockStatus = 16, // 0x00000010
    ModifyBootConfig = 32, // 0x00000020
    StandardRightsRequired = 983040, // 0x000F0000
    AllAccess = StandardRightsRequired | ModifyBootConfig | QueryLockStatus | Lock | EnumerateService | CreateService | Connect, // 0x000F003F
  }
}
