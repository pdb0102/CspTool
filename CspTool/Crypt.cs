using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace amaic.de.csptool
{
    public static class Crypt
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptEnumProviderTypes(int dwIndex, IntPtr pdwReserved, int dwFlags, ref int pdwProvType, StringBuilder pszTypeName, ref int pcbTypeName);

        public static IDictionary<int, string> EnumerateProviderTypes()
        {
            var providerTypes = new Dictionary<int, string>();

            var index = 0;
            var providerTypeIdentifier = 0;
            var providerTypeNameLength_Bytes = 0;
            while (CryptEnumProviderTypes(index, IntPtr.Zero, 0, ref providerTypeIdentifier, null, ref providerTypeNameLength_Bytes))
            {
                var providerTypeName = new StringBuilder(providerTypeNameLength_Bytes);
                if (CryptEnumProviderTypes(index++, IntPtr.Zero, 0, ref providerTypeIdentifier, providerTypeName, ref providerTypeNameLength_Bytes) == false)
                    throw new Win32Exception();

                providerTypes.Add(providerTypeIdentifier, providerTypeName.ToString());
            }

            return providerTypes;
        }
    }
}
