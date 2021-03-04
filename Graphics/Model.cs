using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using GlmNet;
using Tao.OpenGl;

namespace Graphics
{
    class Model
    {
        uint vertexBufferID;
        uint colorBufferID;
        uint normalBufferID;
        uint uvBufferID;
        uint indexBufferID;

        public List<vec3> vertices;
        public List<vec2> uvCoordinates;
        public List<vec3> normals;
        public List<vec3> colors;
        public List<int> indices;
        public mat4 transformationMatrix;
        public Texture texture;

        public Model()
        {
            vertices = new List<vec3>();
            uvCoordinates = new List<vec2>();
            normals = new List<vec3>();
            indices = new List<int>();
            colors = new List<vec3>();
            transformationMatrix = new mat4(1);
        }
        public void Initialize()
        {
            if (vertices.Count > 0)
                vertexBufferID = GPU.GenerateBuffer(vertices);
            if (colors.Count > 0)
                colorBufferID = GPU.GenerateBuffer(colors);
            if (normals.Count > 0)
                normalBufferID = GPU.GenerateBuffer(normals);
            if (uvCoordinates.Count > 0)
                uvBufferID = GPU.GenerateBuffer(uvCoordinates);
            if (indices.Count > 0)
                indexBufferID = GPU.GenerateElementBuffer(indices);
        }

        public void Update(float[] vs,float[] ns)
        {
                GPU.UpdateBuffer(vs, vertexBufferID);
                GPU.UpdateBuffer(ns, normalBufferID);

        }

        public void Draw(int matID)
        {
            Gl.glUniformMatrix4fv(matID, 1, Gl.GL_FALSE, transformationMatrix.to_array());
            
            GPU.BindBuffer(vertexBufferID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
            if (colors.Count > 0)
            {
                GPU.BindBuffer(colorBufferID);
                Gl.glEnableVertexAttribArray(1);
                Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
            }
            if (uvCoordinates.Count > 0)
            {
                GPU.BindBuffer(uvBufferID);
                Gl.glEnableVertexAttribArray(2);
                Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
            }
            if (normals.Count > 0)
            {
                GPU.BindBuffer(normalBufferID);
                Gl.glEnableVertexAttribArray(3);
                Gl.glVertexAttribPointer(3, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
            }

            if (texture != null)
                texture.Bind();
            if (indices.Count == 0)
                Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, vertices.Count);
            else
                Gl.glDrawElements(Gl.GL_TRIANGLES, indices.Count, Gl.GL_UNSIGNED_SHORT, IntPtr.Zero);

            Gl.glDisableVertexAttribArray(0);
            if(colors.Count>0)
                Gl.glDisableVertexAttribArray(1);
            if(uvCoordinates.Count > 0)
                Gl.glDisableVertexAttribArray(2);
            if(normals.Count > 0)
                Gl.glDisableVertexAttribArray(3);
        }

        public void Draw(int matID,mat4 scale,mat4 rot, mat4 trans)
        {
            List<mat4> modelmatrices = new List<mat4>() { scale,rot,trans,transformationMatrix};
            mat4 modelmatrix = MathHelper.MultiplyMatrices(modelmatrices);
            Gl.glUniformMatrix4fv(matID, 1, Gl.GL_FALSE, modelmatrix.to_array());
            GPU.BindBuffer(vertexBufferID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
            if (colors.Count > 0)
            {
                GPU.BindBuffer(colorBufferID);
                Gl.glEnableVertexAttribArray(1);
                Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
            }
            if (uvCoordinates.Count > 0)
            {
                GPU.BindBuffer(uvBufferID);
                Gl.glEnableVertexAttribArray(2);
                Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
            }
            if (normals.Count > 0)
            {
                GPU.BindBuffer(normalBufferID);
                Gl.glEnableVertexAttribArray(3);
                Gl.glVertexAttribPointer(3, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
            }

            if (texture != null)
                texture.Bind();
            if (indices.Count == 0)
                Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, vertices.Count);
            else
                Gl.glDrawElements(Gl.GL_TRIANGLES, indices.Count, Gl.GL_UNSIGNED_SHORT, IntPtr.Zero);

            Gl.glDisableVertexAttribArray(0);
            if (colors.Count > 0)
                Gl.glDisableVertexAttribArray(1);
            if (uvCoordinates.Count > 0)
                Gl.glDisableVertexAttribArray(2);
            if (normals.Count > 0)
                Gl.glDisableVertexAttribArray(3);
        }
    }
}
