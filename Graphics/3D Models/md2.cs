using GlmNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

namespace Graphics._3D_Models
{
    
    enum animType
    {
        STAND,
        RUN,
        ATTACK,
        PAIN_A,
        PAIN_B,
        PAIN_C,
        JUMP,
        FLIP,
        SALUTE,
        FALLBACK,
        WAVE,
        POINTING, 
        CROUCH_STAND,
        CROUCH_WALK,
        CROUCH_ATTACK,
        CROUCH_PAIN,
        CROUCH_DEATH,
        DEATH_FALLBACK,
        DEATH_FALLFORWARD,
        DEATH_FALLBACKSLOW,
        BOOM,
        PacManWalk,
        PacManJump,
        PacManStand,
        MAX_ANIMATIONS

    }
    struct animState
    {

        public int startframe;             // first frame
        public int endframe;               // last frame
        public int fps;                    // frame per second for this animation

        public float curr_time;                // current time
        public float old_time;             // old time
        public float interpol;             // percent of interpolation

        public animType type;                   // animation type

        public int curr_frame;             // current frame
        public int next_frame;             // next frame

    }

    class md2
    {
        public mat4 TranslationMatrix;
        public mat4 rotationMatrix;
        public mat4 scaleMatrix;
        List<string> MD2AnimationNames;
        List<anim_t> animlist;
        List<List<vec3>> vVertices;
        List<List<vec3>> vNormals;


        List<vec3> CvVertices;
        List<vec3> CvNormals;

        List<List<vec3>> AvVertices;
        List<List<vec3>> AvNormals;

        List<List<int>> vNormalsindex;
        List<vec2> UVData;
        List<string> skins;
        List<int> indices;
        List<vec3> mVertices;
        public Model m;

        List<int> renderModes;
        List<int> numRenderVertices;
        List<vec2> anotherUV;

        public animState animSt;
        public float AnimationSpeed;
        public bool Loop;
        float oldframe;
        float currframe;
        float[] vs, ns;
        public md2(string fileName)
        {
            animlist = new List<anim_t>();
            anim_t a1 = new anim_t();
            a1.first_frame = 0; a1.last_frame = 39; a1.fps = 9;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 40; a1.last_frame = 45; a1.fps = 10;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 46; a1.last_frame = 53; a1.fps = 10;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 54; a1.last_frame = 57; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 58; a1.last_frame = 61; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 62; a1.last_frame = 65; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 66; a1.last_frame = 71; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 72; a1.last_frame = 83; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 84; a1.last_frame = 94; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 95; a1.last_frame = 111; a1.fps = 10;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 112; a1.last_frame = 122; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 123; a1.last_frame = 134; a1.fps = 6;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 135; a1.last_frame = 153; a1.fps = 10;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 154; a1.last_frame = 159; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 160; a1.last_frame = 168; a1.fps = 10;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 169; a1.last_frame = 172; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 173; a1.last_frame = 177; a1.fps = 5;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 178; a1.last_frame = 183; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 184; a1.last_frame = 189; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 190; a1.last_frame = 197; a1.fps = 7;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 198; a1.last_frame = 198; a1.fps = 5;
            animlist.Add(a1);

            MD2AnimationNames = new List<string>();
            MD2AnimationNames.Add("Stand");
            MD2AnimationNames.Add("Run");
            MD2AnimationNames.Add("Attach");
            MD2AnimationNames.Add("Pain_A");
            MD2AnimationNames.Add("Pain_B");
            MD2AnimationNames.Add("Pain_C");
            MD2AnimationNames.Add("Jump");
            MD2AnimationNames.Add("Flip");
            MD2AnimationNames.Add("Salute");
            MD2AnimationNames.Add("Fallback");
            MD2AnimationNames.Add("Wave");
            MD2AnimationNames.Add("Pointing");
            MD2AnimationNames.Add("Crouch_stand");
            MD2AnimationNames.Add("Crouch_walk");
            MD2AnimationNames.Add("Crouch_attack");
            MD2AnimationNames.Add("Crouch_pain");
            MD2AnimationNames.Add("Crouch_death");
            MD2AnimationNames.Add("Death_fallback");
            MD2AnimationNames.Add("Death_fallforward");
            MD2AnimationNames.Add("Death_fallbackslow");
            MD2AnimationNames.Add("BOOM");

            TranslationMatrix = new mat4(1);
            rotationMatrix = new mat4(1);
            scaleMatrix = new mat4(1);

            LoadModel(fileName);
        }

