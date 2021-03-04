using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    class Ground
    {
        Model groundModel;
        int groundStride;
        float[,] heights;
        int w, l;
        public Ground(float width, float length, float height, int stride)
        {
            groundStride = stride;
            groundModel = new Model();
            Random r = new Random();
            w = (int)width;
            l = (int)length;
            heights = new float[(int)(width / stride), (int)(length / stride)];

            for (int i = 0; i < width/stride; i++)
            {
                for (int j = 0; j < length/stride; j++)
                {
                    heights[i, j] = (float)r.NextDouble() * height;
                }
            }


            for (int i = 0; i < width - stride; i += stride)
            {
                for (int j = 0; j < length - stride; j += stride)
                {
                    vec3 v1 = new vec3(i - width / 2, heights[(int)(i / stride), (int)(j / stride)], j - length / 2);
                    vec3 v2 = new vec3(i - width / 2 + stride, heights[(int)(i / stride)+1, (int)(j / stride)], j - length / 2);
                    vec3 v3 = new vec3(i - width / 2, heights[(int)(i / stride), (int)(j / stride)+1], j - length / 2 + stride);
                    
                    groundModel.vertices.Add(v1);
                    groundModel.vertices.Add(v2);
                    groundModel.vertices.Add(v3);

                    vec3 n1 = v3 - v1;
                    vec3 n2 = v2 - v1;
                    vec3 t1Normal = glm.cross(n1, n2);
                    t1Normal = glm.normalize(t1Normal);

                    groundModel.normals.Add(t1Normal);
                    groundModel.normals.Add(t1Normal);
                    groundModel.normals.Add(t1Normal);

                    vec3 v4 = new vec3(i - width / 2 + stride, heights[(int)(i / stride) + 1, (int)(j / stride)], j - length / 2);
                    vec3 v5 = new vec3(i - width / 2, heights[(int)(i / stride), (int)(j / stride) + 1], j - length / 2 + stride);
                    vec3 v6 = new vec3(i - width / 2 + stride, heights[(int)(i / stride) + 1, (int)(j / stride) + 1], j - length / 2 + stride);

                    groundModel.vertices.Add(v6);
                    groundModel.vertices.Add(v5);
                    groundModel.vertices.Add(v4);

                    n1 = v4 - v6;
                    n2 = v5 - v6;


                    t1Normal = glm.cross(n1, n2);
                    t1Normal = glm.normalize(t1Normal);

                    groundModel.normals.Add(t1Normal);
                    groundModel.normals.Add(t1Normal);
                    groundModel.normals.Add(t1Normal);

                    groundModel.uvCoordinates.Add(new vec2(0, 0));
                    groundModel.uvCoordinates.Add(new vec2(1, 0));
                    groundModel.uvCoordinates.Add(new vec2(0, 1));
                    groundModel.uvCoordinates.Add(new vec2(1, 0));
                    groundModel.uvCoordinates.Add(new vec2(0, 1));
                    groundModel.uvCoordinates.Add(new vec2(1, 1));

                }
            }
            groundModel.Initialize();
        }


        public float GetHeight(float x, float z)
        {
            float xloc = (x+w/2) / groundStride;
            int xleft = (int)xloc;
            int xright = xleft + 1;
            float dx = xloc - xleft;

            float zloc = (z+l/2) / groundStride;
            int zfront = (int)zloc;
            int zback = zfront + 1;
            float dz = zloc - zfront;


            float height1 = heights[xleft, zfront];
            float height2 = heights[xright, zfront];
            float height3 = heights[xleft, zback];
            float height4 = heights[xright, zback];

            float heightx1 = height1 * (1 - dx) + height2 * dx;
            float heightx2 = height3 * (1 - dx) + height4 * dx;

            float heightz = heightx1 * (1 - dz) + heightx2 * dz;

            return heightz;
        }
        public void Draw(int matID)
        {
            groundModel.Draw(matID);
        }
    }
}
