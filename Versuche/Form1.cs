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
            foreach (var providerType in CspTool.ProviderType.GetProviderTypes().Values)
            {
                Ausgabe.AppendText($"{providerType}:\n");

                foreach (var container in CspTool.Container.EnuemrateContainers(providerType.Id, true))
                {
                    Ausgabe.AppendText($"|-- {container}\n");
                }
            }


            //foreach (var container in CspTool.Container.EnuemrateContainers(1, false))
            //{
            //    Ausgabe.AppendText(container.Name + Environment.NewLine);
            //}
        }
    }
}
