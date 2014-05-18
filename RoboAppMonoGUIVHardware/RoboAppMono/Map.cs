using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RoboAppMono
{
    public partial class Map : Form
    {
        public PictureBoxZoom _pictureBoxZoom;
        public Bitmap b1 = new Bitmap("bmp1.bmp");
        public Bitmap b2 = new Bitmap("bmp2.bmp");
        Thread drawThrd;

        public Map ()
        {
            InitializeComponent();
            try
            {
                pictureBox.Image = new Bitmap("bmp1.bmp");

                _pictureBoxZoom.Reset();
            }
            catch
            {
            }

            _pictureBoxZoom = new PictureBoxZoom(pictureBox);
            _pictureBoxZoom.OnZoomChange += UpdateZoomComboBox;
            UpdateZoomComboBox();
            ///////////////end zoom

            Form.CheckForIllegalCrossThreadCalls = false;
        }

        private void UpdateZoomComboBox ()
        {
            List<float> range = new List<float>(new float[] { 8, 4, 2, 1, .75f, .5f, .25f, .1f });

            if(!range.Contains(_pictureBoxZoom.ZoomFactor))
            {
                range.Add(_pictureBoxZoom.ZoomFactor);
            }

            range.Sort();
            range.Reverse();

            comboBoxZoom.FormattingEnabled = true;
            comboBoxZoom.FormatString = "##0%";
            comboBoxZoom.Items.Clear();
            comboBoxZoom.Items.AddRange(Array.ConvertAll<float, object>(range.ToArray(),
                new Converter<float, object>(element => element)));

            comboBoxZoom.SelectedItem = _pictureBoxZoom.ZoomFactor;
        }
        private void comboBoxZoom_SelectedIndexChanged (object sender, EventArgs e)
        {
            _pictureBoxZoom.Zoom((float)comboBoxZoom.SelectedItem);
        }

        private void button1_Click (object sender, EventArgs e)
        {
            drawThrd = new Thread(drawloop);
            drawThrd.Start();
        }

        public void drawloop ()
        {
            for( ; ;)
            {
                SuperimposeImage(RoboApp.currentx, RoboApp.currenty);
                label1.Text = RoboApp.currentx.ToString() + "," + RoboApp.currentx.ToString();
                label2.Text = "Current Direction : "+RoboNav.currentdir.ToString();
           
                Thread.Sleep(10);
            }

        }

        void SuperimposeImage (int x, int y)
        {
            //load both images
            Bitmap nnb1 = new Bitmap(b1);
            Image mainImage = nnb1;
            Image imposeImage = b2;
            try
            {
                //create graphics from main image
                using(Graphics g = Graphics.FromImage(mainImage))
                {
                    //draw other image on top of main Image
                    g.DrawImage(imposeImage, new Point(x + 15, y + 15));
                    pictureBox.Image = mainImage;
                    //save new image
                    //mainImage.Save("bmp3.bmp");
                }
            }catch(Exception uu)
            {
                label2.Text = uu.ToString();
            }

        }

        private void Map_Load (object sender, EventArgs e)

        {
            label1.Text = RoboApp.currentx.ToString() + "," + RoboApp.currentx.ToString();
            label2.Text = "Current Direction : " + RoboNav.currentdir.ToString();
           
            Bitmap nb1 = b1;
            pictureBox.Image = nb1;
        }

        private void openToolStripMenuItem_Click (object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            try
            {
                 b1= new Bitmap(openFileDialog1.FileName);
                 pictureBox.Image = b1;
               
                // reset the size
                _pictureBoxZoom.Reset();
            }
            catch
            {
                toolStripStatusLabel.Text = "Failed to load the image: " + openFileDialog1.FileName;
            }

        }

        private void Map_FormClosing (object sender, FormClosingEventArgs e)
        {
            try {
                drawThrd.Abort();
            }
            catch { }
        }
    }
}
