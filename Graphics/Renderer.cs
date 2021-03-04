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
using System.Media;
using Graphics._3D_Models;
namespace Graphics
{
    class Renderer
    {
        public static bool SystemSoundIsOpened = true;
        public static bool GameSoundIsOpened = true;
        public vec3 leftSide ,RightSide,ForwardSide,BackwardSide;
        //old variables
        Shader sh;
        float t = 0;
        bool LightTime = true;
        int modelID;
        int transID;
        int viewID;
        int projID;
        int EyePositionID;
        int AmbientLightID;
        int DataID;
        int numOfBullets;
        int playerHealth;
        float DetectionRange, AttackRange, Distance_between_Enemy_And_Playr;
        vec3 ambientLight;
        mat4 ProjectionMatrix;
        mat4 ViewMatrix;
        mat4 down;
        mat4 left;
        mat4 right;
        mat4 up;
        mat4 front;
        mat4 back;
        mat4 bb;

        mat4 health1;
        mat4 health2;
        //###############################################################//
        /////////// Health bar 
        Texture hp;
        Texture bhp;
        uint hpID;
        mat4 healthbar;
        mat4 healthbar2;
        mat4 backhealthbar;
        Shader shader2D;
        int mloc;
        float scalef;
        public Camera cam;
        float scalf2;
        //########################### Option Screen ###################################//
        Texture background;
        Texture play_button_texture;
        Texture about_button;
        Texture option_button;
        Texture exit_button;
        uint background_id;
        public bool option_screen = false;
        //########################### Option Screen ###################################//
        Texture tex_up;
        Texture tex_down;
        Texture tex_left;
        Texture tex_right;
        Texture tex_front;
        Texture tex_back;
        //###########################load screen ###################################//
        Texture load_screen;
        public bool load = false;
        //########################### about Screen ###################################//
        Texture background_about;
        public bool about = false;
        // static models 
        Texture hp2;

        public List<StaticModel> trees;
        public StaticModel Tree_temp;

        public StaticModel house, house2;

        List<StaticModel> eagles;
        StaticModel eagle_temp;
        //////// list of bullets 

        int num_of_bullets;
        bullet bull_temp;

        List<bullet> bullets;
        //   public md2LOL wolf3, wolf4, wolf5;

        Enemy wolf, wolf2, wolf3, wolf4, wolf5;
        public List<Enemy> wolfs;
        public List<Enemy> wolfs2;


        mat4 scaleMat;
        public vec3 pos;
        uint vertexBufferID;


        public vec3 playerPos;
        Model3D m;
        Model3D city;
        uint ShootID;
        Texture shoot;
        Texture stone_texture;
        StaticModel stone;
        vec3 lightPosition;
        vec3 DayLight, NightLight;

        SoundPlayer startScreen = new SoundPlayer("startScreenMusic.wav");
        SoundPlayer PlayerDeath = new SoundPlayer("man death.wav");

        public void Change_option_screen(bool option)
        {
            option_screen = option;
        }
        public void change_about(bool about)
        {
            this.about = about;
        }
        public void Initialize()
        {
            
            wolfs = new List<Enemy>();
            trees = new List<StaticModel>();
            eagles = new List<StaticModel>();
            bullets = new List<bullet>();

            DayLight = new vec3(0.2f, 0.18f, 0.18f);
            NightLight = new vec3(0.08f, 0, 0.24f);

            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            shoot = new Texture(projectPath + "\\Textures\\gunshot.png", 5);

            tex_up = new Texture(projectPath + "\\Textures\\up2.jpg", 1, false);
            tex_down = new Texture(projectPath + "\\Textures\\ground.jpg", 2, false);
            tex_left = new Texture(projectPath + "\\Textures\\arrakisday_lf.jpg", 3, false);

            tex_right = new Texture(projectPath + "\\Textures\\right3.jpg", 4, false);
            tex_front = new Texture(projectPath + "\\Textures\\front2.jpg", 5, false);
            tex_back = new Texture(projectPath + "\\Textures\\back.jpg", 6, false);


            float[] shootvertices = {
                -1,1,0,
                1,0,0,
                0,0,
                0,1,0,
                1,-1,0,
                0,0,1,
                1,1,
                0,1,0,
                -1,-1,0,
                0,0,1,
                0,1,
                0,1,0,
                1,1,0,
                0,0,1,
                1,0,
                0,1,0,
                -1,1,0,
                0,1,0,
                0,0,
                0,1,0,
                1,-1,0,
                1,0,0,
                1,1,
                0,1,0
            };
            ShootID = GPU.GenerateBuffer(shootvertices);

            ////texture 



            Gl.glClearColor(0, 0, 0.4f, 1);

            //camera
            cam = new Camera();
            // eyeX, eyeY,eyeZ, centerX,centerY, centerZ,  upX, fupY, upZ)
            cam.Reset(0, 0, 0, -15, 55, -1217, 0, 1, 0);
            cam.mAngleX = 2.8f;
            // Projection  And ViewMatrix from Class Camera  
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();



            // M >> Gun = player Model 
            m = new Model3D();
            m.LoadFile(projectPath + "\\ModelFiles\\hands with gun", 2, "gun.obj");
            m.scalematrix = glm.scale(new mat4(1), new vec3(0.2f, 0.2f, 0.2f));
            m.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            m.transmatrix = glm.translate(new mat4(1), playerPos);

            playerPos = cam.GetCameraTarget();
            playerPos.y += 7;

            defaultY = cam.GetCameraTarget().y;


            city = new Model3D();
            city.LoadFile(projectPath + "\\ModelFiles\\city", 3, "The City.obj");




            //shader configurations

            transID = Gl.glGetUniformLocation(sh.ID, "trans");

            projID = Gl.glGetUniformLocation(sh.ID, "projection");

            viewID = Gl.glGetUniformLocation(sh.ID, "view");

            sh.UseShader();

            DataID = Gl.glGetUniformLocation(sh.ID, "data");

            vec2 data = new vec2(40000, 50);
            Gl.glUniform2fv(DataID, 1, data.to_array());

            int LightPositionID = Gl.glGetUniformLocation(sh.ID, "LightPosition_worldspace");
            lightPosition = new vec3(1.0f, 1000f, 4.0f);
            Gl.glUniform3fv(LightPositionID, 1, lightPosition.to_array());
            AmbientLightID = Gl.glGetUniformLocation(sh.ID, "ambientLight");
            ambientLight = new vec3(0.2f, 0.18f, 0.18f);
            Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());

