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
        public static IDictionary<int, ProviderType> GetProviderTypes()
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
            var providerTypes = GetProviderTypes();
            
            var index = 0;
            var providerTypeId = 0;
            var providerNameLength_Bytes = 0;
            while (CryptEnumProviders(index, IntPtr.Zero, 0, ref providerTypeId, null, ref providerNameLength_Bytes))
            {
                var providerName = new StringBuilder(providerNameLength_Bytes);
                if (CryptEnumProviders(index++, IntPtr.Zero, 0, ref providerTypeId, providerName, ref providerNameLength_Bytes) == false)
                    throw new Win32Exception();

                yield return new Provider(providerName.ToString(), providerTypes[providerTypeId]);
            }
        }

        public static Provider GetDefaultProvider(int providerTypeId, bool machine)
        {
            var flags = machine ? CryptGetDefaultProviderFlags.CRYPT_MACHINE_DEFAULT : CryptGetDefaultProviderFlags.CRYPT_USER_DEFAULT;
            var defaultProviderNameLength_Bytes = 0;
            if (CryptGetDefaultProvider(providerTypeId, IntPtr.Zero, flags, null, ref defaultProviderNameLength_Bytes) == false)
                throw new Win32Exception();

            var defaultProviderName = new StringBuilder(defaultProviderNameLength_Bytes);
            if (CryptGetDefaultProvider(providerTypeId, IntPtr.Zero, flags, defaultProviderName, ref defaultProviderNameLength_Bytes) == false)
                throw new Win32Exception();

            var providerTypes = GetProviderTypes();

            return new Provider(defaultProviderName.ToString(), providerTypes[providerTypeId]);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptEnumProviderTypes(int dwIndex, IntPtr pdwReserved, int dwFlags, ref int pdwProvType, StringBuilder pszTypeName, ref int pcbTypeName);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptEnumProviders(int dwIndex, IntPtr pdwReserved, int dwFlags, ref int pdwProvType, StringBuilder pszProvName, ref int pcbProvName);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptGetDefaultProvider(int dwProvType, IntPtr pdwReserved, CryptGetDefaultProviderFlags dwFlags, StringBuilder pszProvName, ref int ProvName);


        [DllImport("advapi32.dll", CharSet=CharSet.Auto)]
        static extern bool CryptAcquireContext(ProviderHandle hProv, string pszContainer,  string pszProvider, int dwProvType, CryptAcquireContextFlags dwFlags);
    }
}
