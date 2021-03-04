using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using GlmNet;
using System.IO;
using System.Media;

namespace Graphics
{
    public partial class GraphicsForm : Form
    {
        bool mouse_move = false;
        bool option = false;
        SoundPlayer gunShoot = new SoundPlayer("gun-gunshot-01.wav");
        SoundPlayer walkingSound = new SoundPlayer("walking sound.wav");
        SoundPlayer playerCollisionSound = new SoundPlayer("collision.wav");
        Renderer renderer = new Renderer();
        renderload load_screen = new renderload();
        Thread MainLoopThread;
        float deltaTime;
        public static Dictionary<string, char> controls = new Dictionary<string, char>();
        public GraphicsForm()
        {
            InitializeComponent();

            simpleOpenGlControl1.InitializeContexts();
            //simpleOpenGlControl2.InitializeContexts();

            MoveCursor();

            initialize();
            deltaTime = 1f;
            MainLoopThread = new Thread(MainLoop);
            MainLoopThread.Start();

        }
        void initialize()
        {
            renderer.Initialize();
            load_screen.Initialize();
        }
        void MainLoop()
        {
            while (true)
            {
                deltaTime += 0.008f;
                renderer.Draw();
                renderer.Update(deltaTime);
                simpleOpenGlControl1.Refresh();

                load_screen.Draw();

            }
        }
        void playwalking()
        {

        }
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer.CleanUp();
            load_screen.CleanUp();
            MainLoopThread.Abort();
            
            StreamWriter wt = new StreamWriter("Controls.txt");
            foreach (var item in controls)
            {
                wt.WriteLine(item.Key + " " + item.Value);
            }
            wt.Close();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            renderer.Draw();
            renderer.Update(deltaTime);
        }

        private void simpleOpenGlControl2_Paint(object sender, PaintEventArgs e)
        {
            load_screen.Draw();

        }

        bool moving;
        bool walking = false;
        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool found = false;
            // this condition will be check if the keyChar in the dictionary of controls
            /* foreach (var i in controls)
             {
                 if (string.Compare(e.KeyChar.ToString(), i.Value.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                 {
                     found = true;
                     break;
                 }

             }
             if (!found)
                 return;*/

           /* if (!(renderer.cam.mPosition.to_array()[2] < new vec3(0, 0, 1900)[2] && renderer.cam.mPosition.to_array()[2] > new vec3(0, 0, -1900)[2] && renderer.pos[0] < 1950 && (renderer.pos[0] > -1950)))
            {
                //renderer.cam.Walk(5);
                return;
            }*/

           // MessageBox.Show(renderer.BackwardSide[0].ToString() + " "+ renderer.cam.mPosition.x.ToString());
         /*   if (renderer.BackwardSide.x == renderer.cam.mPosition.x && renderer.BackwardSide.y == renderer.cam.mPosition.y && renderer.BackwardSide.z == renderer.cam.mPosition.z)
                return;*/
                if (PlayerCollisionDetection(e.KeyChar))
            {
                if(Renderer.GameSoundIsOpened)
                {
                    walkingSound.Stop();
                    playerCollisionSound.Play();
                }
                
                return;
            }

            if (Renderer.GameSoundIsOpened)
            {
                if (!walking)
                    walkingSound.PlayLooping();
            }
               
            float speed = 5f;
            renderer.pos = renderer.cam.GetCameraPosition();
            if (string.Compare(e.KeyChar.ToString() , controls["Left"].ToString(),StringComparison.OrdinalIgnoreCase) == 0 && renderer.cam.mPosition.to_array()[2] > new vec3(0, 0, -1850)[2] && (renderer.pos[0] < 1850))
            {

                renderer.cam.Strafe(-speed);

                //WPressed = false; SPressed = false; DPressed = false;
            }
            else
                renderer.cam.Strafe(2 * speed);


            if (string.Compare( e.KeyChar.ToString(),controls["Right"].ToString(),StringComparison.OrdinalIgnoreCase) == 0 && renderer.cam.mPosition.to_array()[2] < new vec3(0, 0, 1550)[2] && (renderer.pos[0] > -1850))
            {

                renderer.cam.Strafe(speed);
                //WPressed = false; SPressed = false; APressed = false;
            }
            else
                renderer.cam.Strafe(-2 * speed);

