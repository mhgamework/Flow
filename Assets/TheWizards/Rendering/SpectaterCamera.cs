using System;
using UnityEngine;


namespace MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics
{
    /// <summary>
    /// Responsible for interpreting user input for a spectator camera
    /// Responsible for construction camera matrices for the spectator camera
    /// </summary>
    public class SpectaterCamera : MonoBehaviour //: ICamera
    {
        Matrix4x4 view;
        Matrix4x4 projection;
        Matrix4x4 viewProjection;
        Matrix4x4 viewInverse;
        bool mChanged = true;



        public float angleX;
        public float angleY;
        public float angleZ;
        private Vector3 vLookAt;
        private Vector3 vLookDir;
        private Vector3 vLookEye;
        private Vector3 vLookUp;


        private float nearClip;
        public float NearClip
        {
            get { return nearClip; }
            set { nearClip = value; calculateProjection(); }
        }

        private float farClip;
        public float FarClip
        {
            get { return farClip; }
            set { farClip = value; calculateProjection(); }
        }

        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private bool enableUserInput;
        public bool EnableUserInput
        {
            get { return enableUserInput; }
            set { enableUserInput = value; }
        }

        private float fieldOfView;
        public float FieldOfView
        {
            get { return fieldOfView; }
            set
            {
                fieldOfView = value;
                calculateProjection();
            }
        }

