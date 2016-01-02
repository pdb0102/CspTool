using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace amaic.de.csptool.cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Csp")]
    public class GetCsp : Cmdlet
    {
        [Parameter(HelpMessage ="Enumerates provider types.")]
        public SwitchParameter ProviderTypes { get; set; }

        protected override void BeginProcessing()
        {
            if (ProviderTypes)
            {
                WriteObject(Crypt.EnumerateProviderTypes());
                return;
            }
        }
    }
}
