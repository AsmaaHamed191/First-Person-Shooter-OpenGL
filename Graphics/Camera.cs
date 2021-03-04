using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    class Camera
    {
        public float mAngleX = 0;
        public float mAngleY = 0;
        public vec3 mDirection;
        public vec3 mPosition;
        public vec3 mCenter;
        public vec3 mRight;
        vec3 mUp;
        mat4 mViewMatrix;
        mat4 mProjectionMatrix;
        public Camera()
        {
            Reset(0, 1, 5, 0, 0, 0, 0, 1, 0);
            SetProjectionMatrix(45, 4 / 3, 0.1f, 1000);
        }

        public void set_cam_pos()
        {
            Reset(0, 1, 5, 0, 0, 0, 0, 1, 0);
            SetProjectionMatrix(45, 4 / 3, 0.1f, 1000);
        }
        public vec3 GetLookDirection()
        {
            return mDirection;
        }

        public mat4 GetViewMatrix()
        {
            return mViewMatrix;
        }

        public mat4 GetProjectionMatrix()
        {
            return mProjectionMatrix;
        }
        public vec3 GetCameraPosition()
        {
            return mPosition;
        }

        public vec3 GetCameraTarget()
        {
            return mCenter;
        }
        public void Reset(float eyeX, float eyeY, float eyeZ, float centerX, float centerY, float centerZ, float upX, float upY, float upZ)
        {
            vec3 eyePos = new vec3(eyeX, eyeY, eyeZ);
            mCenter = new vec3(centerX, centerY, centerZ);
            vec3 upVec = new vec3(upX, upY, upZ);

            // camera position 
            mPosition = eyePos;
            // Camera direction = N = Center - Position 
            mDirection = mCenter - mPosition;
            // Vector U >> Right vector = N * V >> Direction * Up vector 
            mRight = glm.cross(mDirection, upVec);
            // Up vector for camera = Up vetor paramemter 
            mUp = upVec;
            // Normalize three vectors to get values from (1,-1)  Unit vector 
            mUp = glm.normalize(mUp);
            mRight = glm.normalize(mRight);
            mDirection = glm.normalize(mDirection);
            // Generate Viewing matrix Using Camera position , Camera Center , UP vector 
            mViewMatrix = glm.lookAt(mPosition, mCenter, mUp);
        }
        public void SetHeight(float y)
        {
            mCenter.y = y;
        }
        public void UpdateViewMatrix()
        {

            //  N =  ( Rotation matrix  Y * ROtation matrix X  * N ) This equation equavelint 
            mDirection = new vec3((float)(-Math.Cos(mAngleY) * Math.Sin(mAngleX))
                , (float)(Math.Sin(mAngleY))
                , (float)(-Math.Cos(mAngleY) * Math.Cos(mAngleX)));

            // U Direction  Right direction , from Normal to Initial UP vector 
            mRight = glm.cross(mDirection, new vec3(0, 1, 0));
            // UP direction 
            mUp = glm.cross(mRight, mDirection);


            mPosition = mCenter - (mDirection) * 6;
            // Up date viewing matrix 
            mViewMatrix = glm.lookAt(mPosition, mCenter, mUp);
        }
        public void SetProjectionMatrix(float FOV, float aspectRatio, float near, float far)
        {
            mProjectionMatrix = glm.perspective(FOV, aspectRatio, near, far);
        }

        // Rotation around X , Up date Angle X
        public void Yaw(float angleDegrees)
        {
            mAngleX += angleDegrees;
        }
        // Rotation around Y , Up date Angle Y
        public void Pitch(float angleDegrees)
        {
            //mAngleY += angleDegrees;
        }

        public void Walk(float dist)
        {
            mCenter += dist * mDirection;
        }
        public void Strafe(float dist)
        {
            mCenter += dist * mRight;
        }
        public void Fly(float dist)
        {
            mCenter += dist * mUp;
        }
    }
}