        void LoadModel(string path,bool flip = true)
        {
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(stream);
            md2_t header = new md2_t();
            header.ident = br.ReadInt32();
            header.version = br.ReadInt32();
            header.skinheight = br.ReadInt32();
            header.skinwidth = br.ReadInt32();
            header.framesize = br.ReadInt32();

            header.num_skins = br.ReadInt32();
            header.num_xyz = br.ReadInt32();
            header.num_st = br.ReadInt32();
            header.num_tris = br.ReadInt32();
            header.num_glcmds = br.ReadInt32();
            header.num_frames = br.ReadInt32();

            header.ofs_skins = br.ReadInt32();
            header.ofs_st = br.ReadInt32();
            header.ofs_tris = br.ReadInt32();
            header.ofs_frames = br.ReadInt32();
            header.ofs_glcmds = br.ReadInt32();
            header.ofs_end = br.ReadInt32();


            br.BaseStream.Seek(header.ofs_skins, SeekOrigin.Begin);

            skins = new List<string>();

            for (int i = 0; i < header.num_skins; i++)
            {
                string s = new string(br.ReadChars(64));
                skins.Add(s);
            }

            UVData = new List<vec2>();
            br.BaseStream.Seek(header.ofs_st, SeekOrigin.Begin);
            for (int i = 0; i < header.num_st; i++)
            {
                vec2 uv = new vec2();
                uv.x = ((float)br.ReadInt16()) / header.skinwidth;
                uv.y = ((float)br.ReadInt16()) / header.skinheight;
                UVData.Add(uv);
            }
            int x = header.num_xyz - header.num_st;
            if (x > 0)
                for (int i = 0; i < x; i++)
                {
                    UVData.Add(new vec2(0));
                }

            indices = new List<int>();
            br.BaseStream.Seek(header.ofs_tris, SeekOrigin.Begin);
            for (int i = 0; i < header.num_tris; i++)
            {
                indices.Add(br.ReadUInt16());
                indices.Add(br.ReadUInt16());
                indices.Add(br.ReadUInt16());
                br.ReadUInt16();
                br.ReadUInt16();
                br.ReadUInt16();

                //loop on indices each 3 consecutive vertices compute normal
            }


            br.BaseStream.Seek(header.ofs_frames, SeekOrigin.Begin);

            vVertices = new List<List<vec3>>();
            vNormalsindex = new List<List<int>>();
            vNormals = new List<List<vec3>>();
            for (int i = 0; i < header.num_frames; i++)
            {
                br.BaseStream.Seek((header.ofs_frames + (i * header.framesize)), SeekOrigin.Begin);

                vVertices.Add(new List<vec3>(header.num_xyz));
                vNormalsindex.Add(new List<int>(header.num_xyz));
                vNormals.Add(new List<vec3>(header.num_xyz));

                vec3 scale = new vec3();
                byte[] buffer;
                buffer = br.ReadBytes(4);
                scale.x = BitConverter.ToSingle(buffer, 0);
                buffer = br.ReadBytes(4);
                scale.y = BitConverter.ToSingle(buffer, 0);
                buffer = br.ReadBytes(4);
                scale.z = BitConverter.ToSingle(buffer, 0);

                vec3 trans = new vec3();
                buffer = br.ReadBytes(4);
                trans.x = BitConverter.ToSingle(buffer, 0);
                buffer = br.ReadBytes(4);
                trans.y = BitConverter.ToSingle(buffer, 0);
                buffer = br.ReadBytes(4);
                trans.z = BitConverter.ToSingle(buffer, 0);

                string name = new string(br.ReadChars(16));

                List<vertex_t> vts = new List<vertex_t>();
                for (int j = 0; j < header.num_xyz; j++)
                {
                    vertex_t vt = new vertex_t();
                    vt.v = br.ReadBytes(3);
                    vt.lightnormalindex = br.ReadByte();
                    vts.Add(vt);
                }
                frame_t f = new frame_t();
                f.scale = scale.to_array();
                f.translate = trans.to_array();
                f.name = name.ToCharArray();
                f.verts = vts;

                for (int j = 0; j < header.num_xyz; j++)
                {

                    vVertices[i].Add(new vec3(0));
                    vNormals[i].Add(new vec3(0));
                    vVertices[i][j] = new vec3()
                    {
                        x = f.translate[0] + ((float)f.verts[j].v[0]) * f.scale[0],
                        y = f.translate[1] + ((float)f.verts[j].v[1]) * f.scale[1],
                        z = f.translate[2] + ((float)f.verts[j].v[2]) * f.scale[2]
                    };

                    vNormalsindex[i].Add(f.verts[j].lightnormalindex);

                }
                for (int j = 0; j < indices.Count - 3; j += 3)
                {

                    vec3 n1 = vVertices[i][indices[j + 1]] - vVertices[i][indices[j]];
                    vec3 n2 = vVertices[i][indices[j + 2]] - vVertices[i][indices[j]];
                    vec3 normal = glm.cross(n1, n2);
                    vNormals[i][indices[j]] = normal;
                    vNormals[i][indices[j + 1]] = normal;
                    vNormals[i][indices[j + 2]] = normal;
                }

            }
            Texture tex = null;
            if (skins.Count > 1)
            {
                tex = new Texture(skins[1], 20, true);
            }
            else
            {
                FileInfo fi = new FileInfo(path);
                string name = fi.Name;
                int index = fi.FullName.IndexOf(name, StringComparison.CurrentCulture);
                string filePath = "";
                for (int i = 0; i < index; i++)
                {
                    filePath += fi.FullName[i];
                }
                string[] extensions = { "jpg", "jpeg", "png", "bmp" };
                for (int i = 0; i < 4; i++)
                {
                    string stry = filePath + name.Split('.')[0] + "." + extensions[i];
                    tex = new Texture(stry, 20, true);
                    if (tex.width > 0)
                        break;
                }
            }

            br.BaseStream.Seek(header.ofs_glcmds, SeekOrigin.Begin);
            List<int> cmds = new List<int>();
            renderModes = new List<int>();
            numRenderVertices = new List<int>();
            anotherUV = new List<vec2>();

            AvVertices = new List<List<vec3>>();
            AvNormals = new List<List<vec3>>();
            CvNormals = new List<vec3>();
            CvVertices = new List<vec3>();
            for (int j = 0; j < header.num_frames; j++)
            {
                AvVertices.Add(new List<vec3>());
                AvNormals.Add(new List<vec3>());
            }

            for (int i = 0; i < header.num_glcmds; i++)
            {
                cmds.Add(br.ReadInt32());
            }
            int k = 0;
            while (true)
            {
                int action;
                if (cmds.Count > 0)
                {
                    action = cmds[k];
                    if (action == 0) break;
                }
                else
                    action = 1;

                int renderMode = action < 0 ? Gl.GL_TRIANGLE_FAN : Gl.GL_TRIANGLE_STRIP;
                int numVertices = action < 0 ? -action : action;
                k++;

                renderModes.Add(renderMode);
                numRenderVertices.Add(numVertices);
                for (int i = 0; i < numVertices; i++)
                {
                    unsafe
                    {
                        int sd = cmds[k++];
                        int td = cmds[k++];
                        float s = *((float*)(&sd));
                        float t = *((float*)(&td));
                        if (flip)
                            t = 1 - t;
                        int vi = cmds[k++];
                        anotherUV.Add(new vec2(s, t));
                        for (int j = 0; j < header.num_frames; j++)
                        {
                            AvVertices[j].Add(vVertices[j][vi]);
                            AvNormals[j].Add(vNormals[j][vi]);
                            CvVertices.Add(new vec3(0, 0, 0));
                            CvNormals.Add(new vec3(0, 0, 0));
                        }
                    }
                }
            }

            mVertices = vVertices[0];


            m = new Model();
            m.indices = indices;
            m.texture = tex;
            m.vertices = vVertices[0];
            m.uvCoordinates = UVData;
            m.normals = vNormals[0];

            vs = new float[AvVertices[0].Count * 3];
            ns = new float[AvNormals[0].Count * 3];


            if (AvVertices.Count > 0)
                vertexBufferID = GPU.GenerateBuffer(AvVertices[0]);
            if (AvNormals.Count > 0)
                normalBufferID = GPU.GenerateBuffer(AvNormals[0]);
            if (anotherUV.Count > 0)
                uvBufferID = GPU.GenerateBuffer(anotherUV);

        }

