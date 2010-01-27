using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.DirectSound;
using Microsoft.Win32;
using System.IO;
using System.Reflection;


namespace netPongClientNG
{

    public partial class netPongClientGameForm : Form
    {        
        private Microsoft.DirectX.Direct3D.Device m_direct3DDevice = null;
        private Microsoft.DirectX.DirectSound.Device m_directSoundDevice = null;
        private PresentParameters m_presentParams = null;
        private Microsoft.DirectX.DirectInput.Device m_Keyb = null;
        private Mesh m_hPaddleMesh = null;
        private Mesh m_hBallMesh = null;
        private Microsoft.DirectX.Direct3D.Font m_fFps;
        private Vector3 m_worldRotateAngle = new Vector3(0, 0, 0);
        private Matrix m_worldRotate = Matrix.Identity;
        private Vector3 m_worldTranslateVector = new Vector3(0, 0, 0);
        private Vector3 m_camPosition = new Vector3(0, 75, 0f);
        private Vector3 m_camDirection = new Vector3(0, -1, -0.001f);
        private Vector3 m_camUpVector = new Vector3(0, 1, 0);
        private long m_nFrmsPerSec = 0;
        private long m_nCalcFrmsPerSec = 0;
        private DateTime m_tLastFPSCalc = DateTime.Now;
        private DateTime m_tLastBallPosCalc = DateTime.Now;
        private Vector3 m_ballPos = new Vector3(0, 0, 0.0f);
        private Vector3 m_ballDirSpeed = new Vector3(0.0f, 0.0f, 0.05f);
        private Texture m_paddleTexture;
        private Material m_paddleMaterial;
        private Material m_ballMaterial;
        private float m_paddlePlayerXPos = 0.0f;
        private float m_paddleOpponentXPos = 0.0f;
        private Vector3 m_ballMinBoundOrg = new Vector3(0, 0, 0);
        private Vector3 m_ballMaxBoundOrg = new Vector3(0, 0, 0);
        private Vector3 m_paddlePlayerMinBoundOrg = new Vector3(0, 0, 0);
        private Vector3 m_paddlePlayerMaxBoundOrg = new Vector3(0, 0, 0);
        private SecondaryBuffer m_hitPaddleSoundBuffer = null;
        private SecondaryBuffer m_lostBallPlayerSoundBuffer = null;
        private bool m_bIsGamePaused = false;
        private Mesh m_gameFieldPane = null;
        private Material m_gameFieldMaterial;
        private Texture m_gameFieldTexture;
        private bool m_bdeviceLost = false;
        private object m_ballPosLock = new object();
        private static string strInvPlayerName = "";

        public string strInvitationName
        {
            get { return strInvPlayerName; }
            set { strInvPlayerName = value; if (this.InvokeRequired) this.Invoke(new MethodInvoker(invitationRecieved)); else invitationRecieved(); }
        }
        
        public netPongClientGameForm()
        {
            InitializeComponent();
            InitGame();
        }

        public void InitGame()
        {
            InitDXDevice();
            InitSound();
            InitCamera();
            InitLights();
            InitMeshes();
            InitMaterials();
            InitTextures();
            InitFont();
            InitKeyboard();

            m_tLastBallPosCalc = DateTime.Now;

            Show();
        }

        private void InitDXDevice()
        {
            try
            {
                m_presentParams = new PresentParameters();
                SetPresentParameters(ref m_presentParams);

                int nAdapterID = 0;
                RegistryKey adapKey = Registry.CurrentUser.OpenSubKey("Software\\netPongClient\\Adapter");
                if (adapKey != null)
                    nAdapterID = (int)adapKey.GetValue("ID", 0);

                m_direct3DDevice = new Microsoft.DirectX.Direct3D.Device(
                    nAdapterID,
                    Microsoft.DirectX.Direct3D.DeviceType.Hardware,
                    this,
                    CreateFlags.HardwareVertexProcessing,
                    m_presentParams);

                // Hook the DeviceReset event so OnDeviceReset will get called every
                // time we call device.Reset()
                m_direct3DDevice.DeviceReset += new EventHandler(this.OnDeviceReset);


                // Similarly, OnDeviceLost will get called every time we call 
                // device.Reset(). The difference is that DeviceLost gets called
                // earlier, giving us a chance to do the cleanup that needs to 
                // occur before we can call Reset() successfully
                m_direct3DDevice.DeviceLost += new EventHandler(this.OnDeviceLost);


                m_direct3DDevice.RenderState.ZBufferEnable = true;

                //m_direct3DDevice.SetSamplerState(0, SamplerStageStates.AddressU, (int)TextureAddress.Clamp);
                //m_direct3DDevice.SetSamplerState(1, SamplerStageStates.AddressV, (int)TextureAddress.Wrap);
                //m_direct3DDevice.SetSamplerState(2, SamplerStageStates.AddressW, (int)TextureAddress.Wrap);
            }
            catch (DirectXException e)
            {
                MessageBox.Show(null,
                "Error intializing graphics: "
                + e.Message, "Error");
                Close();
            }
        }

