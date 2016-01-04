using System;
using System.Collections.Generic;
using System.Linq;
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

        public override string ToString()
        {
            return Name;
        }
    }
}
