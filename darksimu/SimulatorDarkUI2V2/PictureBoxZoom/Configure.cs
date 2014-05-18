using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Simulator
{
    public partial class Configure : MetroForm
    {
        public Configure ()
        {
            InitializeComponent();
        }

        private void button1_Click (object sender, EventArgs e)
        {
            Simulator.thrdtime = Convert.ToInt32(textBox1.Text);
            Simulator.modulo = Convert.ToInt32(textBox2.Text);
            Simulator.sonatThrd = Convert.ToInt32(textBox3.Text);
            Simulator.dirThrd = Convert.ToInt32(textBox4.Text);
            this.Close();
        }

        private void button2_Click (object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