        private void SetPresentParameters(ref PresentParameters presentParams)
        {
            try
            {
                RegistryKey adapKey = Registry.CurrentUser.OpenSubKey("Software\\netPongClient\\Adapter");
                if (adapKey != null)
                    presentParams.Windowed = !(Convert.ToBoolean(adapKey.GetValue("Fullscreen", 1)));
                else
                    presentParams.Windowed = true;

                int nSavedWidth = (int)adapKey.GetValue("Width", 0); ;
                int nSavedHeight = (int)adapKey.GetValue("Height", 0);
                int nSavedFormat = (int)adapKey.GetValue("Format", 0);

                presentParams.BackBufferHeight = nSavedHeight;
                presentParams.BackBufferWidth = nSavedWidth;
                presentParams.BackBufferFormat = (Format)nSavedFormat;
                presentParams.BackBufferCount = 1;
                presentParams.EnableAutoDepthStencil = true;
                presentParams.AutoDepthStencilFormat = DepthFormat.D16;

                if (presentParams.Windowed)
                {
                    SetClientSizeCore(nSavedWidth, nSavedHeight);
                }
            }
            catch (Exception)
            {
            }

            presentParams.SwapEffect = SwapEffect.Discard;
        }

        /*
        protected override void OnPaint(PaintEventArgs pea)
        {
        }
        */

        private int Render()
        {
            if (m_direct3DDevice == null)
                return 1;

            if (m_bdeviceLost)
            {
                // Try to get the device back
                AttemptRecovery();
            }
            // If we couldn't get the device back, don't try to render
            if (m_bdeviceLost)
            {
                return 0;
            }


            //set frames per second
            m_nCalcFrmsPerSec++;
            TimeSpan tDif = DateTime.Now.Subtract(m_tLastFPSCalc);

            if (tDif.TotalSeconds >= 1)
            {
                m_nFrmsPerSec = m_nCalcFrmsPerSec;
                m_nCalcFrmsPerSec = 0;
                m_tLastFPSCalc = DateTime.Now;
            }

            try
            {
                m_direct3DDevice.BeginScene();

                m_direct3DDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer,
                   System.Drawing.Color.Blue,
                   1.0f, 0);

                //drawing fps
                int nCount = m_fFps.DrawText(null, m_nFrmsPerSec + " fps", new Point(10, 0), Color.Red);

                RenderGameField();
                RenderPaddles();
                RenderBall();

                //set camera position
                m_direct3DDevice.Transform.View = Matrix.LookAtLH(m_camPosition, m_camDirection, m_camUpVector);

                m_direct3DDevice.EndScene();

                try
                {
                    m_direct3DDevice.Present();
                }
                catch (DeviceLostException)
                {
                    m_bdeviceLost = true;
                }
            }
            catch (Exception)
            {
                try
                {
                    m_direct3DDevice.EndScene();
                }
                catch (Exception)
                {
                }
                return -1;
            }

            return 0;
        }

