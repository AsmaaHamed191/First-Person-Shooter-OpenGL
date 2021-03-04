using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Graphics._3D_Models;
using System.Media;

namespace Graphics
{
    class Enemy
    {
        public md2LOL model;
        public vec3 current_position;
        vec3 current_scale;
       public int health;
        bool attack_animation, stand_animation, run_animation, death;
        public bool isDead;
        float AngleX, AngleY;
        SoundPlayer attackSound = new SoundPlayer("attack.wav");
        SoundPlayer DeathSound = new SoundPlayer("death.wav");
        public bool ShouldDisapear = false;
        public TimeSpan DeadAt;

        //###############################################################//
        /////////// Health bar 
        Texture hp;
        Texture bhp;
        uint hpID;
        mat4 healthbar;
        mat4 backhealthbar;
        Shader shader2D;
        int mloc;
        float scalef;

        public Enemy(string path)
        {

            health = 5;
            AngleX = AngleY = 0;
            attack_animation = false;
            stand_animation = true; run_animation = false;
            this.model = new md2LOL(path);
            isDead = false;
        }
        public void health_intialization (string projectPath  , Shader shader2D)
        {
            //############################################################################### (Health)
            //2D control
            hp = new Texture(projectPath + "\\Textures\\HP.bmp", 9);
            bhp = new Texture(projectPath + "\\Textures\\BackHP.bmp", 10);
            float[] squarevertices = {
                -100,100,-100f,
                0,0,

                100,-100,-100f,
                1,1,

                -100,-100,-100f,
                0,1,

                100,100,-100f,
                1,0,

                -100,100,-100f,
                0,0,

                100,-100,-100f,
                1,1
            };

            hpID = GPU.GenerateBuffer(squarevertices);

          
            backhealthbar = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.scale(new mat4(1), new vec3(1,1,1)), glm.translate(new mat4(1),new vec3(current_position.x,current_position.y,current_position.z)) });

            healthbar = MathHelper.MultiplyMatrices(new List<mat4>() {
                glm.scale(new mat4(1), new vec3(1,1,1)), glm.translate(new mat4(1), new vec3(0,0,0)) });

            shader2D.UseShader();
            mloc = Gl.glGetUniformLocation(shader2D.ID, "model");
            scalef = 1;


            //enabling Depth test
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);

        }
        
        public void Draw_Health (Shader shader2D)
        {

            //Gl.glDisable(Gl.GL_DEPTH_TEST);
            //use 2D shader
            shader2D.UseShader();
            //draw 2D square 
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);

            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            //background of healthbar
            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, backhealthbar.to_array());
            //bhp.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
            //decrease the health bar by scaling down the 2D front square 
            //scalef -= 0.0001f;
            //if (scalef < 0)
            //    scalef = 0;
            healthbar = MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1), new vec3(0.5f * 1, 0.1f, 1)), glm.translate(new mat4(1), new vec3(-0.5f - ((1 - 1) * 0.48f), 0.9f, 0)) });
            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
            hp.Bind();
            //draw front health bar 
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
            //re-enable the depthtest to draw the other 3D objects in the scene 
           // Gl.glEnable(Gl.GL_DEPTH_TEST);

            ////Gl.glDisableVertexAttribArray(0);
            ////Gl.glDisableVertexAttribArray(1);

            ////Gl.glDisableVertexAttribArray(0);
            ////Gl.glDisableVertexAttribArray(1);


        }
        public void translate(vec3 pos)
        {
            // Translation matrix and save current position of enemy 
            this.model.TranslationMatrix = glm.translate(new mat4(1), pos);
            current_position = pos;

        }
        public void scale(vec3 scale)
        {
            this.model.scaleMatrix = glm.scale(new mat4(1), scale);
            this.current_scale = scale;
        }
        public void DecreaseHealth()
        {
            //  MessageBox.Show(health.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            health--;
            if (health == 0)
                isDead = true;

        }
        public void Mov(vec3 direction)
        {

            //  model.UpdateExportedAnimation(); 
            if (!run_animation)
            {
                attackSound.Stop();
                model.StartAnimation(animType_LOL.RUN);

                // current animation is run , and close stand animation 
                run_animation = true;
                stand_animation = false;
                death = false;
            }
            current_position += direction * 0.005f;

            this.model.TranslationMatrix = glm.translate(new mat4(1), current_position);
            rotate_face(direction);

            // this.model.scaleMatrix = glm.scale(new mat4(1), current_scale);



        }
        public void Death(TimeSpan DeadAt)
        {
            if (!death)
            {
                if(Renderer.GameSoundIsOpened)
                {
                    attackSound.Stop();
                    DeathSound.Play();
                }
                
                model.StartAnimation(animType_LOL.DEATH);
                death = true;
                stand_animation = false;
                run_animation = false;
                attack_animation = false;
                this.DeadAt = TimeSpan.FromSeconds(DeadAt.Seconds + 1);
                // MessageBox.Show(this.DeadAt.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        public void rotate_face(vec3 direction)
        {
            direction = glm.normalize(direction);

            float look_at_x = 0;
            float look_at_z = 1;

            float dot_product = direction.x * look_at_x + direction.z * look_at_z;
            float cross_product = direction.x * look_at_z - direction.z * look_at_x;

            float Angle = (float)Math.Atan2(cross_product, dot_product);

            model.rotationMatrix = glm.rotate((float)(-Angle), new vec3(0, 0, 1)) *
                glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));

        }
        public void Attack()
        {
            // condition to handle that start animation excute one times untill change this animation 
            if (!attack_animation)
            {
                if (Renderer.GameSoundIsOpened)
                    attackSound.PlayLooping();

                model.StartAnimation(animType_LOL.ATTACK1);
                attack_animation = true;
                run_animation = false;
                stand_animation = false;
            }

            // decrease player health  ... for example for each 5 times attack excute decrease player health by one 

        }
        public void set_defualt_animation()
        {
            if (!stand_animation)
            {
                model.StartAnimation(animType_LOL.STAND);
                stand_animation = true;
                attack_animation = false;
                run_animation = false;
                death = false;
            }

        }





    }
}