            if (string.Compare(e.KeyChar.ToString() ,controls["Backward"].ToString(),StringComparison.OrdinalIgnoreCase)  == 0)
            {
                if (renderer.cam.mPosition.to_array()[2] > new vec3(0, 0, -1900)[2] && renderer.cam.mPosition.to_array()[2] < new vec3(0, 0, 1900)[2] && renderer.pos[0] > -1500 && (renderer.pos[0] < 1500))
                {
                    renderer.cam.Walk(-speed);

                }
                /*  else 
                  {
                     // MessageBox.Show(renderer.cam.mPosition.to_array()[2].ToString());
                      renderer.cam.Walk(2 * speed);
                  }*/
            }
            if (string.Compare(e.KeyChar.ToString(), controls["Forward"].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (renderer.cam.mPosition.to_array()[2] < new vec3(0, 0, 1900)[2] && renderer.cam.mPosition.to_array()[2] > new vec3(0, 0, -1900)[2] && renderer.pos[0] < 1950 && (renderer.pos[0] > -1950))
                {
                    renderer.cam.Walk(speed);
                    //APressed = false; SPressed = false; DPressed = false;
                }
                /* else 
                 {

                     renderer.cam.Walk(-2 * speed);
                 }*/
            }


            if (string.Compare(e.KeyChar.ToString(), controls["Switch"].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (mouse_move == true)
                {
                    mouse_move = false;
                }

                else
                {
                    mouse_move = true;
                }
            }
            if (e.KeyChar == 'c')
            {
             //   MessageBox.Show("about = " + renderer.about.ToString() + "   option = " + renderer.option_screen.ToString() + "   load = " + renderer.load.ToString());

               if (renderer.about == true)
                { 
                    /// about ==> option
                    renderer.option_screen = false;
                }
               else
                {
                    /// play ==> option (khrog)
                    renderer.set_health_wolf();
                    renderer.option_screen = false;
                    simpleButton1.Visible = false;
                    simpleButton2.Visible = false;
                    simpleButton3.Visible = false;
                    simpleButton4.Visible = false;
                    simpleButton5.Visible = false;
                    simpleButton6.Visible = false;
                    
                }
                renderer.about = false;
                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = false;
                
            }
            label6.Text = "X: " + renderer.cam.GetCameraPosition().x;
            label7.Text = "Y: " + renderer.cam.GetCameraPosition().y;
            label8.Text = "Z: " + renderer.cam.GetCameraPosition().z;

            //label7.Text = "X: " + renderer.cam.mAngleX;
            //label8.Text = "Y: " + renderer.cam.mAngleY;
            walking = true;
        }

        float prevX, prevY;
        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouse_move == true && renderer.load == true && renderer.option_screen == true)
            {
                float speed = 0.05f;
                float delta = e.X - prevX;
                if (delta > 2)
                    renderer.cam.Yaw(-speed);
                else if (delta < -2)
                    renderer.cam.Yaw(speed);


                delta = e.Y - prevY;
                if (delta > 2)
                    renderer.cam.Pitch(-speed);
                else if (delta < -2)
                    renderer.cam.Pitch(speed);

                MoveCursor();
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            //float r = float.Parse(textBox1.Text);
            //float g = float.Parse(textBox2.Text);
            //float b = float.Parse(textBox3.Text);
            //float a = float.Parse(textBox4.Text);
            //float s = float.Parse(textBox5.Text);
            //renderer.SendLightData(r, g, b, a, s);
        }