        private void InitCamera()
        {
            if (m_direct3DDevice == null)
                return;

            m_direct3DDevice.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, this.Width / this.Height, 0.3f, 500f);
            m_direct3DDevice.Transform.View = Matrix.LookAtLH(m_camPosition, m_camDirection, m_camUpVector);
        }

        private void InitKeyboard()
        {
            m_Keyb = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);
            m_Keyb.SetCooperativeLevel(this, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
            m_Keyb.Acquire();
        }

        private void InitMeshes()
        {
            // get a reference to the current assembly
            Assembly a = Assembly.GetExecutingAssembly();

            // get a list of resource names from the manifest
            string[] resNames = a.GetManifestResourceNames();

            foreach (string s in resNames)
            {
                if (s.EndsWith("simple_paddle.x"))
                {
                    // attach stream to the resource in the manifest
                    Stream xStream = xStream = a.GetManifestResourceStream(s);
                    if (null != xStream)
                    {
                        try
                        {
                            m_hPaddleMesh = Mesh.FromStream(xStream, MeshFlags.Managed, m_direct3DDevice);
                            xStream.Close();
                            xStream = null;


                            GraphicsStream g = m_hBallMesh.LockVertexBuffer(LockFlags.ReadOnly);
                            Geometry.ComputeBoundingBox(g, m_hBallMesh.NumberVertices, m_hBallMesh.VertexFormat, out m_paddlePlayerMinBoundOrg, out m_paddlePlayerMaxBoundOrg);
                            m_hBallMesh.UnlockVertexBuffer();
                            g.Close();
                            g = null;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                if (s.EndsWith("simple_ball.x"))
                {
                    // attach stream to the resource in the manifest
                    Stream xStream = xStream = a.GetManifestResourceStream(s);
                    if (null != xStream)
                    {
                        try
                        {
                            m_hBallMesh = Mesh.FromStream(xStream, MeshFlags.Managed, m_direct3DDevice);
                            xStream.Close();
                            xStream = null;
                            GraphicsStream g = m_hBallMesh.LockVertexBuffer(LockFlags.ReadOnly);
                            Geometry.ComputeBoundingBox(g, m_hBallMesh.NumberVertices, m_hBallMesh.VertexFormat, out m_ballMinBoundOrg, out m_ballMaxBoundOrg);
                            m_hBallMesh.UnlockVertexBuffer();
                            g.Close();
                            g = null;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            try
            {
                m_gameFieldPane = Mesh.Box(m_direct3DDevice, 60, 10, 50);
            }
            catch (Exception)
            {
            }
        }

        private void ReadKeyboard()
        {
            KeyboardState keys = m_Keyb.GetCurrentKeyboardState();

            //world rotation x
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_ROTATE_X_PLUS"])])
            {
                m_worldRotateAngle.X += 0.03f;
                m_worldRotate = (Matrix.RotationX(m_worldRotateAngle.X) * Matrix.RotationY(m_worldRotateAngle.Y) * Matrix.RotationZ(m_worldRotateAngle.Z));
            }
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_ROTATE_X_MINUS"])])
            {
                m_worldRotateAngle.X -= 0.03f;
                m_worldRotate = (Matrix.RotationX(m_worldRotateAngle.X) * Matrix.RotationY(m_worldRotateAngle.Y) * Matrix.RotationZ(m_worldRotateAngle.Z));
            }
            //world rotation y
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_ROTATE_Y_PLUS"])])
            {
                m_worldRotateAngle.Y -= 0.03f;
                m_worldRotate = (Matrix.RotationX(m_worldRotateAngle.X) * Matrix.RotationY(m_worldRotateAngle.Y) * Matrix.RotationZ(m_worldRotateAngle.Z));
            }
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_ROTATE_Y_MINUS"])])
            {
                m_worldRotateAngle.Y += 0.03f;
                m_worldRotate = (Matrix.RotationX(m_worldRotateAngle.X) * Matrix.RotationY(m_worldRotateAngle.Y) * Matrix.RotationZ(m_worldRotateAngle.Z));
            }
            //world rotation z
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_ROTATE_Z_PLUS"])])
            {
                m_worldRotateAngle.Z += 0.03f;
                m_worldRotate = (Matrix.RotationX(m_worldRotateAngle.X) * Matrix.RotationY(m_worldRotateAngle.Y) * Matrix.RotationZ(m_worldRotateAngle.Z));
            }
            //world rotation z
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_ROTATE_Z_MINUS"])])
            {
                m_worldRotateAngle.Z -= 0.03f;
                m_worldRotate = (Matrix.RotationX(m_worldRotateAngle.X) * Matrix.RotationY(m_worldRotateAngle.Y) * Matrix.RotationZ(m_worldRotateAngle.Z));
            }

            //world translation x
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_MOVE_RIGHT"])])
            {
                m_worldTranslateVector.X += 0.1f;
            }
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_MOVE_LEFT"])])
            {
                m_worldTranslateVector.X -= 0.1f;
            }
            //world translation y
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_MOVE_DOWN"])])
            {
                m_worldTranslateVector.Y += 1f;
            }
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_MOVE_UP"])])
            {
                m_worldTranslateVector.Y -= 1f;
            }
            //world translation z
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_MOVE_BACKWARDS"])])
            {
                m_worldTranslateVector.Z -= 1.0f;
            }
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_WORLD_MOVE_FORWARDS"])])
            {
                m_worldTranslateVector.Z += 1.0f;
            }

            if (!m_bIsGamePaused && (netPongClientGlobals.hUDPGameSession != null) )
            {
                //move player paddle
                if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_PADDLE_LEFT"])])
                {
                    m_paddlePlayerXPos += 0.5f;
                    netPongClientGlobals.hUDPGameSession.SendSetPaddlePosCommand(m_paddlePlayerXPos);
                }
                if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_KEY_PADDLE_RIGHT"])])
                {
                    m_paddlePlayerXPos -= 0.5f;
                    netPongClientGlobals.hUDPGameSession.SendSetPaddlePosCommand(m_paddlePlayerXPos);
                }
            }
            
            //resetting positions
            if (keys[(Key)int.Parse((string)netPongClientGlobals.hKeyBindings["NP_RESET_CAMERA_WORLD_POSITION"])])
            {
                m_worldRotateAngle = new Vector3(0, 0, 0);
                m_worldRotate = Matrix.Identity;
                m_worldTranslateVector = new Vector3(0, 0, 0);
                m_paddlePlayerXPos = 0.0f;
                if (netPongClientGlobals.hUDPGameSession != null)
                    netPongClientGlobals.hUDPGameSession.SendSetPaddlePosCommand(m_paddlePlayerXPos);
            }

            if (keys[Key.Space])
            {
                m_bIsGamePaused = !m_bIsGamePaused;
            }
        }

        public int Work()
        {
            ReadKeyboard();
            //MoveBall();
            CheckCollision();
            return Render();
        }

        public void MoveBall(Vector3 ballPos)
        {
            lock (m_ballPosLock)
            {
                m_ballPos = ballPos;
            }
            if (m_bIsGamePaused)
            {
                m_tLastBallPosCalc = DateTime.Now;
                return;
            }

            //time since last calculation
            DateTime now = DateTime.Now;
            TimeSpan tDif = now.Subtract(m_tLastBallPosCalc);
            m_tLastBallPosCalc = now;

            //recalculate X pos
            m_ballPos.X += (float)tDif.TotalMilliseconds * m_ballDirSpeed.X * 0.05f;

            //recalculate Z pos
            m_ballPos.Z += (float)tDif.TotalMilliseconds * m_ballDirSpeed.Z;
        }

        private void InitFont()
        {

            Microsoft.DirectX.Direct3D.FontDescription fDesc = new FontDescription();
            fDesc.CharSet = CharacterSet.Default;
            fDesc.Height = 20;
            m_fFps = new Microsoft.DirectX.Direct3D.Font(m_direct3DDevice, fDesc);
        }

        private void InitLights()
        {
            /*
            int nCount = m_direct3DDevice.Lights.Count;
            m_direct3DDevice.Lights[nCount].Type = LightType.Directional;
            m_direct3DDevice.Lights[nCount].DiffuseColor = new ColorValue(255, 255, 255);
            //m_direct3DDevice.Lights[nCount].AmbientColor = new ColorValue(255, 255, 0);
            m_direct3DDevice.Lights[nCount].Direction = new Vector3(0, -4, -25);
            m_direct3DDevice.Lights[nCount].Enabled = true;

            nCount = m_direct3DDevice.Lights.Count;
            m_direct3DDevice.Lights[nCount].Type = LightType.Point;
            m_direct3DDevice.Lights[nCount].Diffuse = Color.White;
            m_direct3DDevice.Lights[nCount].AmbientColor = new ColorValue(0, 1, 0);
            m_direct3DDevice.Lights[nCount].Position = new Vector3(2, 2, -2);
            m_direct3DDevice.Lights[nCount].Attenuation0 = 0f;
            m_direct3DDevice.Lights[nCount].Attenuation1 = 0.75f;
            m_direct3DDevice.Lights[nCount].Attenuation2 = 0f;
            m_direct3DDevice.Lights[nCount].Range = 100;
            m_direct3DDevice.Lights[nCount].Specular = Color.White;
            m_direct3DDevice.Lights[nCount].Enabled = true;

            int nCount = m_direct3DDevice.Lights.Count;
            m_direct3DDevice.Lights[nCount].Type = LightType.Spot;
            m_direct3DDevice.Lights[nCount].Diffuse = Color.Red;
            m_direct3DDevice.Lights[nCount].AmbientColor = new ColorValue(255, 255, 255);
            m_direct3DDevice.Lights[nCount].Position = new Vector3(0, 10, -25f);
            m_direct3DDevice.Lights[nCount].XDirection = 0f;//-m_direct3DDevice.Lights[nCount].XPosition;
            m_direct3DDevice.Lights[nCount].YDirection = -1f;//-m_direct3DDevice.Lights[nCount].YPosition;
            m_direct3DDevice.Lights[nCount].ZDirection = 0f;//-m_direct3DDevice.Lights[nCount].ZPosition;
            m_direct3DDevice.Lights[nCount].Attenuation0 = 1f;
            m_direct3DDevice.Lights[nCount].Range = 100f;
            m_direct3DDevice.Lights[nCount].Falloff = 1f;
            m_direct3DDevice.Lights[nCount].InnerConeAngle = (float)Math.PI / 4f;
            m_direct3DDevice.Lights[nCount].OuterConeAngle = (float)Math.PI / 2f;
            m_direct3DDevice.Lights[nCount].Enabled = true;
            */

            int nCount = m_direct3DDevice.Lights.Count;
            m_direct3DDevice.Lights[nCount].Type = LightType.Directional;
            m_direct3DDevice.Lights[nCount].Diffuse = System.Drawing.Color.Gray;
            m_direct3DDevice.Lights[nCount].Ambient = System.Drawing.Color.Gray;
            m_direct3DDevice.Lights[nCount].Direction = new Vector3(
                            (float)Math.Cos(Environment.TickCount / 250.0f),
                            1.0f,
                            (float)Math.Sin(Environment.TickCount / 250.0f));
            m_direct3DDevice.Lights[nCount].Enabled = true; // Turn it on

            m_direct3DDevice.RenderState.Ambient = System.Drawing.Color.FromArgb(0x202020);
            m_direct3DDevice.RenderState.Lighting = true;
            m_direct3DDevice.RenderState.SpecularEnable = true;
        }

        private void InitMaterials()
        {
            m_paddleMaterial = new Material();
            m_paddleMaterial.Ambient = Color.White;
            m_paddleMaterial.Diffuse = Color.White;
            m_paddleMaterial.Specular = Color.White;
            //m_paddleMaterial.Emissive = Color.Red;
            m_paddleMaterial.SpecularSharpness = 100;

            m_ballMaterial = new Material();
            m_ballMaterial.Ambient = Color.White;
            m_ballMaterial.Diffuse = Color.White;
            m_ballMaterial.Specular = Color.White;
            //m_ballMaterial.Emissive = Color.Silver;
            m_ballMaterial.SpecularSharpness = 100;

            m_gameFieldMaterial = new Material();
            m_gameFieldMaterial.Ambient = Color.White;
            m_gameFieldMaterial.Diffuse = Color.White;
            m_gameFieldMaterial.Specular = Color.White;
            //m_gameFieldMaterial.Emissive = Color.Red;
            m_gameFieldMaterial.SpecularSharpness = 100;
        }

        private void InitTextures()
        {
            // get a reference to the current assembly
            Assembly a = Assembly.GetExecutingAssembly();

            // get a list of resource names from the manifest
            string[] resNames = a.GetManifestResourceNames();

            foreach (string s in resNames)
            {
                if (s.EndsWith("Threadplate.dds"))
                {
                    // attach stream to the resource in the manifest
                    Stream xStream = xStream = a.GetManifestResourceStream(s);
                    if (null != xStream)
                    {
                        try
                        {
                            m_paddleTexture = TextureLoader.FromStream(m_direct3DDevice, xStream);
                            xStream.Close();
                            xStream = null;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                if (s.EndsWith("wood.dds"))
                {
                    // attach stream to the resource in the manifest
                    Stream xStream = xStream = a.GetManifestResourceStream(s);
                    if (null != xStream)
                    {
                        try
                        {
                            m_gameFieldTexture = TextureLoader.FromStream(m_direct3DDevice, xStream);
                            xStream.Close();
                            xStream = null;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        private void RenderPaddles()
        {
            try
            {
                m_direct3DDevice.Material = m_paddleMaterial;
                m_direct3DDevice.SetTexture(0, m_paddleTexture);

                //draw paddle 1
                int numPaddleSubSets = m_hPaddleMesh.GetAttributeTable().Length;
                m_direct3DDevice.SetTransform(TransformType.World, (Matrix.Translation(m_worldTranslateVector.X + m_paddleOpponentXPos, m_worldTranslateVector.Y + 0.0f, m_worldTranslateVector.Z + (-25.0f))) * m_worldRotate);
                for (int i = 0; i < numPaddleSubSets; ++i)
                    m_hPaddleMesh.DrawSubset(i);

                //draw paddle 2
                Matrix playerPaddleTransform = (Matrix.Translation(m_worldTranslateVector.X + m_paddlePlayerXPos, m_worldTranslateVector.Y + 0.0f, m_worldTranslateVector.Z + (25.0f))) * m_worldRotate;
                numPaddleSubSets = m_hPaddleMesh.GetAttributeTable().Length;
                m_direct3DDevice.SetTransform(TransformType.World, playerPaddleTransform);
                for (int i = 0; i < numPaddleSubSets; ++i)
                    m_hPaddleMesh.DrawSubset(i);
            }
            catch (Exception)
            {
            }
        }

        private void RenderBall()
        {
            try
            {
                m_direct3DDevice.SetTexture(0, null);
                m_direct3DDevice.Material = m_ballMaterial;

                Vector3 ballPos = new Vector3();
                lock (m_ballPosLock)
                {
                    ballPos = m_ballPos;
                }

                //draw ball
                Matrix ballTransform = (Matrix.Translation(m_worldTranslateVector.X + ballPos.X, m_worldTranslateVector.Y + 0.0f, m_worldTranslateVector.Z + ballPos.Z)) * m_worldRotate;
                int numBallSubSets = m_hBallMesh.GetAttributeTable().Length;
                m_direct3DDevice.SetTransform(TransformType.World, ballTransform);
                for (int i = 0; i < numBallSubSets; ++i)
                    m_hBallMesh.DrawSubset(i);
            }
            catch (Exception)
            {
            }
        }

        private void CheckCollision()
        {
            CheckPaddleCollisions();
            CheckWallCollisions();
        }

        private void CheckWallCollisions()
        {
            if (m_ballPos.X > 30.0f)
            {
                //m_ballPos.X = 30.0f;
                //m_ballDirSpeed.X *= -1;
            }
            if (m_ballPos.X < -30.0f)
            {
                //m_ballPos.X = -30.0f;
                //m_ballDirSpeed.X *= -1;
            }
        }

        private void CheckPaddleCollisions()
        {
            //player must react
            if (m_ballPos.Z > 24.0f)
            {
                float xStartPanel = m_paddlePlayerXPos + 6.5f;
                float xEndPanel = m_paddlePlayerXPos - 6.5f;

                //player caught ball
                if ((xEndPanel <= m_ballPos.X) && (m_ballPos.X <= xStartPanel))
                {
                    /*
                    //adjust x-direction
                    float xLengthBallHitPos = (m_ballPos.X - xStartPanel) + 6.5f;

                    //               = percentage where ball hit paddle * ~70 degree
                    m_ballDirSpeed.X = (xLengthBallHitPos / 6.5f) * (float)(Math.PI / 5.5);

                    //adjust z-direction and set ball out ouf collision zone
                    m_ballDirSpeed.Z *= -1.0f;
                    m_ballPos.Z = 24.0f;
                    */

                    //do sound notification
                    PlayCollisionSound();
                }
                //player missed ball
                else
                {
                    m_paddlePlayerXPos = 0.0f;

                    /*
                    m_ballPos.X = 0.0f;
                    m_ballDirSpeed.X = 0.0f;

                    m_ballPos.Z = -24.0f;
                    */

                    PlayPlayerLostBallSound();
                }
            }
            //opponent must react
            else if (m_ballPos.Z <= -24.0f)
            {
                //m_ballDirSpeed.Z *= -1.0f;
                //m_ballPos.Z = -24.0f;
                PlayCollisionSound();
            }
        }

        private void InitSound()
        {
            m_directSoundDevice = new Microsoft.DirectX.DirectSound.Device();
            m_directSoundDevice.SetCooperativeLevel(this, CooperativeLevel.Priority);

            // get a reference to the current assembly
            Assembly a = Assembly.GetExecutingAssembly();

            // get a list of resource names from the manifest
            string[] resNames = a.GetManifestResourceNames();


            foreach (string s in resNames)
            {
                if (s.EndsWith("pong.wav"))
                {
                    // attach stream to the resource in the manifest
                    Stream xStream = xStream = a.GetManifestResourceStream(s);
                    if (null != xStream)
                    {
                        try
                        {
                            BufferDescription bufdesc = new BufferDescription();
                            bufdesc.ControlEffects = false;
                            m_hitPaddleSoundBuffer = new SecondaryBuffer(xStream, bufdesc, m_directSoundDevice);
                            xStream.Close();
                            xStream = null;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                if (s.EndsWith("playerlostball.wav"))
                {
                    // attach stream to the resource in the manifest
                    Stream xStream = xStream = a.GetManifestResourceStream(s);
                    if (null != xStream)
                    {
                        try
                        {
                            BufferDescription bufdesc = new BufferDescription();
                            bufdesc.ControlEffects = false;
                            m_lostBallPlayerSoundBuffer = new SecondaryBuffer(xStream, bufdesc, m_directSoundDevice);
                            xStream.Close();
                            xStream = null;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        private void PlayCollisionSound()
        {
            m_hitPaddleSoundBuffer.Play(0, BufferPlayFlags.Default);
        }

        private void PlayPlayerLostBallSound()
        {
            m_lostBallPlayerSoundBuffer.Play(0, BufferPlayFlags.Default);
        }

        private void RenderGameField()
        {
            m_direct3DDevice.SetTexture(0, m_gameFieldTexture);
            m_direct3DDevice.Material = m_gameFieldMaterial;

            //draw gamefield
            Mesh field = Mesh.Box(m_direct3DDevice, 61, 0.5f, 49f);
            m_direct3DDevice.SetTransform(TransformType.World, (Matrix.Translation(m_worldTranslateVector.X + 0.0f, m_worldTranslateVector.Y - 1.0f, m_worldTranslateVector.Z + 0.0f)) * m_worldRotate);
            field.DrawSubset(0);
            field.Dispose();
        }

        private void AttemptRecovery()
        {
            try
            {
                m_direct3DDevice.TestCooperativeLevel();
            }
            catch (DeviceLostException)
            {
            }
            catch (DeviceNotResetException)
            {
                try
                {
                    m_direct3DDevice.Reset(m_presentParams);
                    m_bdeviceLost = false;
                }
                catch (DeviceLostException)
                {
                    // If it's still lost or lost again, just do 
                    // nothing
                }
            }
        }

        private void invitationRecieved()
        {
            DialogResult res = MessageBox.Show(this, strInvPlayerName + " invites you for a pong challenge.\r\nDo you want to start a match ?", "player invitation recieved", MessageBoxButtons.YesNo);
            if (res.Equals(DialogResult.Yes))
            {
                netPongClientTcpSocket.SendInvitePlayerCommand(strInvPlayerName);
                StartGame();
            }
            else if (res.Equals(DialogResult.No))
            {
                netPongClientTcpSocket.SendRejectInviteCommand(strInvPlayerName);
            }
        }

        public void SetOpponentPaddlePos(float fOpponentPaddlePos)
        {
            m_paddleOpponentXPos = fOpponentPaddlePos;
        }

        public void StartGame()
        {
        }

        protected void OnDeviceReset(object sender, EventArgs e)
        {
            /*
            InitSound();
            InitCamera();
            InitLights();
            InitMeshes();
            InitMaterials();
            InitTextures();
            InitFont();
            InitKeyboard();
            */
        }

        protected void OnDeviceLost(object sender, EventArgs e)
        {
            // Clean up the VertexBuffer
            //vertices.Dispose();
        }

        private void netPongClientGameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            netPongClientProgram.bEndProgram = true;
        }

        public void Shutdown()
        {
            ShutDownTextures();
            ShutDownMeshes();
            ShutDownFont();

            ShutDownKeyboard();
            ShutDownDXDevice();
            ShutDownSound();
        }

        private void ShutDownDXDevice()
        {
            m_direct3DDevice.Dispose();
        }

        private void ShutDownSound()
        {
            m_directSoundDevice.Dispose();
        }
        
        private void ShutDownMeshes()
        {
            m_hPaddleMesh.Dispose();
            m_hBallMesh.Dispose();
        }

        private void ShutDownTextures()
        {
            m_paddleTexture.Dispose();
            m_gameFieldTexture.Dispose();
        }

        private void ShutDownFont()
        {
            m_fFps.Dispose();
        }

        private void ShutDownKeyboard()
        {
            m_Keyb.Dispose();
        }
        
    }
}