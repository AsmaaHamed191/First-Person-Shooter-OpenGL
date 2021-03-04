using GlmNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
namespace Graphics._3D_Models
{
    public class md2_t
    {

        public int ident;              // magic number. must be equal to "IDP2"
        public int version;            // md2 version. must be equal to 8

        public int skinwidth;          // width of the texture
        public int skinheight;         // height of the texture
        public int framesize;          // size of one frame in bytes

        public int num_skins;          // number of textures
        public int num_xyz;            // number of vertices
        public int num_st;             // number of texture coordinates
        public int num_tris;           // number of triangles
        public int num_glcmds;         // number of opengl commands
        public int num_frames;         // total number of frames

        public int ofs_skins;          // offset to skin names (64 bytes each)
        public int ofs_st;             // offset to s-t texture coordinates
        public int ofs_tris;           // offset to triangles
        public int ofs_frames;         // offset to frame data
        public int ofs_glcmds;         // offset to opengl commands
        public int ofs_end;            // offset to end of file

    }

    class animState_t
    {

        public int startframe;             // first frame
        public int endframe;               // last frame
        public int fps;                    // frame per second for this animation

        public float curr_time;                // current time
        public float old_time;             // old time
        public float interpol;             // percent of interpolation

        public animType_LOL type;                   // animation type

        public int curr_frame;             // current frame
        public int next_frame;             // next frame

    }
    class vertex_t
    {

        public byte[] v;                // compressed vertex (x, y, z) coordinates
        public byte lightnormalindex;    // index to a normal vector for the lighting

    }

    class frame_t
    {

        public float[] scale;       // scale values
        public float[] translate;   // translation vector
        public char[] name;       // frame name
        public List<vertex_t> verts;       // first vertex of this frame

    }

    enum animType_LOL
    {
        STAND,
        ATTACK1,
        ATTACK2,
        RUN,
        DEATH,
        SPELL1,
        SPELL2,
        MAX_ANIMATIONS

    }
    class anim_t
    {

        public int first_frame;            // first frame of the animation
        public int last_frame;             // number of frames
        public int fps;                    // number of frames per second

    }
    class md2LOL
    {
        public mat4 TranslationMatrix;
        public mat4 rotationMatrix;
        vec3 EnemyPos;
        public mat4 scaleMatrix;
        List<string> LOLMD2AnimationNames;
        List<anim_t> animlist;
        List<List<vec3>> vVertices;
        List<List<vec3>> vNormals;
        List<List<int>> vNormalsindex;
        List<vec2> UVData;
        List<string> skins;
        List<int> indices;
        List<vec3> mVertices;
        public Model m;
        public animState_t animSt;
        public float AnimationSpeed;
        public bool Loop;
        float[] vs, ns;
        float oldframe;
        float currframe;
        public md2LOL(string fileName)
        {
            animlist = new List<anim_t>();
            anim_t a1 = new anim_t();
            a1.first_frame = 0; a1.last_frame = 30; a1.fps = 30;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 31; a1.last_frame = 60; a1.fps = 30;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 61; a1.last_frame = 90; a1.fps = 30;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 91; a1.last_frame = 120; a1.fps = 30;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 121; a1.last_frame = 150; a1.fps = 30;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 151; a1.last_frame = 178; a1.fps = 30;
            animlist.Add(a1);
            a1 = new anim_t();
            a1.first_frame = 181; a1.last_frame = 210; a1.fps = 30;
            animlist.Add(a1);

            LOLMD2AnimationNames = new List<string>();
            LOLMD2AnimationNames.Add("Stand");
            LOLMD2AnimationNames.Add("Attack1");
            LOLMD2AnimationNames.Add("Attack2");
            LOLMD2AnimationNames.Add("Run");
            LOLMD2AnimationNames.Add("Death");
            LOLMD2AnimationNames.Add("Spell1");
            LOLMD2AnimationNames.Add("Spell2");

            TranslationMatrix = new mat4(1);
            rotationMatrix = new mat4(1);
            scaleMatrix = new mat4(1);

            LoadModel(fileName);
        }

