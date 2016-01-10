using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace amaic.de.csptool
{

    class Program
    {
        static int Main(string[] parameterListe)
        {
            if (parameterListe.Length != 3)
                return (int)ExitCodes.WrongParameterCount;

            var providerName = parameterListe[0];
            if (String.IsNullOrWhiteSpace(providerName))
                return (int)ExitCodes.ProviderNameIsNullOrWhiteSpace;

            if (String.IsNullOrWhiteSpace(parameterListe[1]))
                return (int)ExitCodes.ProviderNameIsNullOrWhiteSpace;
            int providerTypeId;
            if (Int32.TryParse(parameterListe[1], out providerTypeId) == false)
                return (int)ExitCodes.ProviderTypeIdIsNoInteger;

            if (String.IsNullOrWhiteSpace(parameterListe[2]))
                return (int)ExitCodes.ScopeIsNullOrWhiteSpace;
            int scope;
            if (Int32.TryParse(parameterListe[2], out scope) == false)
                return (int)ExitCodes.ScopeIsNoInteger;

            try
            {
                var providerHandle = Container.GetProviderHandle(providerName, (ProviderType.Ids)providerTypeId, (Scope)scope);
                providerHandle.Dispose();
            }
            catch
            {
                return (int)ExitCodes.WrongProviderParameters;
            }

            return (int)ExitCodes.Success;
        }
    }
}
