using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics
{
    public partial class Settings : Form
    {
        Bitmap on, off;
        Dictionary<string, char> DefaultControls;
        public Settings()
        {
               InitializeComponent();
            DefaultControls = new Dictionary<string, char>();
            DefaultControls.Add("Forward", 'W');
            DefaultControls.Add("Backward", 'S');
            DefaultControls.Add("Left", 'A');
            DefaultControls.Add("Right", 'D');
            DefaultControls.Add("Switch", 'Z');

            on = new Bitmap("on.png");
             off = new Bitmap("off.png");
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            forwardtxt.Text = GraphicsForm.controls["Forward"].ToString();
            backwardtxt.Text = GraphicsForm.controls["Backward"].ToString();
            lefttxt.Text = GraphicsForm.controls["Left"].ToString();
            righttxt.Text = GraphicsForm.controls["Right"].ToString();
            switchtxt.Text = GraphicsForm.controls["Switch"].ToString();
            if (Renderer.SystemSoundIsOpened)
                bunifuImageButton1.Image = on;
            else
                bunifuImageButton1.Image = off;

            if (Renderer.GameSoundIsOpened)
                bunifuImageButton2.Image = on;
            else
                bunifuImageButton2.Image = off;

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }
    
        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            if (Renderer.SystemSoundIsOpened)
                bunifuImageButton1.Image = off;
            else
                bunifuImageButton1.Image = on;
            Renderer.SystemSoundIsOpened = !Renderer.SystemSoundIsOpened;
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            if (Renderer.GameSoundIsOpened)
                bunifuImageButton2.Image = off;
            else
                bunifuImageButton2.Image = on;
            Renderer.GameSoundIsOpened = !Renderer.GameSoundIsOpened;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
           
            if(string.IsNullOrWhiteSpace(forwardtxt.Text) || string.IsNullOrWhiteSpace(backwardtxt.Text)|| string.IsNullOrWhiteSpace(lefttxt.Text)|| string.IsNullOrWhiteSpace(righttxt.Text) || string.IsNullOrWhiteSpace(switchtxt.Text))
            {
                MessageBox.Show("Not Valid Game Controls");
                return;
            }
            GraphicsForm.controls["Forward"] = forwardtxt.Text[0];
            GraphicsForm.controls["Backward"] = backwardtxt.Text[0];
            GraphicsForm.controls["Left"] = lefttxt.Text[0];
            GraphicsForm.controls["Right"] = righttxt.Text[0];
            GraphicsForm.controls["Switch"] = switchtxt.Text[0];
            MessageBox.Show("Controls Updated");

        }

        private void bunifuImageButton4_Click(object sender, EventArgs e)
        {
            GraphicsForm.controls = DefaultControls;
            forwardtxt.Text = DefaultControls["Forward"].ToString();
            backwardtxt.Text = DefaultControls["Backward"].ToString();
            lefttxt.Text = DefaultControls["Left"].ToString();
            righttxt.Text = DefaultControls["Right"].ToString();
            switchtxt.Text = DefaultControls["Switch"].ToString();

        }

        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                e.Handled = false;
            else if ((!char.IsControl(e.KeyChar) && char.IsDigit(e.KeyChar) && char.IsSymbol(e.KeyChar)) || forwardtxt.Text.Length >= 1)
                e.Handled = true;
        }
    }
}