        void LoadModel(string path)
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
                uv.x = ((float)br.ReadInt16())/header.skinwidth;
                uv.y = ((float)br.ReadInt16())/header.skinheight;
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
                for (int j = 0; j < indices.Count-3; j+=3)
                {

                    vec3 n1 = vVertices[i][indices[j + 1]] - vVertices[i][indices[j]];
                    vec3 n2 = vVertices[i][indices[j + 2]] - vVertices[i][indices[j]];
                    vec3 normal = glm.cross(n1, n2);
                    vNormals[i][indices[j]] = normal;
                    vNormals[i][indices[j+1]] = normal;
                    vNormals[i][indices[j+2]] = normal;
                }

            }
            Texture tex = null;
            if (skins.Count > 1)
            {
                tex = new Texture(skins[1], 20,false);
            }
            else
            {
                FileInfo fi = new FileInfo(path);
                string name = fi.Name;
                int index = fi.FullName.IndexOf(name,StringComparison.CurrentCulture);
                string filePath = "";
                for (int i = 0; i < index; i++)
                {
                    filePath += fi.FullName[i];
                }
                string[] extensions = { "jpg", "jpeg", "png", "bmp" };
                for (int i = 0; i < 4; i++)
                {
                    string stry = filePath + name.Split('.')[0] + "." + extensions[i];
                    tex = new Texture(stry, 20, false);
                    if (tex.width > 0)
                        break;
                }
            }

            mVertices = vVertices[0];


            m = new Model();
            m.indices = indices;
            
            m.texture = tex;
            m.vertices = vVertices[0];
            

            m.uvCoordinates = UVData;
            m.normals = vNormals[0];
            m.Initialize();

            vs = new float[m.vertices.Count * 3];
            ns = new float[m.vertices.Count * 3];
        }
        public void Draw(int matID)
        {
            List<mat4> modelmatrices = new List<mat4>() {scaleMatrix,rotationMatrix,TranslationMatrix };
            m.transformationMatrix = MathHelper.MultiplyMatrices(modelmatrices);
            m.Draw(matID);
        }

        public List<vec3> GetCurrentVertices(animState_t animState)
        {
            return vVertices[animState.curr_frame];
        }
        

        public void StartAnimation(animType_LOL type)
        {
            animState_t res = new animState_t();
            res.startframe = animlist[(int)type].first_frame;
            res.endframe = animlist[(int)type].last_frame;
            res.curr_frame = animlist[(int)type].first_frame;
            res.next_frame = animlist[(int)type].first_frame + 1;

            res.fps = animlist[(int)type].fps;
            res.type = type;

            res.curr_time = 0.0f;
            res.old_time = 0.0f;

            res.interpol = 0.0f;

            animSt  = res;
            AnimationSpeed = 0.01f;
            Loop = true;
            currframe = animSt.curr_frame;
            oldframe = currframe;
            
        }

        public void UpdateExportedAnimation(float AnimationSpeed)
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
       
        public void move (vec3 dir)
        {
            
            dir.y = 0;
            EnemyPos =  dir * 0.05f;          
            TranslationMatrix = glm.translate(new mat4(1), EnemyPos);

            /*Stopwatch waiting = new Stopwatch();
            //TimeSpan timeSpan = new TimeSpan(0, 0, 5);
            //for (int i = 0; i < 30; i++)
            //{
 
            //    EnemyPos += dir * i/100;
            //    TranslationMatrix = glm.translate(new mat4(1), EnemyPos);
            //    System.Threading.Thread.Sleep(5000);
            //}
            */

        }
        public void update_enemy (vec3 cam_pos)
        {
            double red = 100, d = 200;
            vec3 dir = cam_pos - EnemyPos;
            double pos = Math.Sqrt(Math.Pow(dir.x, 2) + Math.Pow(dir.y, 2) + Math.Pow(dir.z, 2));
            dir = glm.normalize(dir);

            if (pos <= d)
            {
                move(dir);
            }
            else if (pos <= red)
            {
              //  animType_LOL(animType.ATTACK); 
            }
        }
        public void UpdateVertices()
        {
            int q = 0;
            if (oldframe != currframe)
            {
                for (int i = 0; i < m.vertices.Count; i++)
                {
                    if (animSt.interpol >= 0)
                    {
                        vec3 newVertex = vVertices[animSt.curr_frame][i] + (vVertices[animSt.next_frame][i] - vVertices[animSt.curr_frame][i]) * animSt.interpol;
                        vec3 newNormal = vNormals[animSt.curr_frame][i] + (vNormals[animSt.next_frame][i] - vNormals[animSt.curr_frame][i]) * animSt.interpol;
                        ns[q] = newNormal.x;
                        vs[q++] = newVertex.x;
                        ns[q] = newNormal.y;
                        vs[q++] = newVertex.y;
                        ns[q] = newNormal.z;
                        vs[q++] = newVertex.z;

                    }
                }
                m.Update(vs,ns);
            }
        }
    }
}
