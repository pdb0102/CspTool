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
            var providerTypes = Crypt.GetProviderTypes();

            foreach (var providerType in providerTypes.Values)
            {
                var defaultProvider = Crypt.GetDefaultProvider(providerType.Id, false);
                Ausgabe.AppendText($"USER: {providerType.Name} -> {defaultProvider.Name} \n");

                defaultProvider = Crypt.GetDefaultProvider(providerType.Id, true);
                Ausgabe.AppendText($"MACH: {providerType.Name} -> {defaultProvider.Name} \n");
            }
        }
    }
}
