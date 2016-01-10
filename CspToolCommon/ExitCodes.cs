using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amaic.de.csptool
{
    public enum ExitCodes : int
    {
        Success = 0,
        WrongParameterCount = 1,
        ProviderNameIsNullOrWhiteSpace = 2,
        ProviderTypeIdIsNullOrWhiteSpace = 3,
        ProviderTypeIdIsNoInteger = 4,
        ScopeIsNullOrWhiteSpace = 5,
        ScopeIsNoInteger = 6,
        WrongProviderParameters = 7,
        KillMe = 99999 // Well, never sent back because application is frozen
    }

}
