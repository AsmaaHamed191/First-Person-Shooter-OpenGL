using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using Graphics._3D_Models;
using System.Diagnostics;
using System.Threading;
namespace Graphics
{
    class bullet
    {
        public vec3 position, direction;
        public Model3D bullets;
        Shader sh;
        int transID;
        int projID;
        int viewID;
        int DataID;
        vec3 boundbox;
        int num_of_bullets;
        public bullet(int trans, vec3 cam_target, vec3 position)
        {
            num_of_bullets = 5;
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            transID = trans;
            bullets = new Model3D();
            bullets.LoadFile(projectPath + "\\ModelFiles\\static", 8, "sphere.OBJ");
            direction = cam_target;

            this.position = position;
            position.y += -3;
            // position.x += 4;
            bullets.scalematrix = glm.scale(new mat4(1), new vec3(0.2f, 0.2f, 0.2f));
            /* bullets.transmatrix = glm.translate(new mat4(1), position); 
              bullets.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));*/


        }
        public void Draw()
        {

            bullets.Draw(transID);

        }
        public void Update()
        {
            position += direction * 5f;
            bullets.transmatrix = glm.translate(new mat4(1), position);
        }
        /*public void Translate(vec3 direction)
        {
            // set translation of bullet 
    
            position.y += 7; // increase y position same position of player 
            bullets.transmatrix = bullets.transmatrix = glm.translate(new mat4(1), direction);
            position = direction;
        }
        */
        /*  public void shoot(vec3 direction)
          {

              bullets.scalematrix = glm.scale(new mat4(1), new vec3(0.4f, 0.4f, 0.4f));

                 // position.y += 0.007f;
                  for (int i = 0; i < 2000; i++)
                  {

                      position += direction * 0.005f;
                      bullets.transmatrix = glm.translate(new mat4(1), position);


              }
          }*/
        public void clear()
        {
            this.bullets.scalematrix = glm.scale(new mat4(1), new vec3(0, 0, 0));
        }

    }
}
