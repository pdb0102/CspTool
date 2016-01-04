using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace amaic.de.csptool
{
    public class ProviderHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public ProviderHandle() : base(true) { }

        protected override bool ReleaseHandle()
        {
            return CryptReleaseContext(handle, 0);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptReleaseContext(IntPtr hProv, Int32 dwFlags);

    }
}
