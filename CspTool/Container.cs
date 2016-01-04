using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace amaic.de.csptool
{
    public class Container
    {
        public Container(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        public static IEnumerable<Container> EnuemrateContainers(ProviderType.Ids providerTypeId, bool machine)
        {
            ProviderHandle providerHandle;
            var acFlags = CryptAcquireContextFlags.CRYPT_VERIFYCONTEXT;
            if (machine) acFlags |= CryptAcquireContextFlags.CRYPT_MACHINE_KEYSET;
            if (CryptAcquireContext(out providerHandle, null, null, providerTypeId, acFlags) == false)
                throw new Win32Exception();

            var containerNameMaxLength_Bytes = 0;
            if (CryptGetProvParam(providerHandle, CryptGetProvParamParameterTypes.PP_ENUMCONTAINERS, null, ref containerNameMaxLength_Bytes, CryptGetProvParamFlags.CRYPT_FIRST) == false)
                throw new Win32Exception();


            var containerName = new byte[containerNameMaxLength_Bytes];
            var gppFlags = CryptGetProvParamFlags.CRYPT_FIRST;
            while (CryptGetProvParam(providerHandle, CryptGetProvParamParameterTypes.PP_ENUMCONTAINERS, containerName, ref containerNameMaxLength_Bytes, gppFlags))
            {
                yield return new Container(Encoding.ASCII.GetString(containerName).TrimEnd('\0'));

                Array.Clear(containerName, 0, containerName.Length);
                gppFlags = CryptGetProvParamFlags.CRYPT_NEXT;
            }

            providerHandle.Dispose();
        }

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

        [Flags]
        enum CryptGetProvParamFlags : uint
        {
            CRYPT_FIRST = 1,
            CRYPT_NEXT = 2,
            CRYPT_SGC_ENUM = 4
        }

        enum CryptGetProvParamParameterTypes : uint
        {
            NULL = 0,
            PP_ENUMALGS = 1,
            PP_ENUMCONTAINERS = 2,
            PP_IMPTYPE = 3,
            PP_NAME = 4,
            PP_VERSION = 5,
            PP_CONTAINER = 6,
            PP_CHANGE_PASSWORD = 7,
            PP_KEYSET_SEC_DESCR = 8,
            PP_CERTCHAIN = 9,
            PP_KEY_TYPE_SUBTYPE = 10,
            PP_PROVTYPE = 16,
            PP_KEYSTORAGE = 17,
            PP_APPLI_CERT = 18,
            PP_SYM_KEYSIZE = 19,
            PP_SESSION_KEYSIZE = 20,
            PP_UI_PROMPT = 21,
            PP_ENUMALGS_EX = 22,
            PP_ENUMMANDROOTS = 25,
            PP_ENUMELECTROOTS = 26,
            PP_KEYSET_TYPE = 27,
            PP_ADMIN_PIN = 31,
            PP_KEYEXCHANGE_PIN = 32,
            PP_SIGNATURE_PIN = 33,
            PP_SIG_KEYSIZE_INC = 34,
            PP_KEYX_KEYSIZE_INC = 35,
            PP_UNIQUE_CONTAINER = 36,
            PP_SGC_INFO = 37,
            PP_USE_HARDWARE_RNG = 38,
            PP_KEYSPEC = 39,
            PP_ENUMEX_SIGNING_PROT = 40,
            PP_CRYPT_COUNT_KEY_USE = 41,
            PP_USER_CERTSTORE = 42,
            PP_SMARTCARD_READER = 43,
            PP_SMARTCARD_GUID = 45,
            PP_ROOT_CERTSTORE = 46,
            PP_SMARTCARD_READER_ICON = 47
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptAcquireContext(out ProviderHandle hProv, string pszContainer, string pszProvider, ProviderType.Ids dwProvType, CryptAcquireContextFlags dwFlags);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptGetProvParam(ProviderHandle hProv, CryptGetProvParamParameterTypes dwParam, [Out] byte[] pbData, ref int dwDataLen, CryptGetProvParamFlags dwFlags);
    }
}
