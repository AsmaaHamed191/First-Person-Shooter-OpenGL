using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmNet;
using Graphics._3D_Models;

namespace Graphics
{
    class StaticModel
    {
        public Model3D Model3D_obj;
        public vec3 current_pos;
        public StaticModel()
        {
            this.Model3D_obj = new Model3D();
        }

        public void translate(vec3 pos)
        {
            this.Model3D_obj.transmatrix = glm.translate(new mat4(1), pos);
            this.current_pos = pos;
        }


    }
}
