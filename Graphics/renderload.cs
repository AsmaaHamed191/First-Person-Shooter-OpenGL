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

namespace Graphics
{
    class renderload
    {
        Shader shad1;
        Texture hp;
 
        uint bufferid;
        uint vertexBufferID1;

        Shader shader2D;

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            // set coloring 
            shad1 = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            Gl.glClearColor(0, 0, 0, 1);
            float[] foll_screen =
             {
               -1,1, 
                1,1,
                1,-1,

                -1,0,
                -1,-1,
                1,-1


            };


            vertexBufferID1 = GPU.GenerateBuffer(foll_screen);
        }

        public void Draw()
        {
            shad1.UseShader();

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, vertexBufferID1);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
        }
        public void Update()
        {

        }
        public void CleanUp()
        {
            shad1.DestroyShader();
           // shad2.DestroyShader();
        }
    }
}
