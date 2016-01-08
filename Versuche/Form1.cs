using amaic.de.csptool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        private void Versuch7()
        {
            foreach (var provider in Provider.EnumerateProviders())
            {
                foreach (Scope scope in Enum.GetValues(typeof(Scope)))
                {
                    foreach (var container in provider.EnumerateContainers(scope))
                    {
                        try
                        {
                            var filePath = container.FilePath;
                            if(filePath == @"C:\ProgramData\Microsoft\Crypto\RSA\S-1-5-18\fc1e3851f429ea606d6ff1e01a5229f1_87e3ec08-4530-49c5-845d-79692c4e3213")
                            {

                            }
                            Debug.Print($"{filePath}{Environment.NewLine}");
                            //Ausgabe.AppendText($"{container.FilePath}{Environment.NewLine}");
                        }
                        catch (Win32Exception ausnahmefehler)
                        {
                            Debug.Print($"{container.Name}: {ausnahmefehler.Message}{Environment.NewLine}");
                            //Thread.Sleep(1000);
                            //Ausgabe.AppendText($"{container.Name}: {ausnahmefehler.Message}{Environment.NewLine}");
                        }
                    }
                }
            }
        }

        private void Versuch6()
        {
            foreach (CspTool.Container.KeyTypes keyType in Enum.GetValues(typeof(CspTool.Container.KeyTypes)))
            {
                foreach (CspTool.Container.RsaDss rsaDss in Enum.GetValues(typeof(CspTool.Container.RsaDss)))
                {
                    var directory = CspTool.Container.GetKeyDiretory(keyType, rsaDss);
                    Ausgabe.AppendText($"{directory}: {Directory.Exists(directory)}{Environment.NewLine}");
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
