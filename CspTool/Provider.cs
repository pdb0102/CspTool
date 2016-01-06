using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace amaic.de.csptool
{
    public class Provider
    {
        public Provider(string name, ProviderType providerType)
        {
            Name = name;
            ProviderType = providerType;
        }

        public string Name { get; private set; }
        public ProviderType ProviderType { get; private set; }

        public AsymmetricAlgorithm GetCryptoServiceProvider()
        {
            var providerType = ProviderType;
            var providerTypeId = providerType.Id;

            var cspParameter = new CspParameters((int)providerTypeId, Name);

            switch (providerTypeId)
            {
                case ProviderType.Ids.PROV_RSA_FULL:
                case ProviderType.Ids.PROV_RSA_AES:
                    return new RSACryptoServiceProvider(cspParameter);

                case ProviderType.Ids.PROV_RSA_SCHANNEL:


                case ProviderType.Ids.NULL:

                case ProviderType.Ids.PROV_RSA_SIG:

                case ProviderType.Ids.PROV_DSS:

                case ProviderType.Ids.PROV_FORTEZZA:

                case ProviderType.Ids.PROV_MS_EXCHANGE:

                case ProviderType.Ids.PROV_SSL:

                case ProviderType.Ids.PROV_DSS_DH:

                case ProviderType.Ids.PROV_EC_ECDSA_SIG:

                case ProviderType.Ids.PROV_EC_ECNRA_SIG:

                case ProviderType.Ids.PROV_EC_ECDSA_FULL:

                case ProviderType.Ids.PROV_EC_ECNRA_FULL:

                case ProviderType.Ids.PROV_DH_SCHANNEL:

                case ProviderType.Ids.PROV_SPYRUS_LYNKS:

                case ProviderType.Ids.PROV_RNG:

                case ProviderType.Ids.PROV_INTEL_SEC:

                case ProviderType.Ids.PROV_REPLACE_OWF:


                default:
                    throw new NotImplementedException($"Provider type '{providerType}' not supported.");
            }
        }

        public IEnumerable<Container> EnumerateContainers(Scope scope)
        {
            return Container.EnumerateContainers(this, scope);
        }

        public override string ToString()
        {
            return Name;
        }


        public static IEnumerable<Provider> EnumerateProviders()
        {
            var providerTypes = ProviderType.GetProviderTypes();

            var index = 0;
            var providerTypeId = ProviderType.Ids.NULL;
            var providerNameLength_Bytes = 0;
            while (CryptEnumProviders(index, IntPtr.Zero, 0, ref providerTypeId, null, ref providerNameLength_Bytes))
            {
                var providerName = new StringBuilder(providerNameLength_Bytes);
                if (CryptEnumProviders(index++, IntPtr.Zero, 0, ref providerTypeId, providerName, ref providerNameLength_Bytes) == false)
                    throw new Win32Exception();

                yield return new Provider(providerName.ToString(), providerTypes[providerTypeId]);
            }
        }

        public static Provider GetDefaultProvider(ProviderType.Ids providerTypeId, Scope scope)
        {
            var gdpFlags = scope == Scope.Machine ? CryptGetDefaultProviderFlags.CRYPT_MACHINE_DEFAULT : CryptGetDefaultProviderFlags.CRYPT_USER_DEFAULT;
            var defaultProviderNameLength_Bytes = 0;
            if (CryptGetDefaultProvider(providerTypeId, IntPtr.Zero, gdpFlags, null, ref defaultProviderNameLength_Bytes) == false)
                throw new Win32Exception();

            var defaultProviderName = new StringBuilder(defaultProviderNameLength_Bytes);
            if (CryptGetDefaultProvider(providerTypeId, IntPtr.Zero, gdpFlags, defaultProviderName, ref defaultProviderNameLength_Bytes) == false)
                throw new Win32Exception();

            var providerTypes = ProviderType.GetProviderTypes();

            return new Provider(defaultProviderName.ToString(), providerTypes[providerTypeId]);
        }


        [Flags]
        enum CryptGetDefaultProviderFlags : uint
        {
            NULL = 0,
            CRYPT_MACHINE_DEFAULT = 0x00000001,
            CRYPT_USER_DEFAULT = 0x00000002,
            CRYPT_DELETE_DEFAULT = 0x00000004
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptEnumProviders(int dwIndex, IntPtr pdwReserved, int dwFlags, ref ProviderType.Ids pdwProvType, StringBuilder pszProvName, ref int pcbProvName);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptGetDefaultProvider(ProviderType.Ids dwProvType, IntPtr pdwReserved, CryptGetDefaultProviderFlags dwFlags, StringBuilder pszProvName, ref int ProvName);
    }
}
