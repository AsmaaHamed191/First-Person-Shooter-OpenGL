using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using Assimp;
using GlmNet;

namespace Graphics
{
    class GPU
    {
        static public uint GenerateBuffer(float[] data)
        {
            uint BufferID = 0;
            uint[] vbo = { 0 };
            Gl.glGenBuffers(1, vbo);
            BufferID = vbo[0];
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, BufferID);
            GCHandle Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr Ptr = Handle.AddrOfPinnedObject();
            var size = Marshal.SizeOf(typeof(float)) * data.Length;
            Gl.glBufferData(Gl.GL_ARRAY_BUFFER, (IntPtr)size, Ptr, Gl.GL_STATIC_DRAW);
            Handle.Free();
            return BufferID;
        }

        static public uint GenerateBuffer(List<vec3> vec)
        {
            uint BufferID = 0;
            uint[] vbo = { 0 };
            Gl.glGenBuffers(1, vbo);
            BufferID = vbo[0];
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, BufferID);
            float[] data = new float[vec.Count * 3];
            int i = 0;
            foreach (var v in vec)
            {
                data[i++] = v.x;
                data[i++] = v.y;
                data[i++] = v.z;
            }
            GCHandle Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr Ptr = Handle.AddrOfPinnedObject();
            var size = Marshal.SizeOf(typeof(float))* data.Length;
            Gl.glBufferData(Gl.GL_ARRAY_BUFFER, (IntPtr)size, Ptr, Gl.GL_STATIC_DRAW);
            Handle.Free();
            return BufferID;
        }

        static public uint GenerateBuffer(List<vec2> vec)
        {
            uint BufferID = 0;
            uint[] vbo = { 0 };
            Gl.glGenBuffers(1, vbo);
            BufferID = vbo[0];
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, BufferID);
            float[] data = new float[vec.Count * 2];
            int i = 0;
            foreach (var v in vec)
            {
                data[i++] = v.x;
                data[i++] = v.y;
            }
            GCHandle Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr Ptr = Handle.AddrOfPinnedObject();
            var size = Marshal.SizeOf(typeof(float)) * data.Length;
            Gl.glBufferData(Gl.GL_ARRAY_BUFFER, (IntPtr)size, Ptr, Gl.GL_STATIC_DRAW);
            Handle.Free();
            return BufferID;
        }

        static public uint GenerateElementBuffer(ushort[] data)
        {
            uint BufferID = 0;
            uint[] vbo = { 0 };
            Gl.glGenBuffers(1, vbo);
            BufferID = vbo[0];
            Gl.glBindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, BufferID);
            GCHandle Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr Ptr = Handle.AddrOfPinnedObject();
            var size = Marshal.SizeOf(typeof(ushort)) * data.Length;
            Gl.glBufferData(Gl.GL_ELEMENT_ARRAY_BUFFER, (IntPtr)size, Ptr, Gl.GL_STATIC_DRAW);
            Handle.Free();
            return BufferID;
        }

        static public uint GenerateElementBuffer(List<int> indices)
        {
            uint BufferID = 0;
            uint[] vbo = { 0 };
            Gl.glGenBuffers(1, vbo);
            BufferID = vbo[0];
            Gl.glBindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, BufferID);
            ushort[] data = new ushort[indices.Count];
            for (int i = 0; i < indices.Count; i++)
            {
                data[i] = (ushort)indices[i];
            }

            GCHandle Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr Ptr = Handle.AddrOfPinnedObject();
            var size = Marshal.SizeOf(typeof(ushort)) * data.Length;
            Gl.glBufferData(Gl.GL_ELEMENT_ARRAY_BUFFER, (IntPtr)size, Ptr, Gl.GL_STATIC_DRAW);
            Handle.Free();
            return BufferID;
        }

        static public void UpdateBuffer(List<vec3> vec,uint BufferID)
        {
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, BufferID);
            float[] data = new float[vec.Count * 3];
            int i = 0;
            for (int j = 0; j < vec.Count; j++)
            {
                data[i++] = vec[j].x;
                data[i++] = vec[j].y;
                data[i++] = vec[j].z;
            }
            GCHandle Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr Ptr = Handle.AddrOfPinnedObject();
            var size = Marshal.SizeOf(typeof(float)) * data.Length;
            Gl.glBufferData(Gl.GL_ARRAY_BUFFER, (IntPtr)size, Ptr, Gl.GL_STATIC_DRAW);
            //Gl.glBufferSubData(Gl.GL_ARRAY_BUFFER,(IntPtr)0 ,(IntPtr)size, Ptr);
            Handle.Free();
        }
        static public void UpdateBuffer(List<vec2> vec, uint BufferID)
        {
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, BufferID);
            float[] data = new float[vec.Count * 2];
            int i = 0;
            for (int j = 0; j < vec.Count; j++)
            {
                data[i++] = vec[j].x;
                data[i++] = vec[j].y;
            }
            GCHandle Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr Ptr = Handle.AddrOfPinnedObject();
            var size = Marshal.SizeOf(typeof(float)) * data.Length;
            Gl.glBufferData(Gl.GL_ARRAY_BUFFER, (IntPtr)size, Ptr, Gl.GL_STATIC_DRAW);
            Handle.Free();
        }

        static public void UpdateBuffer(float[] data, uint BufferID)
        {
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, BufferID);
            GCHandle Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr Ptr = Handle.AddrOfPinnedObject();
            var size = Marshal.SizeOf(typeof(float)) * data.Length;
            Gl.glBufferData(Gl.GL_ARRAY_BUFFER, (IntPtr)size, Ptr, Gl.GL_STATIC_DRAW);
            //Gl.glBufferSubData(Gl.GL_ARRAY_BUFFER,(IntPtr)0 ,(IntPtr)size, Ptr);
            Handle.Free();
        }
        static public void BindBuffer(uint bufferID)
        {
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, bufferID);
        }
        
    }
}
