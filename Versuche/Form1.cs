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
            foreach(var providerType in Crypt.EnumerateProviderTypes())
            {
                Ausgabe.AppendText($"#{providerType.Key} {providerType.Value} \n");       
            }
        }
    }
}