        private void simpleOpenGlControl1_KeyUp(object sender, KeyEventArgs e)
        {
            walkingSound.Stop();
            walking = false;
        }
        int XMax = 1064, XMin = 577;
        private void simpleOpenGlControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (renderer.load == true && renderer.option_screen == false)
            {
                // MessageBox.Show("x = " + e.X + "  " + "y = " + e.Y);
                if (e.X > XMin && e.X < XMax)
                    if (e.Y > 114 && e.Y < 114 + 120)
                    {

                        //Play Button 
                        renderer.Change_option_screen(true);
                        simpleButton1.Visible = true;
                        simpleButton2.Visible = true;
                        simpleButton3.Visible = true;
                        simpleButton4.Visible = true;

                        simpleButton5.Visible = true;
                        simpleButton6.Visible = true;
                       
                        renderer.option_screen = true;
                        mouse_move = true;
                    }
                    else if (e.Y > 639 && e.Y < 639 + 120)
                    {
                        Application.Exit();
                    }
                    else if (e.Y > 464 && e.Y < 646 + 120)
                    {
                        //about
                        renderer.Change_option_screen(true);
                        renderer.change_about(true);


                    }
                    else if (e.Y > 290 && e.Y < 290 + 120)
                    {
                        //scores
                        //MessageBox.Show("Scores");
                        Settings set = new Settings();
                        set.ShowDialog();

                    }

            }
            else if (renderer.load == true && renderer.option_screen == true)
            {
                if(Renderer.GameSoundIsOpened)
                {
                    gunShoot.Stop();
                    gunShoot.Play();
                }
                
                renderer.draw = true;
                renderer.cam.mAngleY += 0.1f;
                renderer.shootes();

            }

        }

        private void simpleOpenGlControl1_Load(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void GraphicsForm_Load(object sender, EventArgs e)
        {
            simpleButton1.Visible = false;
            simpleButton2.Visible = false;
            simpleButton3.Visible = false;
            simpleButton4.Visible = false;
            
            //Read Controls and store them 
            StreamReader sr = new StreamReader("Controls.txt");
            string ControlsFile = sr.ReadToEnd();
            string[] keys = ControlsFile.Split('\n');
            for (int i = 0; i < keys.Length - 1; i++) 
            {
                string[] line = keys[i].Split(' ');
                controls.Add(line[0], line[1][0]);
            }
            simpleButton1.Location = new Point(1900, 200);
        }

        private void MoveCursor()
        {
            if (mouse_move == true && renderer.load == true && renderer.option_screen == true)
            {
                this.Cursor = Cursors.Cross;

                // this.Cursor = new Cursor(Cursor.Current.Handle);
                Point p = PointToScreen(simpleOpenGlControl1.Location);
                Cursor.Position = new Point(simpleOpenGlControl1.Size.Width / 2 + p.X, simpleOpenGlControl1.Size.Height / 2 + p.Y);
                Cursor.Clip = new Rectangle(this.Location, this.Size);
                prevX = simpleOpenGlControl1.Location.X + simpleOpenGlControl1.Size.Width / 2;
                prevY = simpleOpenGlControl1.Location.Y + simpleOpenGlControl1.Size.Height / 2;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            /*simpleOpenGlControl1.Visible = true;
            button1.Visible = false;
            simpleOpenGlControl1.Visible = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
           // MessageBox.Show(this.Size.ToString());
            TopMost = true;*/
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            move_up();
        }

        private void move_up()
        {
            float speed = 5f;
            if (renderer.cam.mPosition.to_array()[2] < new vec3(0, 0, 1900)[2] && renderer.cam.mPosition.to_array()[2] > new vec3(0, 0, -1900)[2] && renderer.pos[0] < 1950 && (renderer.pos[0] > -1950))
            {
                renderer.cam.Walk(speed);
                //APressed = false; SPressed = false; DPressed = false;
            }

        }


        private void GraphicsForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {

            if (renderer.cam.mPosition.to_array()[2] > new vec3(0, 0, -1950)[2] && (renderer.pos[0] > -1950))
            {

                renderer.cam.Strafe(5);
                //WPressed = false; SPressed = false; APressed = false;
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            if (renderer.cam.mPosition.to_array()[2] > new vec3(0, 0, -1900)[2] && renderer.cam.mPosition.to_array()[2] < new vec3(0, 0, 1900)[2] && renderer.pos[0] > -1500 && (renderer.pos[0] < 1500))
            {
                renderer.cam.Walk(-5);

            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (renderer.cam.mPosition.to_array()[2] < new vec3(0, 0, 1950)[2] && (renderer.pos[0] < 1950))
            {

                renderer.cam.Strafe(-5);
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            renderer.set_health_wolf();
            //renderer.cam.set_cam_pos();
            simpleButton6.Visible = false;
            simpleButton5.Visible = false;
            simpleButton1.Visible = false;
            simpleButton2.Visible = false;
            simpleButton3.Visible = false;
            simpleButton4.Visible = false;

        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            renderer.option_screen = false;
            renderer.set_health_wolf();
           // renderer.cam.set_cam_pos();
            simpleButton6.Visible = false;
            simpleButton5.Visible =false;
            simpleButton1.Visible = false;
            simpleButton2.Visible = false;
            simpleButton3.Visible = false;
            simpleButton4.Visible = false;
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            renderer.Change_option_screen(true);
            simpleButton1.Visible = false;
            simpleButton2.Visible = false;
            simpleButton3.Visible = false;
            simpleButton4.Visible = false;

        }
        public void set_buttons ()
        {
            renderer.option_screen = false;
            renderer.set_health_wolf();
            // renderer.cam.set_cam_pos();
            simpleButton6.Visible = false;
            simpleButton5.Visible = false;
            simpleButton1.Visible = false;
            simpleButton2.Visible = false;
            simpleButton3.Visible = false;
            simpleButton4.Visible = false;

        }
        bool PlayerCollisionDetection(char KeyPressed)
        {

          
                if (string.Compare(KeyPressed.ToString(), controls["Forward"].ToString(), StringComparison.OrdinalIgnoreCase) == 0)

                {
                    // check collision with trees
                    for (int i = 0; i < renderer.trees.Count; ++i)
                {
                    float DistanceBetweenPlayerAndTree = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.trees[i].current_pos.x, renderer.trees[i].current_pos.y, renderer.trees[i].current_pos.z);

                    if (DistanceBetweenPlayerAndTree < 60 && glm.dot(renderer.cam.mDirection, (renderer.trees[i].current_pos - renderer.playerPos)) > 0)
                    {
                        return true;
                    }
                }

                float DistanceBetweenPlayerAndHouse = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.house.current_pos.x, renderer.house.current_pos.y, renderer.house.current_pos.z);
                if (DistanceBetweenPlayerAndHouse < 350 && glm.dot(renderer.cam.mDirection, (renderer.house.current_pos - renderer.playerPos)) > 0)
                {
                    return true;
                }

                DistanceBetweenPlayerAndHouse = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.house2.current_pos.x, renderer.house2.current_pos.y, renderer.house2.current_pos.z);
                if (DistanceBetweenPlayerAndHouse < 350 && glm.dot(renderer.cam.mDirection, (renderer.house2.current_pos - renderer.playerPos)) > 0)
                    return true;
                //check collision
                for (int i = 0; i < renderer.wolfs.Count; ++i)
                {
                    float Distance_between_Enemy_And_Playr = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.wolfs[i].current_position.x, renderer.wolfs[i].current_position.y, renderer.wolfs[i].current_position.z);

                    if (Distance_between_Enemy_And_Playr < 150 && glm.dot(renderer.cam.mDirection, (renderer.wolfs[i].current_position - renderer.playerPos)) > 0)
                    {
                        return true;
                    }

                }
            }
            if (string.Compare(KeyPressed.ToString(), controls["Backward"].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
            {
                // check collision with trees
                for (int i = 0; i < renderer.trees.Count; ++i)
                {

                    float DistanceBetweenPlayerAndTree = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.trees[i].current_pos.x, renderer.trees[i].current_pos.y, renderer.trees[i].current_pos.z);

                    if (DistanceBetweenPlayerAndTree < 60 && glm.dot(renderer.cam.mDirection, (renderer.playerPos - renderer.trees[i].current_pos)) > 0)
                    {

                        return true;

                    }

                }

                float DistanceBetweenPlayerAndHouse = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.house.current_pos.x, renderer.house.current_pos.y, renderer.house.current_pos.z);
                if (DistanceBetweenPlayerAndHouse < 350 && glm.dot(renderer.cam.mDirection, (renderer.playerPos - renderer.house.current_pos)) > 0)
                {
                    return true;
                }

                DistanceBetweenPlayerAndHouse = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.house2.current_pos.x, renderer.house2.current_pos.y, renderer.house2.current_pos.z);
                if (DistanceBetweenPlayerAndHouse < 350 && glm.dot(renderer.cam.mDirection, (renderer.playerPos - renderer.house2.current_pos)) > 0)
                    return true;
                //check collision
                for (int i = 0; i < renderer.wolfs.Count; ++i)
                {
                    float Distance_between_Enemy_And_Playr = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.wolfs[i].current_position.x, renderer.wolfs[i].current_position.y, renderer.wolfs[i].current_position.z);

                    if (Distance_between_Enemy_And_Playr < 150 && glm.dot(renderer.cam.mDirection, (renderer.playerPos - renderer.wolfs[i].current_position)) > 0)
                    {
                        return true;
                    }

                }
            }
            if (string.Compare(KeyPressed.ToString(), controls["Right"].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
            {
                // check collision with trees
                for (int i = 0; i < renderer.trees.Count; ++i)
                {
                    float DistanceBetweenPlayerAndTree = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.trees[i].current_pos.x, renderer.trees[i].current_pos.y, renderer.trees[i].current_pos.z);

                    if (DistanceBetweenPlayerAndTree < 60 && glm.dot(renderer.cam.mRight, (renderer.trees[i].current_pos - renderer.playerPos)) > 0)
                    {
                        return true;
                    }
                }

                float DistanceBetweenPlayerAndHouse = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.house.current_pos.x, renderer.house.current_pos.y, renderer.house.current_pos.z);
                if (DistanceBetweenPlayerAndHouse < 350 && glm.dot(renderer.cam.mRight, (renderer.house.current_pos - renderer.playerPos)) > 0)
                {
                    return true;
                }

                DistanceBetweenPlayerAndHouse = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.house2.current_pos.x, renderer.house2.current_pos.y, renderer.house2.current_pos.z);
                if (DistanceBetweenPlayerAndHouse < 350 && glm.dot(renderer.cam.mRight, (renderer.house2.current_pos - renderer.playerPos)) > 0)
                    return true;
                //check collision
                for (int i = 0; i < renderer.wolfs.Count; ++i)
                {
                    float Distance_between_Enemy_And_Playr = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.wolfs[i].current_position.x, renderer.wolfs[i].current_position.y, renderer.wolfs[i].current_position.z);

                    if (Distance_between_Enemy_And_Playr < 150 && glm.dot(renderer.cam.mRight, (renderer.wolfs[i].current_position - renderer.playerPos)) > 0)
                    {
                        return true;
                    }

                }
            }
            if (string.Compare(KeyPressed.ToString(), controls["Left"].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
            {
                // check collision with trees
                for (int i = 0; i < renderer.trees.Count; ++i)
                {

                    float DistanceBetweenPlayerAndTree = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.trees[i].current_pos.x, renderer.trees[i].current_pos.y, renderer.trees[i].current_pos.z);

                    if (DistanceBetweenPlayerAndTree < 60 && glm.dot(renderer.cam.mRight, (renderer.playerPos - renderer.trees[i].current_pos)) > 0)
                    {

                        return true;

                    }

                }

                float DistanceBetweenPlayerAndHouse = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.house.current_pos.x, renderer.house.current_pos.y, renderer.house.current_pos.z);
                if (DistanceBetweenPlayerAndHouse < 350 && glm.dot(renderer.cam.mRight, (renderer.playerPos - renderer.house.current_pos)) > 0)
                {
                    return true;
                }

                DistanceBetweenPlayerAndHouse = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.house2.current_pos.x, renderer.house2.current_pos.y, renderer.house2.current_pos.z);
                if (DistanceBetweenPlayerAndHouse < 350 && glm.dot(renderer.cam.mRight, (renderer.playerPos - renderer.house2.current_pos)) > 0)
                    return true;
                //check collision
                for (int i = 0; i < renderer.wolfs.Count; ++i)
                {
                    float Distance_between_Enemy_And_Playr = MathHelper.Get_Distance(renderer.playerPos.x, renderer.playerPos.y, renderer.playerPos.z, renderer.wolfs[i].current_position.x, renderer.wolfs[i].current_position.y, renderer.wolfs[i].current_position.z);

                    if (Distance_between_Enemy_And_Playr < 150 && glm.dot(renderer.cam.mRight, (renderer.playerPos - renderer.wolfs[i].current_position)) > 0)
                    {
                        return true;
                    }

                }
            }
            return false;
        }
    }
}