        uint vertexBufferID;
        uint normalBufferID;
        uint uvBufferID;
        public void Draw(int matID)
        {
            List<mat4> modelmatrices = new List<mat4>() { scaleMatrix, rotationMatrix, TranslationMatrix };
            m.transformationMatrix = MathHelper.MultiplyMatrices(modelmatrices);
            m.texture.Bind();
            int iTotalOffset = 0;
            Gl.glUniformMatrix4fv(matID, 1, Gl.GL_FALSE, m.transformationMatrix.to_array());

            GPU.BindBuffer(vertexBufferID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
            GPU.BindBuffer(uvBufferID);
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
            GPU.BindBuffer(normalBufferID);
            Gl.glEnableVertexAttribArray(3);
            Gl.glVertexAttribPointer(3, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);

            for (int i = 0; i < renderModes.Count; i++)
            {
                Gl.glDrawArrays(renderModes[i], iTotalOffset, numRenderVertices[i]);
                iTotalOffset += numRenderVertices[i];
            }

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(2);
            Gl.glDisableVertexAttribArray(3);


        }

        public List<vec3> GetCurrentVertices(animState_t animState)
        {
            return vVertices[animState.curr_frame];
        }

        public void StartAnimation(animType type)
        {
            animState res;
            res.startframe = animlist[(int)type].first_frame;
            res.endframe = animlist[(int)type].last_frame;
            res.curr_frame = animlist[(int)type].first_frame;
            res.next_frame = animlist[(int)type].first_frame + 1;

            res.fps = animlist[(int)type].fps;
            res.type = type;

            res.curr_time = 0.0f;
            res.old_time = 0.0f;

            res.interpol = 0.0f;
            animSt = res;
            AnimationSpeed = 0.01f;
            Loop = true;
            currframe = animSt.curr_frame;
            oldframe = currframe;
        }
        
        public void UpdateExportedAnimation()
        {
            if (!Loop)
                if (animSt.curr_frame == animSt.endframe)
                    return;
            animSt.curr_time += AnimationSpeed;

            if (animSt.curr_time - animSt.old_time > (1.0f / ((float)animSt.fps)))
            {
                animSt.old_time = animSt.curr_time;
                oldframe = currframe;
                animSt.curr_frame = animSt.next_frame;
                currframe = animSt.curr_frame;
                animSt.next_frame++;
                if (animSt.next_frame > animSt.endframe)
                    animSt.next_frame = animSt.startframe;
            }

            animSt.interpol = (float)animSt.fps * (animSt.curr_time - animSt.old_time);
            UpdateVertices();
        }

        public void UpdateVertices()
        {
            if (oldframe != currframe)
            {
                int q = 0;
                for (int i = 0; i < AvVertices[0].Count; i++)
                {
                    if (animSt.interpol >= 0)
                    {
                        vec3 newVertex = AvVertices[animSt.curr_frame][i] + (AvVertices[animSt.next_frame][i] - AvVertices[animSt.curr_frame][i]) * animSt.interpol;
                        vec3 newNormal = AvNormals[animSt.curr_frame][i] + (AvNormals[animSt.next_frame][i] - AvNormals[animSt.curr_frame][i]) * animSt.interpol;
                        ns[q] = newNormal.x;
                        vs[q++] = newVertex.x;
                        ns[q] = newNormal.y;
                        vs[q++] = newVertex.y;
                        ns[q] = newNormal.z;
                        vs[q++] = newVertex.z;
                    }
                }

                if (AvVertices.Count > 0)
                    GPU.UpdateBuffer(vs, vertexBufferID);
                if (AvNormals.Count > 0)
                    GPU.UpdateBuffer(ns, normalBufferID);
            }
        }
    }
}
