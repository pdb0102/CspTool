using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amaic.de.csptool
{
    [Flags]
    enum CryptGetDefaultProviderFlags : uint
    {
        NULL = 0,
        CRYPT_MACHINE_DEFAULT = 0x00000001,
        CRYPT_USER_DEFAULT = 0x00000002,
        CRYPT_DELETE_DEFAULT = 0x00000004
    }

}
