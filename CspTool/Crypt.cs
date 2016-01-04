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

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptEnumProviders(int dwIndex, IntPtr pdwReserved, int dwFlags, ref int pdwProvType, StringBuilder pszProvName, ref int pcbProvName);


        public static IDictionary<int, ProviderType> EnumerateProviderTypes()
        {
            var providerTypes = new Dictionary<int, ProviderType>();

            var index = 0;
            var providerTypeId = 0;
            var providerTypeNameLength_Bytes = 0;
            while (CryptEnumProviderTypes(index, IntPtr.Zero, 0, ref providerTypeId, null, ref providerTypeNameLength_Bytes))
            {
                var providerTypeName = new StringBuilder(providerTypeNameLength_Bytes);
                if (CryptEnumProviderTypes(index++, IntPtr.Zero, 0, ref providerTypeId, providerTypeName, ref providerTypeNameLength_Bytes) == false)
                    throw new Win32Exception();

                providerTypes.Add(providerTypeId, new ProviderType(providerTypeId, providerTypeName.ToString()));
            }

            return providerTypes;
        }

        public static IEnumerable<Provider> EnumerateProviders()
        {
            var providerTypes = EnumerateProviderTypes();

            var providers = new List<Provider>();

            var index = 0;
            var providerTypeId = 0;
            var providerNameLength_Bytes = 0;
            while (CryptEnumProviders(index, IntPtr.Zero, 0, ref providerTypeId, null, ref providerNameLength_Bytes))
            {
                var providerName = new StringBuilder(providerNameLength_Bytes);
                if (CryptEnumProviders(index++, IntPtr.Zero, 0, ref providerTypeId, providerName, ref providerNameLength_Bytes) == false)
                    throw new Win32Exception();

                providers.Add(new Provider(providerName.ToString(), providerTypes[providerTypeId]));
            }

            return providers;
        }
    }
}
