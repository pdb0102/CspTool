using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amaic.de.csptool
{
    [Flags]
    enum CryptAcquireContextFlags : uint
    {
        NULL = 0,
        CRYPT_VERIFYCONTEXT = 0xF0000000,
        CRYPT_NEWKEYSET = 0x00000008,
        CRYPT_DELETEKEYSET = 0x00000010,
        CRYPT_MACHINE_KEYSET = 0x00000020,
        CRYPT_SILENT = 0x00000040,
        CRYPT_DEFAULT_CONTAINER_OPTIONAL = 0x00000080
    }

}
