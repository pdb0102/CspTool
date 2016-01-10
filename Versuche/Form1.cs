using amaic.de.csptool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
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
            Versuch7();
#endif
        }


#if DEBUG
        private void Versuch9()
        {
        }

        private void Versuch8()
        {
            foreach (var provider in Provider.EnumerateProviders())
            {
                Ausgabe.AppendText($"{provider}: {provider.IsBadProvider(Scope.User)}{Environment.NewLine}");
            }
        }

        private async void Versuch7()
        {
            try
            {
                foreach (var provider in Provider.EnumerateProviders())
                {
                    foreach (Scope scope in Enum.GetValues(typeof(Scope)))
                    {
                        foreach (var container in provider.EnumerateContainers(scope))
                        {
                            var containerfilePathTask = Task.Run(() => container.FilePath);
                            try
                            {
                                var containerfilePath = await containerfilePathTask;
                                Ausgabe.AppendText($"{containerfilePath}: {File.Exists(containerfilePath)}{Environment.NewLine}");
                            }
                            catch (Win32Exception ausnahmefehler)
                            {
                                Ausgabe.AppendText($"{container.Name}: {ausnahmefehler.Message}{Environment.NewLine}");
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException) { }
        }

        IEnumerable<string> EnumerateProviderFilePaths()
        {
            foreach (var provider in Provider.EnumerateProviders())
            {
                foreach (Scope scope in Enum.GetValues(typeof(Scope)))
                {
                    foreach (var container in provider.EnumerateContainers(scope))
                    {
                        string providerFilePath;
                        try
                        {
                            var filePath = container.FilePath;
                            providerFilePath = $"{filePath}: {File.Exists(filePath)}{Environment.NewLine}";
                        }
                        catch (Win32Exception ausnahmefehler)
                        {
                            providerFilePath = $"{container.Name}: {ausnahmefehler.Message}{Environment.NewLine}";
                        }
                        yield return providerFilePath;
                    }
                }
            }
        }

        private void Versuch5()
        {
            var defaultProvider = Provider.GetDefaultProvider(ProviderType.Ids.PROV_RSA_AES, Scope.User);

            var containers = defaultProvider.EnumerateContainers(Scope.User).ToArray();
            foreach (var container in containers)
            {
                Ausgabe.AppendText($"{container.UniqueName}{Environment.NewLine}");
            }
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

                foreach (var container in providerType.EnumerateContainers(Scope.User))
                {
                    Ausgabe.AppendText($"|-- {container} [{container.Provider}]{Environment.NewLine}");
                }
            }


            //foreach (var container in CspTool.Container.EnuemrateContainers(1, false))
            //{
            //    Ausgabe.AppendText(container.Name + Environment.NewLine);
            //}

        }
#endif
    }
}
