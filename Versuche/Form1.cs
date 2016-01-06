using amaic.de.csptool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CspTool = amaic.de.csptool;

namespace Versuche
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
#if DEBUG
            Versuch4();
#endif
        }

#if DEBUG
        private void Versuch4()
        {
            foreach(var providerType in ProviderType.GetProviderTypes())
            {
                Ausgabe.AppendText($"{providerType}{Environment.NewLine}");
            }

            var provider = Provider.GetDefaultProvider(ProviderType.Ids.PROV_RSA_FULL, Scope.Machine);

            foreach(var container in provider.EnumerateContainers(Scope.Machine))
            {
                container.Versuch2();
            }
        }

        private void Versuch3()
        {
            var defaultProvider = Provider.GetDefaultProvider(ProviderType.Ids.PROV_RSA_FULL, Scope.User);
            var containers = defaultProvider.EnumerateContainers(Scope.User);
            var alexander = containers.Where(c => c.Name == "Alexander").Single();
            alexander.Versuch2();

        }

        private void Versuch2()
        {
            var defaultProvider = Provider.GetDefaultProvider(ProviderType.Ids.PROV_RSA_FULL, Scope.User);
            var containers = defaultProvider.EnumerateContainers(Scope.User);
            var alexander = containers.Where(c => c.Name == "Alexander").Single();
            alexander.Versuch1();
            
        }

        void Versuch1()
        {
            foreach (var providerType in CspTool.ProviderType.GetProviderTypes().Values)
            {
                Ausgabe.AppendText($"{providerType}:{Environment.NewLine}");

                foreach (var container in providerType.GetContainers(Scope.User))
                {
                    Ausgabe.AppendText($"|-- {container} [{container.Provider}]{Environment.NewLine}");
                }
            }


            //foreach (var container in CspTool.Container.EnuemrateContainers(1, false))
            //{
            //    Ausgabe.AppendText(container.Name + Environment.NewLine);
            //}

        }
    }
#endif
}
