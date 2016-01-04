﻿using System;
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

        [Parameter(ParameterSetName = "DefaultProvider", Position = 1, Mandatory = true, HelpMessage = "Provider type id for which the default provider is requested.")]
        [ValidateRange(1,99)]
        public int ProviderTypeId { get; set; }

        [Parameter(ParameterSetName = "DefaultProvider", HelpMessage = "Machine scope.")]
        public SwitchParameter Machine { get; set; }



        protected override void BeginProcessing()
        {
            if (ProviderTypes)
            {
                WriteObject(Crypt.GetProviderTypes());
                return;
            }
            else if(DefaultProvider)
            {
                WriteObject(Crypt.GetDefaultProvider(ProviderTypeId, Machine));
                return;
            }

            foreach (var provider in Crypt.EnumerateProviders())
            {
                WriteObject(provider);
            }
        }
    }
}