            EyePositionID = Gl.glGetUniformLocation(sh.ID, "EyePosition_worldspace");

            //enabling Depth test
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
            c = timer;


            float[] verts = {
                 -1,0,1,  1,0,0, 0,0,

                1,0,1,   1,0,0,1,0,
                -1,0,-1, 1,0,0,0,1,

                1,0,1,   1,0,0,1,0,
                -1,0,-1, 1,0,0,0,1,
                1,0,-1 , 1,0,0,1,1
            };


            vertexBufferID = GPU.GenerateBuffer(verts);

            modelID = Gl.glGetUniformLocation(sh.ID, "model");
            scaleMat = glm.scale(new mat4(1), new vec3(2000, 2000, 2000));


            ViewMatrix = glm.lookAt(new vec3(0.5f, 0.5f, 0.5f), new vec3(0, 0, 0), new vec3(0, 1, 0));
            ProjectionMatrix = glm.perspective(45, 760.0f / 440.0f, 0.1f, 100.0f);

            //############################################################################
            // TO draw sky box >> ALl faces >> each face contain same ViewMatrix and ProjectionMatrix 
            // but each face contain different Transformation Matrix 

            down = MathHelper.MultiplyMatrices(new List<mat4>() {
               glm.translate(new mat4(1),new vec3(0,0,0)),
                 scaleMat
                   });
            leftSide = new vec3(0, 1, 1);
            left = MathHelper.MultiplyMatrices(new List<mat4>() {

               glm.rotate((-90.0f / 180 * 3.1412f), new vec3(1, 0, 0)),
               glm.translate(new mat4(1),new vec3(0,1,1)),
                 scaleMat
                   });
            RightSide = new vec3(0, 1, -1);
            right = MathHelper.MultiplyMatrices(new List<mat4>() {

                   glm.rotate((90.0f / 180 * 3.1412f), new vec3(1, 0, 0)),
               glm.translate(new mat4(1),new vec3(0,1,-1)),
                 scaleMat
                   });
            BackwardSide = new vec3(-1, 1, 0);
            back = MathHelper.MultiplyMatrices(new List<mat4>() {

                   glm.rotate((-90.0f / 180 * 3.1412f), new vec3(0, 0, 1)),
               glm.translate(new mat4(1),new vec3(-1,1,0)),
                 scaleMat
                   });

            ForwardSide = new vec3(1, 1, 0);
            front = MathHelper.MultiplyMatrices(new List<mat4>() {

                   glm.rotate((90.0f / 180 * 3.1412f), new vec3(0, 0, 1)),

               glm.translate(new mat4(1),new vec3(1,1,0)),
                scaleMat
                   });

            up = MathHelper.MultiplyMatrices(new List<mat4>() {

                  // glm.rotate((90.0f / 180 * 3.1412f), new vec3(0, 1, 0)),
               glm.translate(new mat4(1),new vec3(0,2,0)),
                 scaleMat
                   });



            //////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////// 
            // ********** END OF SKY BOX FACES 
            // ######################################################33


            // Initial size of bulets = 5 
            /* for (int i = 0; i < 5; i++ )
               bullets.Add(new bullet(transID, cam.GetLookDirection(),cam.GetCameraPosition()));*/
            //############################################################################### (Health)
            shader2D = new Shader(projectPath + "\\Shaders\\2Dvertex.vertexshader", projectPath + "\\Shaders\\2Dfrag.fragmentshader");

            //2D control
            hp2 = new Texture(projectPath + "\\Textures\\blue-shapes.png", 9);
            hp = new Texture(projectPath + "\\Textures\\HP.bmp", 9);
            bhp = new Texture(projectPath + "\\Textures\\BackHP.bmp", 10);
            float[] squarevertices = {
                -1,1,0,
                0,0,

                1,-1,0,
                1,1,

                -1,-1,0,
                0,1,

                1,1,0,
                1,0,

                -1,1,0,
                0,0,

                1,-1,0,
                1,1
            };

