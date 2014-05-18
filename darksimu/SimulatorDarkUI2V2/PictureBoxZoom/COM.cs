using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Simulator
{
    public partial class COM : MetroForm
    {
        SerialPort _serialPort;
        SerialPort _serialPort2;

        public COM ()
        {
            InitializeComponent();
        }

        private void COM_Load (object sender, EventArgs e)
        {
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;


            foreach(string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
                //comboBox12.Items.Add(s);
            }

            comboBox2.Items.Add("9600");
            comboBox11.Items.Add("9600");
            foreach(string s in Enum.GetNames(typeof(Parity)))
            {
                comboBox3.Items.Add(s);
                comboBox10.Items.Add(s);
            }
            comboBox4.Items.Add("8");
            comboBox9.Items.Add("8");
            foreach(string s in Enum.GetNames(typeof(StopBits)))
            {
                comboBox5.Items.Add(s);
                comboBox8.Items.Add(s);
            }
            foreach(string s in Enum.GetNames(typeof(Handshake)))
            {
                comboBox6.Items.Add(s);
                comboBox7.Items.Add(s);
            }
        }

        private void button3_Click (object sender, EventArgs e)
        {
            _serialPort = new SerialPort();


            _serialPort.PortName = comboBox1.SelectedItem.ToString();
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;


            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;


            _serialPort2 = new SerialPort();


            _serialPort2.PortName = textBox1.Text.ToString();
            _serialPort2.BaudRate = 9600;
            _serialPort2.Parity = Parity.None;
            _serialPort2.DataBits = 8;
            _serialPort2.StopBits = StopBits.One;
            _serialPort2.Handshake = Handshake.None;


            _serialPort2.ReadTimeout = 500;
            _serialPort2.WriteTimeout = 500;


            try
            {


                Simulator x = new Simulator(_serialPort, _serialPort2);
                this.Hide();
                x.Show();

            }
            catch(Exception ex)
            {

                MessageBox.Show(ex.ToString());

            }
        }

        private void button1_Click (object sender, EventArgs e)
        {
            _serialPort = new SerialPort();
            _serialPort2 = new SerialPort();

            _serialPort.PortName = comboBox1.SelectedItem.ToString();
            _serialPort.BaudRate = Convert.ToInt32(comboBox2.SelectedItem.ToString());
            _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), comboBox3.SelectedItem.ToString());
            _serialPort.DataBits = int.Parse(comboBox4.SelectedItem.ToString());
            _serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), comboBox5.SelectedItem.ToString());
            _serialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), comboBox6.SelectedItem.ToString());


            //_serialPort2.PortName = comboBox12.SelectedItem.ToString();
            _serialPort2.BaudRate = Convert.ToInt32(comboBox11.SelectedItem.ToString());
            _serialPort2.Parity = (Parity)Enum.Parse(typeof(Parity), comboBox10.SelectedItem.ToString());
            _serialPort2.DataBits = int.Parse(comboBox9.SelectedItem.ToString());
            _serialPort2.StopBits = (StopBits)Enum.Parse(typeof(StopBits), comboBox8.SelectedItem.ToString());
            _serialPort2.Handshake = (Handshake)Enum.Parse(typeof(Handshake), comboBox7.SelectedItem.ToString());


            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _serialPort2.ReadTimeout = 500;
            _serialPort2.WriteTimeout = 500;

            try
            {


                Simulator x = new Simulator(_serialPort, _serialPort2);
                this.Hide();
                x.Show();

            }
            catch(Exception ex)
            {

                MessageBox.Show(ex.ToString());

            }
        }

        private void button2_Click (object sender, EventArgs e)
        {
            Simulator x = new Simulator();
            this.Hide();
            x.Show();
        }

        private void label12_Click (object sender, EventArgs e)
        {

        }
    }
}