        private float aspectRatio;
        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                aspectRatio = value;
                calculateProjection();
            }
        }


        public SpectaterCamera(float nearPlane, float farPlane)
        {
            enabled = true;
            nearClip = nearPlane;
            farClip = farPlane;
            fieldOfView = 45 * Mathf.Deg2Rad;
            aspectRatio = 4 / 3f;
            view = Matrix4x4.identity;//Matrix.CreateLookAt(new Vector3(0, 0, -4000), Vector3.Zero, Vector3.Up);
            calculateProjection();
            CameraUp = Vector3.up;
            CameraDirection = Vector3.forward;
            CameraPosition = new Vector3(0.1f, 0.1f, 0.1f);
            AngleVertical = 0;
            AngleHorizontal = 0;
            AngleRoll = 0;
            enableUserInput = true;
            MovementSpeed = 10;





            UpdateCameraInfo();

        }
        private void calculateProjection()
        {
            projection = Matrix4x4.Perspective(fieldOfView,
        //4 / 3F, 1.23456f, 10000.0f );
        aspectRatio, nearClip, farClip);

        }


        public SpectaterCamera()
            : this(0.1f, 400.0f)
        {
            mChanged = true;
        }




        public Vector3 Positie
        {
            get { return CameraPosition; }
            set { CameraPosition = value; }
        }

        private Vector3 _snelheid;



        public float MovementSpeed { get; set; }

        public Vector3 Snelheid
        {
            get { return _snelheid; }
            set { _snelheid = value; }
        }


        public void UpdateCameraInfo()
        {
            if (mChanged)
            {
                //mChanged = false;
                //view = Matrix4x4.LookAtRH(this.vLookEye, this.vLookAt, this.vLookUp);
                ////_cameraInfo.Frustum = new BoundingFrustum( _cameraInfo.ViewMatrix * _cameraInfo.ProjectionMatrix );

                //viewProjection = view * projection;
                //viewInverse = Matrix.Invert(view);

            }
        }



        public Vector3 CameraPosition
        {
            get
            {
                return this.vLookEye;
            }
            set
            {
                this.vLookEye = value;
                this.mChanged = true;
                /*if (this.curStyle == CameraStyle.PositionBased)
                {*/
                //this.vLookAt = Vector3.Add(this.vLookEye, this.vLookDir);
                //}
            }
        }
        /// <summary>
        /// TODO: WARNING setter is bugged!
        /// </summary>
        public Vector3 CameraDirection
        {
            get
            {
                return this.vLookDir;
            }
            set
            {

                //setCameraDirectionInternal(value);

                //Vector3 groundProj = value;
                //groundProj.Y = 0;
                //groundProj.Normalize();

                //AngleVertical = (float)Math.Acos(Vector3.Dot(groundProj, value));

                //if (value.Y < 0) AngleVertical = -AngleVertical;

                //Vector3 source = new Vector3(0f, 0f, 1f);
                //float horizontal = (float)Math.Acos(Vector3.Dot(groundProj, source));
                //if (groundProj.X > 0) horizontal = -horizontal; // This is inverted since the angles are inverted in the angle properties to matrix conversion?
                //AngleHorizontal = horizontal;

                //AngleRoll = 0;



            }
        }
        private void setCameraDirectionInternal(Vector3 value)
        {
            this.vLookDir = value;
            this.mChanged = true;
            //   this.vLookAt = Vector3.Add(this.vLookEye, this.vLookDir);
        }

        public Vector3 CameraUp
        {
            get
            {
                return this.vLookUp;
            }
            set
            {
                this.vLookUp = value;
                this.mChanged = true;
                /*if (this.curStyle == CameraStyle.PositionBased)
                {*/
                this.mChanged = true;
                //}
            }
        }





        public float mouseSensitivityX = 10;
        public float mouseSensitivityY = 10;
        public bool cursorToggleAllowed = true;
        public KeyCode cursorToggleButton = KeyCode.Escape;
        private bool togglePressed = false;


        public float AngleVertical
        {
            get
            {
                return this.angleX;
            }
            set
            {
                if (value != this.angleX)
                {
                    this.angleX = value;
                    this.mChanged = true;
                    /*if (this.curStyle == CameraStyle.PositionBased)
                    {*/
                    //    Matrix sourceMatrix = Matrix.RotationYawPitchRoll(-this.angleY, -this.angleX, -this.angleZ);
                    Vector3 source = new Vector3(0f, 0f, 1f);
                    //     setCameraDirectionInternal(Vector3.TransformNormal(source, sourceMatrix));
                    source = new Vector3(0f, 1f, 0f);
                    //       this.CameraUp = Vector3.TransformNormal(source, sourceMatrix);
                    //}
                }
            }
        }
        public float AngleRoll
        {
            get
            {
                return this.angleZ;
            }
            set
            {
                this.angleZ = value;
                this.mChanged = true;
                /*if (this.curStyle == CameraStyle.PositionBased)
                {*/
                //   Matrix sourceMatrix = Matrix.RotationYawPitchRoll(-this.angleY, -this.angleX, -this.angleZ);
                Vector3 source = new Vector3(0f, 0f, 1f);
                //   setCameraDirectionInternal(Vector3.TransformNormal(source, sourceMatrix));
                source = new Vector3(0f, 1f, 0f);
                //   this.CameraUp = Vector3.TransformNormal(source, sourceMatrix);
                //}
            }
        }
        public float AngleHorizontal
        {
            get
            {
                return this.angleY;
            }
            set
            {
                if (value != this.angleY)
                {
                    this.angleY = value;
                    this.mChanged = true;
                    /*if (this.curStyle == CameraStyle.PositionBased)
                    {*/
                    //  Matrix sourceMatrix = Matrix.RotationYawPitchRoll(-this.angleY, -this.angleX, -this.angleZ);
                    Vector3 source = new Vector3(0f, 0f, 1f);
                    //    setCameraDirectionInternal(Vector3.TransformNormal(source, sourceMatrix));
                    source = new Vector3(0f, 1f, 0f);
                    //    this.CameraUp = Vector3.TransformNormal(source, sourceMatrix);
                    //}
                }
            }
        }




        private void OnEnable()
        {
            if (cursorToggleAllowed)
            {
                togglePressed = true;
                //Screen.lockCursor = true;
                //Cursor.visible = false;
            }
        }

        public void Update()
        {
            if (!Enabled) return;
            updateMouseLockButton();

            if (!togglePressed) return;
            processUserInput(Time.deltaTime);

            UpdateCameraInfo();

        }

        private void updateMouseLockButton()
        {
            if (cursorToggleAllowed && Input.GetKeyDown(cursorToggleButton))
            {
                togglePressed = !togglePressed;
            }
            Screen.lockCursor = togglePressed;
            Cursor.visible = !togglePressed;
        }

        private void processUserInput(float elapsed)
        {
            if (!enableUserInput) return;
            Vector3 vSnelheid = new Vector3();

            if (Input.GetKey(KeyCode.S)) { vSnelheid += Vector3.back; }
            if (Input.GetKey(KeyCode.Z)) { vSnelheid += Vector3.forward; }
            if (Input.GetKey(KeyCode.Q)) { vSnelheid += Vector3.left; }
            if (Input.GetKey(KeyCode.D)) { vSnelheid += Vector3.right; }
            if (Input.GetKey(KeyCode.Space)) { vSnelheid += Vector3.up; }
            if (Input.GetKey(KeyCode.LeftControl)) { vSnelheid += Vector3.down; }


            vSnelheid = Quaternion.Euler(AngleVertical * Mathf.Rad2Deg, AngleHorizontal * Mathf.Rad2Deg, AngleRoll * Mathf.Rad2Deg) * vSnelheid;

            //vSnelheid = Vector3.TransformCoordinate(vSnelheid, Matrix.RotationYawPitchRoll(-AngleHorizontal, -AngleVertical, -AngleRoll));

            if (vSnelheid.magnitude != 0) vSnelheid.Normalize();

            if (Input.GetKey(KeyCode.T))
            {
                vSnelheid *= 30;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                vSnelheid *= 5;
            }

            vSnelheid *= MovementSpeed;


            Snelheid = vSnelheid;


            //Positie += Snelheid * elapsed;
            transform.position += Snelheid * elapsed;
            CameraPosition = transform.position;
            //CameraPosition = Positie;



            ProcessMouseInput(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            transform.localRotation = Quaternion.Euler(AngleVertical * Mathf.Rad2Deg, AngleHorizontal * Mathf.Rad2Deg, 0);
        }

        public void ProcessMouseInput(float mouseRelativeX, float mouseRelativeY)
        {
            mouseRelativeX *= mouseSensitivityX;
            mouseRelativeY *= mouseSensitivityY;
            //   mouseRelativeX *= Mathf.Deg2Rad; // dont know if this makes sense
            //    mouseRelativeY *= Mathf.Deg2Rad; // dont know if this makes sense
            if (mouseRelativeX != 0)
            {
                AngleHorizontal += Mathf.Deg2Rad * (mouseRelativeX);
                if (AngleHorizontal > Mathf.PI * 2)
                {
                    AngleHorizontal -= Mathf.PI * 2;
                }
                if (AngleHorizontal < 0)
                {
                    AngleHorizontal += Mathf.PI * 2;
                }
            }
            if (mouseRelativeY != 0)
            {
                AngleVertical = Mathf.Clamp(AngleVertical - Mathf.Deg2Rad * (mouseRelativeY), -Mathf.PI / 2, Mathf.PI / 2);
            }
        }


    }
}
