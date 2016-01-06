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
        [Parameter(ParameterSetName = "ProviderTypes", Position = 0, HelpMessage = "Returns provider types dictionary.")]
        public SwitchParameter ProviderTypes { get; set; }


        [Parameter(ParameterSetName = "DefaultProvider", Position = 0, Mandatory = true, HelpMessage = "Returns default provider.")]
        public SwitchParameter DefaultProvider { get; set; }


        [Parameter(ParameterSetName = "KeyContainers", Position = 0, Mandatory = true, HelpMessage = "Returns key containers.")]
        public SwitchParameter KeyContainers { get; set; }


        [Parameter(ParameterSetName = "Provider", Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public Provider Provider { get; set; }



        [Parameter(ParameterSetName = "DefaultProvider", Position = 1, Mandatory = true, HelpMessage = "Provider type id of default provider.")]
        [Parameter(ParameterSetName = "KeyContainers", Position = 1, Mandatory = true, HelpMessage = "Provider type id of key containers.")]
        public ProviderType.Ids ProviderTypeId { get; set; }

        [Parameter(ParameterSetName = "DefaultProvider", HelpMessage = "Machine scope.")]
        [Parameter(ParameterSetName = "KeyContainers", HelpMessage = "Machine scope.")]
        [Parameter(ParameterSetName = "Provider", HelpMessage = "Machine scope.")]
        public SwitchParameter Machine { get; set; }


        bool _handled = false;

        protected override void BeginProcessing()
        {
            if (ProviderTypes)
            {
                foreach (var providerType in ProviderType.GetProviderTypes().Values)
                {
                    WriteObject(providerType);
                }
                _handled = true;
            }
            else if(DefaultProvider)
            {
                WriteObject(Provider.GetDefaultProvider(ProviderTypeId, Machine ? Scope.Machine : Scope.User));
                _handled = true;
            }
            else if (KeyContainers)
            {
                foreach (var container in ProviderType.EnumerateContainers(ProviderTypeId, Machine ? Scope.Machine : Scope.User))
                {
                    WriteObject(container);
                }
                _handled = true;
            }
        }

        protected override void ProcessRecord()
        {
            if (_handled) return;

            if (Provider == null)
            {
                foreach (var provider in Provider.EnumerateProviders())
                {
                    WriteObject(provider);
                }
            }
            else
            {
                foreach (var container in Provider.EnumerateContainers(Machine ? Scope.Machine : Scope.User))
                {
                    WriteObject(container);
                }
            }
        }
    }
}
