using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace amaic.de.csptool
{
    public class Container
    {
        public Container(string name, Provider provider, Scope scope)
        {
            Name = name;
            Provider = provider;
            Scope = scope;
        }

        public string Name { get; private set; }
        public Provider Provider { get; private set; }
        public Scope Scope { get; private set; }

        public string UniqueName
        {
            get
            {
                var acFlags = CryptAcquireContextFlags.NULL;
                if (Scope == Scope.Machine) acFlags |= CryptAcquireContextFlags.CRYPT_MACHINE_KEYSET;

                var providerHandle = GetProviderHandle(acFlags);

                var uniqueKeyContainerNameLength_Bytes = 0;
                if (CryptGetProvParam(providerHandle, CryptGetProvParamParameterTypes.PP_UNIQUE_CONTAINER, null, ref uniqueKeyContainerNameLength_Bytes, 0) == false)
                    throw new Win32Exception();

                var uniqueKeyContainerName = new byte[uniqueKeyContainerNameLength_Bytes];
                if (CryptGetProvParam(providerHandle, CryptGetProvParamParameterTypes.PP_UNIQUE_CONTAINER, uniqueKeyContainerName, ref uniqueKeyContainerNameLength_Bytes, 0) == false)
                    throw new Win32Exception();

                providerHandle.Dispose();

                return Encoding.ASCII.GetString(uniqueKeyContainerName).Trim('\0');                
            }
        }

        public string FilePath
        {
            get
            {
                return Path.Combine(
                    GetKeyDiretory(GetKeyType(), GetRsaDss()),
                    UniqueName
                    );
            }
        }

        ProviderHandle GetProviderHandle(CryptAcquireContextFlags flags)
        {
            ProviderHandle providerHandle;

            if (CryptAcquireContext(out providerHandle, Name, Provider.Name, Provider.ProviderType.Id, flags) == false)
                throw new Win32Exception();

            return providerHandle;
        }

        KeyTypes GetKeyType()
        {
            return Scope == Scope.Machine ? KeyTypes.SharedPrivate : KeyTypes.UserPrivate;
        }

        RsaDss GetRsaDss()
        {
            var providerTypeId = Provider.ProviderType.Id;
            switch (providerTypeId)
            {
                case ProviderType.Ids.PROV_RSA_FULL:
                case ProviderType.Ids.PROV_RSA_SIG:
                case ProviderType.Ids.PROV_RSA_SCHANNEL:
                case ProviderType.Ids.PROV_RSA_AES:
                    return RsaDss.RSA;

                case ProviderType.Ids.PROV_DSS:
                case ProviderType.Ids.PROV_DSS_DH:
                    return RsaDss.DSS;

                case ProviderType.Ids.PROV_FORTEZZA:
                case ProviderType.Ids.PROV_MS_EXCHANGE:
                case ProviderType.Ids.PROV_SSL:
                case ProviderType.Ids.PROV_EC_ECDSA_SIG:
                case ProviderType.Ids.PROV_EC_ECNRA_SIG:
                case ProviderType.Ids.PROV_EC_ECDSA_FULL:
                case ProviderType.Ids.PROV_EC_ECNRA_FULL:
                case ProviderType.Ids.PROV_DH_SCHANNEL:
                case ProviderType.Ids.PROV_SPYRUS_LYNKS:
                case ProviderType.Ids.PROV_RNG:
                case ProviderType.Ids.PROV_INTEL_SEC:
                case ProviderType.Ids.PROV_REPLACE_OWF:
                case ProviderType.Ids.NULL:
                default:
                    throw new NotImplementedException();
            }

        }

        AsymmetricAlgorithm GetCryptoServiceProvider()
        {
            var provider = Provider;
            var providerType = provider.ProviderType;
            var providerTypeId = providerType.Id;

            var cspParameter = new CspParameters((int)providerTypeId, provider.Name, Name);

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

        public override string ToString()
        {
            return $"{Name} [{Provider}, {Scope}]";
        }

        public static IEnumerable<Container> EnumerateContainers(ProviderType.Ids providerTypeId, Scope scope)
        {
            return EnumerateContainers(null, providerTypeId, scope);
        }
        public static IEnumerable<Container> EnumerateContainers(ProviderType providerType, Scope scope)
        {
            if (providerType == null) throw new ArgumentNullException(nameof(providerType));

            return EnumerateContainers(null, providerType.Id, scope);
        }
        public static IEnumerable<Container> EnumerateContainers(Provider provider, Scope scope)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            if (provider.IsBadProvider(scope)) return new Container[0];

            return EnumerateContainers(provider.Name, provider.ProviderType.Id, scope);
        }
        public static IEnumerable<Container> EnumerateContainers(string providerName, ProviderType.Ids providerTypeId, Scope scope)
        {
            var acFlags = CryptAcquireContextFlags.CRYPT_VERIFYCONTEXT;
            if (scope == Scope.Machine) acFlags |= CryptAcquireContextFlags.CRYPT_MACHINE_KEYSET;

            var providerHandle = GetProviderHandle(null, providerName, providerTypeId, acFlags);

            var containerNameMaxLength_Bytes = 0;
            if (CryptEnumerateContainerNames(providerHandle, null, ref containerNameMaxLength_Bytes, EnumerationFlags.CRYPT_FIRST) == false)
                throw new Win32Exception();

            var providerTypes = ProviderType.GetProviderTypes();

            var containerName = new byte[containerNameMaxLength_Bytes];
            var gppFlags = EnumerationFlags.CRYPT_FIRST;
            while (CryptEnumerateContainerNames(providerHandle, containerName, ref containerNameMaxLength_Bytes, gppFlags))
            {
                yield return
                    new Container(
                        Encoding.ASCII.GetString(containerName).TrimEnd('\0'),
                        new Provider(
                            GetProviderName(providerHandle),
                            providerTypes[providerTypeId]
                            ),
                        scope
                        );

                Array.Clear(containerName, 0, containerName.Length);
                gppFlags = EnumerationFlags.CRYPT_NEXT;
            }

            providerHandle.Dispose();
        }

        static string GetProviderName(ProviderHandle providerHandle)
        {
            if (providerHandle == null) throw new ArgumentNullException(nameof(providerHandle));

            var providerNameLength_Bytes = 0;
            if (CryptGetProvParam(providerHandle, CryptGetProvParamParameterTypes.PP_NAME, null, ref providerNameLength_Bytes, 0) == false)
                throw new Win32Exception();

            var providerName = new byte[providerNameLength_Bytes];
            if (CryptGetProvParam(providerHandle, CryptGetProvParamParameterTypes.PP_NAME, providerName, ref providerNameLength_Bytes, 0) == false)
                throw new Win32Exception();

            return Encoding.ASCII.GetString(providerName).TrimEnd('\0');
        }

        public static ProviderHandle GetProviderHandle(string providerName, ProviderType.Ids providerTypeId, Scope scope)
        {
            var acFlags = CryptAcquireContextFlags.NULL;
            if (scope == Scope.Machine) acFlags |= CryptAcquireContextFlags.CRYPT_MACHINE_KEYSET;

            ProviderHandle providerHandle;
            if (CryptAcquireContext(out providerHandle, null, providerName, providerTypeId, acFlags) == false)
                throw new Win32Exception();

            return providerHandle;
        }
        static ProviderHandle GetProviderHandle(string containerName, string providerName, ProviderType.Ids providerTypeId, CryptAcquireContextFlags acFlags)
        {
            ProviderHandle providerHandle;
            if (CryptAcquireContext(out providerHandle, containerName, providerName, providerTypeId, acFlags) == false)
                throw new Win32Exception();

            return providerHandle;
        }

        static string GetKeyDiretory(KeyTypes keyType, RsaDss rsaDss)
        {
            var baseDirectory = 
                keyType == KeyTypes.UserPrivate ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : Environment.GetEnvironmentVariable("ALLUSERSPROFILE");

            var rsaDssSubfolder = 
                rsaDss == RsaDss.RSA ? "RSA" : "DSS";

            string userSubfolder;
            switch (keyType)
            {
                case KeyTypes.UserPrivate:
                    userSubfolder = WindowsIdentity.GetCurrent().User.Value;
                    break;

                case KeyTypes.LocalSystemPrivate:
                    userSubfolder = "S-1-5-18";
                    break;

                case KeyTypes.LocalServicePrivate:
                    userSubfolder = "S-1-5-19";
                    break;

                case KeyTypes.NetworkServicePrivate:
                    userSubfolder = "S-1-5-20";
                    break;

                case KeyTypes.SharedPrivate:
                    userSubfolder = "MachineKeys";
                    break;

                default:
                    throw new NotImplementedException();
            }

            return Path.Combine(
                baseDirectory,
                @"Microsoft\Crypto",
                rsaDssSubfolder,
                userSubfolder
                );
        }

        enum KeyTypes
        {
            UserPrivate,
            LocalSystemPrivate,
            LocalServicePrivate,
            NetworkServicePrivate,
            SharedPrivate
        }

        enum RsaDss
        {
            RSA,
            DSS
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
        enum EnumerationFlags : uint
        {
            NULL = 0,
            CRYPT_FIRST = 1,
            CRYPT_NEXT = 2,
            CRYPT_SGC_ENUM = 4
        }

        [Flags]
        enum SecurityDescriptorFlags : uint
        {
            NULL = 0,
            OWNER_SECURITY_INFORMATION = 0x00000001,
            GROUP_SECURITY_INFORMATION = 0x00000002,
            DACL_SECURITY_INFORMATION = 0x00000004,
            SACL_SECURITY_INFORMATION = 0x00000008
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

        enum SecurityDescriptorControl : ushort
        {
            NULL = 0,
            SE_OWNER_DEFAULTED = 0x0001,
            SE_GROUP_DEFAULTED = 0x0002,
            SE_DACL_PRESENT = 0x0004,
            SE_DACL_DEFAULTED = 0x0008,
            SE_SACL_PRESENT = 0x0010,
            SE_SACL_DEFAULTED = 0x0020,
            SE_DACL_AUTO_INHERIT_REQ = 0x0100,
            SE_SACL_AUTO_INHERIT_REQ = 0x0200,
            SE_DACL_AUTO_INHERITED = 0x0400,
            SE_SACL_AUTO_INHERITED = 0x0800,
            SE_DACL_PROTECTED = 0x1000,
            SE_SACL_PROTECTED = 0x2000,
            SE_RM_CONTROL_VALID = 0x4000,
            SE_SELF_RELATIVE = 0x8000
        }

        const int SDDL_REVISION_1 = 1;

        enum SecurityInformation : uint
        {
            NULL = 0,
            OWNER_SECURITY_INFORMATION = 0x00000001,
            GROUP_SECURITY_INFORMATION = 0x00000002,
            DACL_SECURITY_INFORMATION = 0x00000004,
            SACL_SECURITY_INFORMATION = 0x00000008,
            LABEL_SECURITY_INFORMATION = 0x00000010,
            ATTRIBUTE_SECURITY_INFORMATION = 0x00000020,
            SCOPE_SECURITY_INFORMATION = 0x00000040,
            PROCESS_TRUST_LABEL_SECURITY_INFORMATION = 0x00000080,
            BACKUP_SECURITY_INFORMATION = 0x00010000,

            PROTECTED_DACL_SECURITY_INFORMATION = 0x80000000,
            PROTECTED_SACL_SECURITY_INFORMATION = 0x40000000,
            UNPROTECTED_DACL_SECURITY_INFORMATION = 0x20000000,
            UNPROTECTED_SACL_SECURITY_INFORMATION = 0x10000000
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptAcquireContext(out ProviderHandle hProv, string pszContainer, string pszProvider, ProviderType.Ids dwProvType, CryptAcquireContextFlags dwFlags);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CryptGetProvParam(ProviderHandle hProv, CryptGetProvParamParameterTypes dwParam, [Out] byte[] pbData, ref int dwDataLen, uint dwFlags);
        static bool CryptEnumerateContainerNames(ProviderHandle providerHandle, byte[] containerName, ref int containerNameLength_Bytes, EnumerationFlags flags)
        {
            return CryptGetProvParam(providerHandle, CryptGetProvParamParameterTypes.PP_ENUMCONTAINERS, containerName, ref containerNameLength_Bytes, (uint)flags);
        }
        static bool CryptGetSecurityDescriptor(ProviderHandle providerHandle, byte[] securityDescriptor, ref int securityDescriptorLength_Bytes, SecurityDescriptorFlags flags)
        {
            return CryptGetProvParam(providerHandle, CryptGetProvParamParameterTypes.PP_KEYSET_SEC_DESCR, securityDescriptor, ref securityDescriptorLength_Bytes, (uint)flags);
        }

        [DllImport("advapi32.dll")]
	    static extern Int32 GetSecurityDescriptorLength(byte[] pSecurityDescriptor);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetSecurityDescriptorControl(byte[] pSecurityDescriptor, ref SecurityDescriptorControl pControl, ref int lpdwRevision);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetSecurityDescriptorOwner(byte[] pSecurityDescriptor, out IntPtr pOwner, out bool lpbOwnerDefaulted);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool ConvertSecurityDescriptorToStringSecurityDescriptor(byte[] SecurityDescriptor, int RequestedStringSDRevision, SecurityInformation SecurityInformation, ref StringBuilder StringSecurityDescriptor, ref long StringSecurityDescriptorLen);


#if DEBUG
        public void Versuch1()
        {
            var providerHandle = GetProviderHandle(Name, Provider.Name, Provider.ProviderType.Id, CryptAcquireContextFlags.NULL);

            var securityDescriptorLength_Bytes = 0;
            if (CryptGetSecurityDescriptor(providerHandle, null, ref securityDescriptorLength_Bytes, SecurityDescriptorFlags.OWNER_SECURITY_INFORMATION) == false)
                throw new Win32Exception();

            var securityDescriptor = new byte[securityDescriptorLength_Bytes];
            if (CryptGetSecurityDescriptor(providerHandle, securityDescriptor, ref securityDescriptorLength_Bytes, SecurityDescriptorFlags.OWNER_SECURITY_INFORMATION) == false)
                throw new Win32Exception();

            providerHandle.Dispose();

            var securityDescriptorControl = SecurityDescriptorControl.NULL;
            var revision = 0;
            if (GetSecurityDescriptorControl(securityDescriptor, ref securityDescriptorControl, ref revision) == false)
                throw new Win32Exception();

            var stringSecurityDescriptor = new StringBuilder();
            var stringSecurityDescriptorLength_Bytes = 0L;
            if (ConvertSecurityDescriptorToStringSecurityDescriptor(securityDescriptor, SDDL_REVISION_1, SecurityInformation.OWNER_SECURITY_INFORMATION, ref stringSecurityDescriptor, ref stringSecurityDescriptorLength_Bytes) == false)
                throw new Win32Exception();

            IntPtr securityDescriptorOwner;
            bool defaulted;
            if (GetSecurityDescriptorOwner(securityDescriptor, out securityDescriptorOwner, out defaulted) == false)
                throw new Win32Exception();

            var s = new SecurityIdentifier(securityDescriptorOwner);
        }
#endif

    }
}

