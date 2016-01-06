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
    public class ProviderType
    {
        public ProviderType(Ids id, string name)
        {
            Id = id;
            Name = name;
        }

        public Ids Id { get; private set; }
        public string Name { get; private set; }

        public AsymmetricAlgorithm GetCryptoServiceProvider()
        {
            var providerTypeId = Id;

            var cspParameter = new CspParameters((int)providerTypeId);

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
                    throw new NotImplementedException($"Provider type '{this}' not supported.");
            }
        }

        public IEnumerable<Container> GetContainers(Scope scope)
        {
            return Container.EnumerateContainers(Id, scope);
        }

        public override string ToString()
        {
            return $"#{(uint)Id:00} {Name}"; 
        }



        public static IDictionary<Ids, ProviderType> GetProviderTypes()
        {
            var providerTypes = new Dictionary<Ids, ProviderType>();

            var index = 0;
            var providerTypeId = Ids.NULL;
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

        public enum Ids : uint
        {
            NULL = 0,
            PROV_RSA_FULL = 1,
            PROV_RSA_SIG = 2,
            PROV_DSS = 3,
            PROV_FORTEZZA = 4,
            PROV_MS_EXCHANGE = 5,
            PROV_SSL = 6,
            PROV_RSA_SCHANNEL = 12,
            PROV_DSS_DH = 13,
            PROV_EC_ECDSA_SIG = 14,
            PROV_EC_ECNRA_SIG = 15,
            PROV_EC_ECDSA_FULL = 16,
            PROV_EC_ECNRA_FULL = 17,
            PROV_DH_SCHANNEL = 18,
            PROV_SPYRUS_LYNKS = 20,
            PROV_RNG = 21,
            PROV_INTEL_SEC = 22,
            PROV_REPLACE_OWF = 23,
            PROV_RSA_AES = 24
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptEnumProviderTypes(int dwIndex, IntPtr pdwReserved, int dwFlags, ref Ids pdwProvType, StringBuilder pszTypeName, ref int pcbTypeName);
    }
}