            hpID = GPU.GenerateBuffer(squarevertices);

            backhealthbar = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.scale(new mat4(1), new vec3(0.5f,0.051f, 1)), glm.translate(new mat4(1),new vec3(-0.5f,0.9f,0)) });

            healthbar2 = MathHelper.MultiplyMatrices(new List<mat4>() {
                glm.scale(new mat4(1), new vec3(0.19f, 0.051f, 1)), glm.translate(new mat4(1), new vec3(0.5f, 0.9f, 0)) });

            shader2D.UseShader();
            mloc = Gl.glGetUniformLocation(shader2D.ID, "model");
            scalef = 1;
            scalf2 = 1;
            //--------------------------------------------

            //enabling Depth test
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);


            // ################################ Animated Models ...... 
            wolfs2 = new List<Enemy>();
            wolf = new Enemy(projectPath + "\\ModelFiles\\animated\\md2LOL\\wolf.md2");
            // wolf.model = new md2LOL(projectPath + "\\ModelFiles\\animated\\md2LOL\\wolf.md2");
            wolf.model.StartAnimation(animType_LOL.STAND);

            wolf.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)) *
            glm.rotate((float)((-180.0f / 180) * Math.PI), new vec3(0, 0, 1));

            wolf.scale(new vec3(2.2f, 2.2f, 2.2f));
            wolf.translate(new vec3(300, 10, 0));
            wolf.health_intialization(projectPath, shader2D);
            wolfs.Add(wolf);
            wolfs2.Add(wolf);

            //___________________________________________________________________ */
            wolf2 = new Enemy(projectPath + "\\ModelFiles\\animated\\md2LOL\\wolf.md2");

            wolf2.model.StartAnimation(animType_LOL.STAND);
            wolf2.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)) * glm.rotate((float)((-180.0f / 180) * Math.PI), new vec3(0, 0, 1));
            wolf2.scale(new vec3(2.2f, 2.2f, 2.2f));

            wolf2.translate(new vec3(400, 10, 0));
            wolf2.health_intialization(projectPath, shader2D);
            wolfs.Add(wolf2);
            wolfs2.Add(wolf2);

            // ____________________________________________________________________
            wolf3 = new Enemy(projectPath + "\\ModelFiles\\animated\\md2LOL\\wolf.md2");

            wolf3.model.StartAnimation(animType_LOL.STAND);
            wolf3.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)) * glm.rotate((float)((-180.0f / 180) * Math.PI), new vec3(0, 0, 1));
            wolf3.scale(new vec3(2.2f, 2.2f, 2.2f));
            wolf3.translate(new vec3(-200, 10, 100));
            //wolf3.health_intialization(projectPath, shader2D);
            wolfs.Add(wolf3);
            wolfs2.Add(wolf3);
            // ______________________________________________________
            wolf4 = new Enemy(projectPath + "\\ModelFiles\\animated\\md2LOL\\wolf.md2");

            wolf4.model.StartAnimation(animType_LOL.STAND);
            wolf4.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            wolf4.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)) * glm.rotate((float)((-180.0f / 180) * Math.PI), new vec3(0, 0, 1));
            wolf4.scale(new vec3(2.2f, 2.2f, 2.2f));
            wolf4.translate(new vec3(-450, 10, -50));
            // wolf4.health_intialization(projectPath, shader2D);
            wolfs.Add(wolf4);
            wolfs2.Add(wolf4);
            // _______________________________________________________
            wolf5 = new Enemy(projectPath + "\\ModelFiles\\animated\\md2LOL\\wolf.md2");
            wolf5.model.StartAnimation(animType_LOL.STAND);
            wolf5.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)) * glm.rotate((float)((-180.0f / 180) * Math.PI), new vec3(0, 0, 1));  //  * glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(0, 1, 0));
            wolf5.scale(new vec3(2.2f, 2.2f, 2.2f));
            wolf5.translate(new vec3(350, 0, -300));//.TranslationMatrix = glm.translate(new mat4(1),);
            wolfs2.Add(wolf5);                        // wolf5.health_intialization(projectPath, shader2D);
            wolfs.Add(wolf5);
            // ##################################################################
            // ***************   Start Static MOdels 
            stone_texture = new Texture(projectPath + "\\ModelFiles\\static\\Rock_3dModel\\Download (1).jpg", 11);

            stone = new StaticModel();
            stone.Model3D_obj.LoadFile(projectPath + "\\ModelFiles\\static\\Rock_3dModel", 15, "sculpt.obj");
            stone.Model3D_obj.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            //stone.Model3D_obj.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            //stone.translate(new vec3(-500, 1, -200));
            // trees.Add(stone);
            ////_____________________________________________________________
            Tree_temp = new StaticModel();
            Tree_temp.Model3D_obj.LoadFile(projectPath + "\\ModelFiles\\static\\tree", 5, "Tree.obj");
            Tree_temp.Model3D_obj.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            Tree_temp.Model3D_obj.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            Tree_temp.translate(new vec3(-500, 1, -800));
            trees.Add(Tree_temp);
            ////______________________________________________________________
            Tree_temp = new StaticModel();

            Tree_temp.Model3D_obj.LoadFile(projectPath + "\\ModelFiles\\static\\tree", 5, "Tree.obj");
            Tree_temp.Model3D_obj.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            Tree_temp.translate(new vec3(-500, 1, 700));
            Tree_temp.Model3D_obj.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            trees.Add(Tree_temp);
            // ________________________________________________________________
            Tree_temp = new StaticModel();

            Tree_temp.Model3D_obj.LoadFile(projectPath + "\\ModelFiles\\static\\tree", 6, "Tree.obj");
            Tree_temp.Model3D_obj.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            Tree_temp.translate(new vec3(700, 1, 800));
            Tree_temp.Model3D_obj.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            trees.Add(Tree_temp);
            // ________________________________________________________________
            Tree_temp = new StaticModel();
            Tree_temp.Model3D_obj.LoadFile(projectPath + "\\ModelFiles\\static\\tree", 7, "Tree.obj");
            Tree_temp.Model3D_obj.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            Tree_temp.translate(new vec3(900, 1, -500));
            Tree_temp.Model3D_obj.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            trees.Add(Tree_temp);
            // ________________________________________________________________________________
            house = new StaticModel();
            house.Model3D_obj.LoadFile(projectPath + "\\ModelFiles\\static\\House", 9, "house.obj");
            house.Model3D_obj.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            house.translate(new vec3(-800, 10, 100));
            house.Model3D_obj.rotmatrix = glm.rotate((float)((90.0f / 180) * Math.PI), new vec3(0, 1, 0));

            // ________________________________________________________________________________________
            house2 = new StaticModel();
            house2.Model3D_obj.LoadFile(projectPath + "\\ModelFiles\\static\\House", 15, "house.obj");
            house2.Model3D_obj.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            house2.translate(new vec3(800, 10, -100));

            house2.Model3D_obj.rotmatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(0, 1, 0));
            // ____________________________________________________________________________________
            eagle_temp = new StaticModel();
            eagle_temp.Model3D_obj.LoadFile(projectPath + "\\ModelFiles\\static\\eagle 2", 10, "EAGLE_2.OBJ");
            eagle_temp.Model3D_obj.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            eagle_temp.translate(new vec3(-100, 500, 500));
            eagle_temp.Model3D_obj.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            eagles.Add(eagle_temp);
            // _________________________________________________________________________________
            eagle_temp = new StaticModel();
            eagle_temp.Model3D_obj.LoadFile(projectPath + "\\ModelFiles\\static\\eagle 2", 11, "EAGLE_2.OBJ");
            eagle_temp.Model3D_obj.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            eagle_temp.translate(new vec3(300, 400, 200));
            eagle_temp.Model3D_obj.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            eagles.Add(eagle_temp);
            // ____________________________________________________________________________________
            eagle_temp = new StaticModel();
            eagle_temp.Model3D_obj.LoadFile(projectPath + "\\ModelFiles\\static\\eagle 2", 12, "EAGLE_2.OBJ");
            eagle_temp.Model3D_obj.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            eagle_temp.translate(new vec3(100, 400, 250));
            eagle_temp.Model3D_obj.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            eagles.Add(eagle_temp);
            // ___________________________________________________________________________________________
            eagle_temp = new StaticModel();

            eagle_temp.Model3D_obj.LoadFile(projectPath + "\\ModelFiles\\static\\eagle 2", 13, "EAGLE_2.OBJ");
            eagle_temp.Model3D_obj.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            eagle_temp.translate(new vec3(500, 600, 50));
            eagle_temp.Model3D_obj.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));

            eagles.Add(eagle_temp);
            // ###############################################################
            // **** For Move movements 
            DetectionRange = 500;
            AttackRange = 200;
            Distance_between_Enemy_And_Playr = 1000; // Initial any number bigger than tow Ranges 
                                                     //########################### Option Screen ###################################//
            float[] screen_option =
            {
                // tringle1
                -1 , 1 , 0,
                 0,1, //u , v
                 1, 1 , 0 ,
                 1, 1 , // u , v 
                 1 , -1 , 0,
                 1 , 0 , // u , v

                 // tringle2 
                 -1 , 1 , 0,
                 0 , 1 ,// u , v
                 -1 , -1 , 0,
                 0 , 0 ,// u , v
                 1 , -1 , 0 ,
                 1 , 0 // u , v


            };
            background = new Texture(projectPath + "\\Textures\\Best-FPS-Games-920x613.jpg", 11);
            play_button_texture = new Texture(projectPath + "\\Textures\\button_play.png", 11);
            about_button = new Texture(projectPath + "\\Textures\\button_about.png", 11);
            option_button = new Texture(projectPath + "\\Textures\\button_scores.png", 12);
            exit_button = new Texture(projectPath + "\\Textures\\button_exit.png", 12);
            load_screen = new Texture(projectPath + "\\Textures\\Screenshot_4.png", 23);
            background_about = new Texture(projectPath + "\\Textures\\Screenshot_6.png", 23);
            background_id = GPU.GenerateBuffer(screen_option);
            shader2D.UseShader();
            mloc = Gl.glGetUniformLocation(shader2D.ID, "model");

        }
        float defaultY;


        // Bullet Area .. 
        public void shootes()
        {
            /*
                        bullets[bullets.Count-1 ].Draw();

                         bullets.Add(new bullet(transID, cam.GetLookDirection(),cam.GetCameraPosition()));
                        bullets[0].Draw();
                         bullets[0].clear(); 
                         bullets.RemoveAt(0); */
            bullets.Add(new bullet(transID, cam.GetLookDirection(), cam.GetCameraPosition()));


        }

        public void draw_bullet()
        {
            // bull.Draw();

            for (int i = 0; i < bullets.Count; i++)
                bullets[i].Draw();
        }
        public void set_health_wolf()
        {
            
            ///health player 
            ///
            scalf2 = 1;
            //position player
            playerPos.y -= 2.5f;
            playerPos.z += 2f;
            m.rotmatrix = glm.rotate(3.1412f + cam.mAngleX, new vec3(0, 1, 0));
            m.transmatrix = glm.translate(new mat4(1), playerPos);

            
            for (int i = 0; i < wolfs.Count; i++)
            {
                wolfs[i].health = 5;

            }

            ///////////////////wolfs
            wolf.model.StartAnimation(animType_LOL.STAND);

            wolf.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)) *
            glm.rotate((float)((-180.0f / 180) * Math.PI), new vec3(0, 0, 1));
            wolf.scale(new vec3(2.2f, 2.2f, 2.2f));
            wolf.translate(new vec3(300, 10, 0));

            //___________________________________________________________________ */
            wolf2.model.StartAnimation(animType_LOL.STAND);
            wolf2.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)) * glm.rotate((float)((-180.0f / 180) * Math.PI), new vec3(0, 0, 1));
            wolf2.scale(new vec3(2.2f, 2.2f, 2.2f));

            wolf2.translate(new vec3(400, 10, 0));

            // ____________________________________________________________________

            wolf3.model.StartAnimation(animType_LOL.STAND);
            wolf3.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)) * glm.rotate((float)((-180.0f / 180) * Math.PI), new vec3(0, 0, 1));
            wolf3.scale(new vec3(2.2f, 2.2f, 2.2f));
            wolf3.translate(new vec3(-200, 10, 100));

            // ______________________________________________________
            wolf4.model.StartAnimation(animType_LOL.STAND);
            wolf4.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            wolf4.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)) * glm.rotate((float)((-180.0f / 180) * Math.PI), new vec3(0, 0, 1));
            wolf4.scale(new vec3(2.2f, 2.2f, 2.2f));
            wolf4.translate(new vec3(-450, 10, -50));

            // _______________________________________________________

            wolf5.model.StartAnimation(animType_LOL.STAND);
            wolf5.model.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)) * glm.rotate((float)((-180.0f / 180) * Math.PI), new vec3(0, 0, 1));  //  * glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(0, 1, 0));
            wolf5.scale(new vec3(2.2f, 2.2f, 2.2f));
            wolf5.translate(new vec3(350, 0, -300));

        }
        //###################################################

        public void updateBullet()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                // update Bullet to go on ...
                bullets[i].Update();

                bool removeBullet = false;
                for (int j = 0; j < trees.Count; j++)
                {
                    float DistanceBetweenBulletAndTree = MathHelper.Get_Distance(bullets[i].position.x, bullets[i].position.y, bullets[i].position.z, trees[j].current_pos.x, trees[j].current_pos.y, trees[j].current_pos.z);
                    // if the bullet collsion with a tree it should not hurt the wold so it will be removed
                    if (DistanceBetweenBulletAndTree < 56)
                    {

                        removeBullet = true;
                        goto next;

                    }
                }
                float DistanceBetweernBulletAndHouse1 = MathHelper.Get_Distance(bullets[i].position.x, bullets[i].position.y, bullets[i].position.z, house.current_pos.x, house.current_pos.y, house.current_pos.z);

                if (DistanceBetweernBulletAndHouse1 < 300)
                {
                    removeBullet = true;
                    goto next;
                }
                float DistanceBetweernBulletAndHouse2 = MathHelper.Get_Distance(bullets[i].position.x, bullets[i].position.y, bullets[i].position.z, house2.current_pos.x, house2.current_pos.y, house2.current_pos.z);

                if (DistanceBetweernBulletAndHouse2 < 300)
                {
                    removeBullet = true;
                    goto next;
                }

                // check if the bullet has a collision with any wolf

                for (int j = 0; j < wolfs.Count; j++)
                {
                    float DistanceBetweernBulletAndWolf = MathHelper.Get_Distance(bullets[i].position.x, bullets[i].position.y, bullets[i].position.z, wolfs[j].current_position.x, wolfs[j].current_position.y, wolfs[j].current_position.z);
                    if (DistanceBetweernBulletAndWolf < 70)
                    {
                        //decrease wolf health by 1 and if the health reached to 0 mark as DEAD
                        wolfs[j].DecreaseHealth();

                        removeBullet = true;
                        break;


                    }

                }


                next:

                if (bullets[i].position.z >= 1904 || removeBullet)
                {

                    bullets.RemoveAt(i);

                }

            }

        }

        bool musicPlaying = false;
        bool musicIsStopped = false;

        public void Draw()
        {
            if (load == true && option_screen == true && about == true)
            {
                //About

                Gl.glDisable(Gl.GL_DEPTH_TEST);
                //use 2D shader
                shader2D.UseShader();
                //draw 2D square 
                Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
                Gl.glEnableVertexAttribArray(0);
                Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
                Gl.glEnableVertexAttribArray(1);
                Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));

                healthbar = MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1), new vec3(1, 1, 1)), glm.translate(new mat4(1), new vec3(0, 0f, 0)) });
                Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
                background_about.Bind();
                Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                Gl.glEnable(Gl.GL_DEPTH_TEST);
            }
            ////////////// if option screen opened option = false 
            if (load == false)
            {
                /////////////////////// load back ground 
                Gl.glDisable(Gl.GL_DEPTH_TEST);
                //use 2D shader
                shader2D.UseShader();
                //draw 2D square 
                Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
                Gl.glEnableVertexAttribArray(0);
                Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
                Gl.glEnableVertexAttribArray(1);
                Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));

                healthbar = MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1), new vec3(1, 1, 1)), glm.translate(new mat4(1), new vec3(0, 0f, 0)) });
                Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
                load_screen.Bind();
                Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                Gl.glEnable(Gl.GL_DEPTH_TEST);
                /////////////////////////////////////////////// end load back ground 
                Gl.glDisable(Gl.GL_DEPTH_TEST);
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
                //Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                //decrease the health bar by scaling down the 2D front square 
                scalef -= 0.002f;
                if (scalef < 0)
                {
                    scalef = 0;
                    load = true;
                }
                healthbar = MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1), new vec3(0.5f * scalef, 0.1f, 1)), glm.translate(new mat4(1), new vec3(0.01f - ((1 - scalef) * 0.48f), -0.14f, 0)) });
                Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
                hp2.Bind();
                //draw front health bar 
                Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                //re-enable the depthtest to draw the other 3D objects in the scene 
                Gl.glEnable(Gl.GL_DEPTH_TEST);

            }
            else if (about == false)
            {
                if (option_screen == false && load == true)
                {
                    if (SystemSoundIsOpened)
                    {
                        if (!musicPlaying)
                        {
                            startScreen.PlayLooping();
                            musicPlaying = true;
                        }
                    }
                    else
                    {
                        startScreen.Stop();
                        musicPlaying = false;
                    }
                    //////////////////////////////////////////////// back ground 
                    Gl.glDisable(Gl.GL_DEPTH_TEST);
                    //use 2D shader
                    shader2D.UseShader();
                    //draw 2D square 
                    Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
                    Gl.glEnableVertexAttribArray(0);
                    Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
                    Gl.glEnableVertexAttribArray(1);
                    Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));

                    healthbar = MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1), new vec3(1, 1, 1)), glm.translate(new mat4(1), new vec3(0, 0f, 0)) });
                    Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
                    background.Bind();
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                    Gl.glEnable(Gl.GL_DEPTH_TEST);

                    /////////////////////////////////////////// start button 
                    Gl.glDisable(Gl.GL_DEPTH_TEST);
                    //use 2D shader
                    shader2D.UseShader();
                    //draw 2D square 
                    Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
                    Gl.glEnableVertexAttribArray(0);
                    Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
                    Gl.glEnableVertexAttribArray(1);
                    Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));

                    healthbar = MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1), new vec3(0.3f, 0.15f, 0)), glm.translate(new mat4(1), new vec3(0, 0.6f, 0)) });
                    Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
                    play_button_texture.Bind();
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                    Gl.glEnable(Gl.GL_DEPTH_TEST);

                    /////////////////////////////////////////////// score button
                    Gl.glDisable(Gl.GL_DEPTH_TEST);
                    //use 2D shader
                    shader2D.UseShader();
                    //draw 2D square 
                    Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
                    Gl.glEnableVertexAttribArray(0);
                    Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
                    Gl.glEnableVertexAttribArray(1);
                    Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));

                    healthbar = MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1), new vec3(0.3f, 0.15f, 0)), glm.translate(new mat4(1), new vec3(0, 0.2f, 0)) });
                    Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
                    option_button.Bind();
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                    Gl.glEnable(Gl.GL_DEPTH_TEST);

                    ///////////////////////////////////////////////// about button
                    Gl.glDisable(Gl.GL_DEPTH_TEST);
                    //use 2D shader
                    shader2D.UseShader();
                    //draw 2D square 
                    Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
                    Gl.glEnableVertexAttribArray(0);
                    Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
                    Gl.glEnableVertexAttribArray(1);
                    Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));

                    healthbar = MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1), new vec3(0.3f, 0.15f, 0)), glm.translate(new mat4(1), new vec3(0, -0.2f, 0)) });
                    Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
                    about_button.Bind();
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                    Gl.glEnable(Gl.GL_DEPTH_TEST);
                    /////////////////////////////////////////////////exit button
                    Gl.glDisable(Gl.GL_DEPTH_TEST);
                    //use 2D shader
                    shader2D.UseShader();
                    //draw 2D square 
                    Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
                    Gl.glEnableVertexAttribArray(0);
                    Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
                    Gl.glEnableVertexAttribArray(1);
                    Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));

                    healthbar = MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1), new vec3(0.3f, 0.15f, 0)), glm.translate(new mat4(1), new vec3(0, -0.6f, 0)) });
                    Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
                    exit_button.Bind();
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                    Gl.glEnable(Gl.GL_DEPTH_TEST);
                    Gl.glDisableVertexAttribArray(0);
                    Gl.glDisableVertexAttribArray(1);
                }
                else
                {
                    if (!musicIsStopped)
                    {
                        startScreen.Stop();
                        musicIsStopped = true;
                        musicPlaying = false;
                    }
                    // draw the Game ----
                    // using to clear Buffer 
                    Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
                    //use 3D shader
                    sh.UseShader();


                    // for Light Simulation
                    ambientLight = DayLight + (NightLight - DayLight) * t;
                    if (LightTime)
                        t -= 0.0002f;
                    else
                        t += 0.0002f;
                    if (t > 0.8f)
                        LightTime = true;
                    else
                        LightTime = false;
                    Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());
                    ///////////////////////////////////////////////////////////////////////////////////////

                    //draw 3D model Player by using Camera Centre .. ###################3
                    playerPos = cam.GetCameraTarget();
                    playerPos.y -= 2.5f;
                    playerPos.z += 2f;
                    m.rotmatrix = glm.rotate(3.1412f + cam.mAngleX, new vec3(0, 1, 0));
                    m.transmatrix = glm.translate(new mat4(1), playerPos);
                    m.Draw(transID);

                    // ###########################################


                    //send shader values 
                    Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
                    Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
                    Gl.glUniform3fv(EyePositionID, 1, cam.GetCameraPosition().to_array());
                    ////////////////////////////////////////////////////////////

                    ////////////////////////////////////////////////////////////


                    //    city.Draw(transID);

                    Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, ShootID);
                    Gl.glEnableVertexAttribArray(0);
                    Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)0);

                    Gl.glEnableVertexAttribArray(1);
                    Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(3 * sizeof(float)));

                    Gl.glEnableVertexAttribArray(2);
                    Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(6 * sizeof(float)));

                    Gl.glEnableVertexAttribArray(3);
                    Gl.glVertexAttribPointer(3, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(8 * sizeof(float)));

                    shoot.Bind();
                    vec3 shootpos = cam.GetCameraTarget();
                    shootpos.y -= 1.5f;
                    shootpos += cam.GetLookDirection() * 8;

                    Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1),new vec3(2+(float)c/10,2 + (float)c / 10, 2 + (float)c / 10)),
                glm.rotate(cam.mAngleX, new vec3(0, 1, 0)),glm.rotate((float)c/10, new vec3(0, 0, 1)),glm.translate(new mat4(1),shootpos),
            }).to_array());
                    Gl.glEnable(Gl.GL_BLEND);
                    Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

                    if (draw)
                    {
                        cam.mAngleY -= 0.01f;
                        Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                        c--;
                        if (c < 0)
                        {
                            cam.mAngleY = 0;
                            c = timer;
                            draw = false;
                        }
                    }
                    Gl.glDisable(Gl.GL_BLEND);


                    // sky box vertces 
                    GPU.BindBuffer(vertexBufferID);

                    Gl.glEnableVertexAttribArray(0);
                    Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), IntPtr.Zero);
                    // coloc sky box 


                    Gl.glEnableVertexAttribArray(1);
                    Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
                    // for texture Sky box
                    Gl.glEnableVertexAttribArray(2);
                    Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));



                    // ####################################### start draw Faces 
                    tex_down.Bind();
                    Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, down.to_array());
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);


                    tex_up.Bind();
                    Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, up.to_array());
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

                    tex_down.Bind();
                    Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, down.to_array());
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

                    tex_left.Bind();

                    Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, left.to_array());
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

                    tex_right.Bind();

                    Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, right.to_array());
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

                    tex_front.Bind();

                    Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, front.to_array());
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);


                    tex_back.Bind();
                    Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, back.to_array());
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

                    //################################################### ENd Draw faces 

                    // ###################### Draw Static Models #######################
                    for (int i = 0; i < trees.Count; i++)
                        trees[i].Model3D_obj.Draw(transID);
                    for (int i = 0; i < eagles.Count; i++)
                        eagles[i].Model3D_obj.Draw(transID);

                    house.Model3D_obj.Draw(transID);
                    house2.Model3D_obj.Draw(transID);
                    //#########################################################################

                    // ################## Draw Animated Models 

                    for (int i = 0; i < wolfs.Count; i++)
                    {
                        wolfs[i].model.Draw(transID);

                        /* if (wolfs[i].isDead)
                         {
                             wolfs[i].Death();

                         }*/
                    }
                    //////////////////////////////////////////////////////////////draw health bar 
                    Gl.glDisable(Gl.GL_DEPTH_TEST);
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
                    bhp.Bind();
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                    //decrease the health bar by scaling down the 2D front square 

                    healthbar2 = MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1), new vec3(1f * scalf2, 0.051f, 1)), glm.translate(new mat4(1), new vec3(-1f - ((1 - scalf2) * 0.48f), 0.9f, 0)) });
                    Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar2.to_array());
                    hp.Bind();
                    //draw front health bar 
                    Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

                    //re-enable the depthtest to draw the other 3D objects in the scene 
                    Gl.glEnable(Gl.GL_DEPTH_TEST);
                    ///////////////////////////////////////////////////////////////end draw health 
                    Gl.glDisableVertexAttribArray(0);
                    Gl.glDisableVertexAttribArray(1);

                    /////////####################### stone 
                    stone_texture.Bind();
                    stone.Model3D_obj.Draw(transID);

                    // #######################################333

                    draw_bullet();


                }



            }

        }
        int timer = 5;
        int c;
        public bool draw = false;
        bool gameIsOver = false;
        bool playWin = false;
        public void Update(float deltaTime)
        {
            if (wolfs.Count == 0)
            {
                //Player Win !!
                if (!playWin)
                {

                    playWin = true;
                    WinFormcs win = new WinFormcs();
                    win.Show();

                }
            }
            //estimating runtime
            TimeSpan time = TimeSpan.FromSeconds(deltaTime);

            // Any Update Done by Keys Will Update Class Camera Values 
            //then we Update Render values from Camera  during run time  .. 
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            for (int i = 0; i < wolfs.Count; i++)
                wolfs[i].model.UpdateExportedAnimation(0.005f);


            playerPos.y -= 3.5f;

            // Movements  enemy will mov to Hero if distance between Them less than DetectionRange 

            for (int i = 0; i < wolfs.Count; i++)
            {

                //check if any wolf marked as DEAD and Do the next ..
                // if there a wolf was dead from milliseconds then it would disapear   
                if (wolfs[i].isDead && (wolfs[i].DeadAt.Seconds == time.Seconds))
                {

                    wolfs.RemoveAt(i);
                    continue;
                }
                //if there is wolf marked as DEAD then set death animation and death time
                if (wolfs[i].isDead)
                {
                    wolfs[i].Death(time);
                }
                else
                {
                    // Calculate distance between them using Ecludin Equation .. using playerpos and current_position 
                    Distance_between_Enemy_And_Playr = MathHelper.Get_Distance(playerPos.x, playerPos.y, playerPos.z, wolfs[i].current_position.x, wolfs[i].current_position.y, wolfs[i].current_position.z);
                    if (Distance_between_Enemy_And_Playr < DetectionRange)
                    {
                        if (Distance_between_Enemy_And_Playr < AttackRange)
                        {
                            wolfs[i].Attack();
                            scalf2 -= 0.0001f;

                            if (scalf2 <= 0.3)
                            {
                                if(!gameIsOver)
                                {
                                    PlayerDeath.Play();
                                  
                                    //////////////////// when player death 
                                    gameIsOver = true;
                                    GameOverForm gameOver = new GameOverForm();
                                    gameOver.Show();

                                }
                               
                            }
                        }

                        else
                        {
                            //if the walf has not any collision then move toward the player
                            if (!WolfCollisionDetection(i))
                                wolfs[i].Mov(playerPos - wolfs[i].current_position);
                            else
                            {
                                //take decision when collision occured
                                vec3 WolfRight = glm.cross((playerPos - wolfs[i].current_position), new vec3(0, 1, 0));
                                vec3 WolfLeft = glm.cross((wolfs[i].current_position-playerPos), new vec3(0, 1, 0));
                                wolfs[i].Mov(WolfRight);
                              

                            }

                        }



                    }
                    else
                        wolfs[i].set_defualt_animation();
                }


            }


            //####################################### draw bullet


            updateBullet();



        }

        public void SendLightData(float red, float green, float blue, float attenuation, float specularExponent)
        {
            vec3 ambientLight = new vec3(red, green, blue);
            Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());
            vec2 data = new vec2(attenuation, specularExponent);
            Gl.glUniform2fv(DataID, 1, data.to_array());
        }
        bool WolfCollisionDetection(int WolfIndex)
        {
            for (int i = 0; i < wolfs.Count; i++)
            {
                if (WolfIndex == i)
                    continue;
                float distanceBetweenWolfandOther = MathHelper.Get_Distance(wolfs[WolfIndex].current_position.x, wolfs[WolfIndex].current_position.y, wolfs[WolfIndex].current_position.z, wolfs[i].current_position.x, wolfs[i].current_position.y, wolfs[i].current_position.z);

                if (distanceBetweenWolfandOther < 50)
                    return true;
            }
            for (int i = 0; i < trees.Count; ++i)
            {
                float distanceBetweenWolfandTree = MathHelper.Get_Distance(wolfs[WolfIndex].current_position.x, wolfs[WolfIndex].current_position.y, wolfs[WolfIndex].current_position.z, trees[i].current_pos.x, trees[i].current_pos.y, trees[i].current_pos.z);
                if (distanceBetweenWolfandTree < 90)
                    return true;

            }
            float distanceBetweenWolfandHouse1 = MathHelper.Get_Distance(wolfs[WolfIndex].current_position.x, wolfs[WolfIndex].current_position.y, wolfs[WolfIndex].current_position.z, house.current_pos.x, house.current_pos.y, house.current_pos.z);
            if (distanceBetweenWolfandHouse1 < 350)
                return true;
            float distanceBetweenWolfandHouse2 = MathHelper.Get_Distance(wolfs[WolfIndex].current_position.x, wolfs[WolfIndex].current_position.y, wolfs[WolfIndex].current_position.z, house2.current_pos.x, house2.current_pos.y, house2.current_pos.z);
            if (distanceBetweenWolfandHouse2 < 350)
                return true;
            return false;
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
