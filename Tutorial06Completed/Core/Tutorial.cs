#define GUI_SIMPLE
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using System.IO;
using System;
#if GUI_SIMPLE
using Fusee.Engine.Core.GUI;
#endif

namespace Fusee.Tutorial.Core
{
    [FuseeApplication(Name = "Tutorial Example", Description = "The official FUSEE Tutorial.")]
    public class Tutorial : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver6 * 2.0f, _angleVert = -M.PiOver6 * 0.5f,
                             _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit, _zoomVel, _zoom;
        private static float2 _offset;
        private static float2 _offsetInit;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneContainer _tower;
        private SceneContainer _wuggy;
        private float4x4 _sceneScale;
        private float4x4 _projection;
        private bool _twoTouchRepeated;

        private bool _keys;

        private Renderer _renderer;

#if GUI_SIMPLE
        private GUIHandler guiHandler;
        private GUIPanel guiPanelOverall;
        private GUIPanel guiPanelStatus;
        private GUIPanel guiPanelMap;
        private GUIPanel guiPanelShop;
        private GUIPanel guiPanelWaves;
        private GUIButton button1;
        private GUIButton startButton;
        private Font fontCabin;
        private FontMap _guiCabinBlack;
        private GUIText guiText;
#endif

#if GUI_SIMPLE
        private GUIHandler guiHandlerMap;
        private GUIButton block1;
#endif


        private List<Tower> listTowers;
        private List<Wuggy> listWuggys;


        // Init is called on startup. 
        public override void Init()
        {
            
            Width = 1295;
            Height = 760;

            listTowers = new List<Tower>();
            listWuggys = new List<Wuggy>();

            // Load the scene
            _scene = AssetStorage.Get<SceneContainer>("TDMAPinWuggyFINAL.fus");
            _tower = AssetStorage.Get<SceneContainer>("TowerRed.fus");
            //_wuggy = AssetStorage.Get<SceneContainer>("WuggyLand.fus");


            _sceneScale = float4x4.CreateScale(0.04f);

            listTowers.Add(new Tower(DeepCopy(_tower), new float3(0, 0, 100), 0, 0, 0));
            listTowers.Add(new Tower(DeepCopy(_tower), new float3(0, 400, 500), 0, 0, 0));

            //listWuggys.Add(new Wuggy(DeepCopy(_wuggy), new float3(0, 500, 750), 1, new float3(0.2f, 0.9f, 0.2f), 0, 1));

            // Instantiate our self-written renderer
            _renderer = new Renderer(RC);


#if GUI_SIMPLE
            guiHandler = new GUIHandler();
            guiHandler.AttachToContext(RC);

            fontCabin = AssetStorage.Get<Font>("Cabin.ttf");
            fontCabin.UseKerning = true;
            _guiCabinBlack = new FontMap(fontCabin, 18);

            guiText = new GUIText("Defend!", _guiCabinBlack, 310, 35);
            guiText.TextColor = new float4(0.05f, 0.25f, 0.15f, 0.8f);

            guiHandler.Add(guiText);

            // panel
            guiPanelOverall = new GUIPanel("Defend the Village", _guiCabinBlack, 880, 0, 400, 720);
            guiPanelOverall.PanelColor = new float4(0.0f, 0.5f, 0.5f, 1.0f);
            //guiHandler.Add(guiPanelOverall);

            guiPanelStatus = new GUIPanel("Defend the Village", _guiCabinBlack, 880, 0, 400, 120);
            guiPanelStatus.PanelColor = new float4(1.0f, 0.5f, 0.5f, 1.0f);
            guiHandler.Add(guiPanelStatus);

            guiPanelMap = new GUIPanel("Map", _guiCabinBlack, 880, 120, 400, 250);
            guiPanelMap.PanelColor = new float4(0.0f, 0.5f, 0.5f, 1.0f);
            guiHandler.Add(guiPanelMap);

            guiPanelShop = new GUIPanel("Shop", _guiCabinBlack, 880, 370, 400, 250);
            guiPanelShop.PanelColor = new float4(0.5f, 0.0f, 0.5f, 1.0f);
            guiHandler.Add(guiPanelShop);

            guiPanelWaves = new GUIPanel("Wave", _guiCabinBlack, 880, 620, 400, 100);
            guiPanelWaves.PanelColor = new float4(0.5f, 0.5f, 0.0f, 1.0f);
            guiHandler.Add(guiPanelWaves);

            button1 = new GUIButton("Testbutton", _guiCabinBlack, 900, 500, 100, 30);
            button1.ButtonColor = new float4(1.0f, 1.0f, 1.0f, 1.0f);
            button1.BorderColor = new float4(0, 0.6f, 0.2f, 1);
            button1.BorderWidth = 2;
            button1.OnGUIButtonDown += _guiFuseeLink_OnGUIButtonDown;
            button1.OnGUIButtonEnter += _guiFuseeLink_OnGUIButtonEnter;
            button1.OnGUIButtonLeave += _guiFuseeLink_OnGUIButtonLeave;
            guiHandler.Add(button1);

#endif

#if GUI_SIMPLE
            guiHandlerMap = new GUIHandler();
            guiHandlerMap.AttachToContext(RC);
            
            block1 = new GUIButton("Block", _guiCabinBlack, 900, 500, 100, 30);
            block1.ButtonColor = new float4(1.0f, 1.0f, 1.0f, 1.0f);
            block1.BorderColor = new float4(0, 0.6f, 0.2f, 1);
            block1.BorderWidth = 2;
            block1.OnGUIButtonDown += _guiFuseeLink_OnGUIButtonDown;
            block1.OnGUIButtonEnter += _guiFuseeLink_OnGUIButtonEnter;
            block1.OnGUIButtonLeave += _guiFuseeLink_OnGUIButtonLeave;
            guiHandlerMap.Add(block1);
#endif

            // Set the clear color for the backbuffer
            RC.ClearColor = new float4(1, 1, 1, 1);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);

            // Zoom & Roll
            if (Touch.TwoPoint)
            {
                if (!_twoTouchRepeated)
                {
                    _twoTouchRepeated = true;
                    _angleRollInit = Touch.TwoPointAngle - _angleRoll;
                    _offsetInit = Touch.TwoPointMidPoint - _offset;
                }
                _zoomVel = Touch.TwoPointDistanceVel * -0.01f;
                _angleRoll = Touch.TwoPointAngle - _angleRollInit;
                _offset = Touch.TwoPointMidPoint - _offsetInit;
            }
            else
            {
                _twoTouchRepeated = false;
                _zoomVel = Mouse.WheelVel * -0.5f;
                _angleRoll *= curDamp * 0.8f;
                _offset *= curDamp * 0.8f;
            }

            // UpDown / LeftRight rotation
            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * 0.000002f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * 0.000002f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _keys = false;
                float2 touchVel;
                touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * 0.000002f;
                _angleVelVert = -RotationSpeed * touchVel.y * 0.000002f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * 0.002f;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * 0.002f;
                }
                else
                {
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _zoom += _zoomVel;
            // Limit zoom
            if (_zoom < 80)
                _zoom = 80;
            if (_zoom > 2000)
                _zoom = 2000;

            _angleHorz += _angleVelHorz;
            // Wrap-around to keep _angleHorz between -PI and + PI
            _angleHorz = M.MinAngle(_angleHorz);

            _angleVert += _angleVelVert;
            // Limit pitch to the range between [-PI/2, + PI/2]
            _angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

            // Wrap-around to keep _angleRoll between -PI and + PI
            _angleRoll = M.MinAngle(_angleRoll);

            //GUI
            RC.Projection = float4x4.CreateOrthographic(0, 0, 1280, 720);
            RC.Viewport(0, 0, 1280, 720);


#if GUI_SIMPLE
            guiHandler.RenderGUI();
#endif

            // Setup Minimap
            RC.Projection = float4x4.CreateOrthographic(12750, 6955, -1000000.00f, 50000);
            _renderer.View = float4x4.CreateRotationX(-3.141592f / 2) * float4x4.CreateTranslation(0, 0, -300);

            RC.Viewport(885, 355, 390, 240);

            RC.SetShader(_renderer.shader);
            _renderer.Traverse(_scene.Children);


            foreach (Tower t in listTowers)
            {
                _renderer.Traverse(t.Model.Children);
            }

            foreach (Wuggy w in listWuggys)
            {
                _renderer.Traverse(w.Model.Children);
            }

#if GUI_SIMPLE
            guiHandlerMap.RenderGUI();
#endif

            RC.SetRenderState(new RenderStateSet
            {
                AlphaBlendEnable = false,
                ZEnable = true
            });
            
            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationZ(_angleRoll) * float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 20, -_zoom, 0, 0, 0, 0, 1, 0);
            _renderer.View = mtxCam * mtxRot * _sceneScale;
            var mtxOffset = float4x4.CreateTranslation(2 * _offset.x / Width, -2 * _offset.y / Height, 0);
            RC.Projection = mtxOffset * _projection;
            RC.Viewport(0, 0, 880, 720);

            RC.SetShader(_renderer.shader);
            _renderer.Traverse(_scene.Children);

            foreach (Tower t in listTowers)
            {
                _renderer.Traverse(t.Model.Children);
            }

            foreach (Wuggy w in listWuggys)
            {
                _renderer.Traverse(w.Model.Children);
            }

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();

        }

        public static float NormRot(float rot)
        {
            while (rot > M.Pi)
                rot -= M.TwoPi;
            while (rot < -M.Pi)
                rot += M.TwoPi;
            return rot;
        }



        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            _projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
        }

        #if GUI_SIMPLE
        private void _guiFuseeLink_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea)
        {
            button1.ButtonColor = new float4(1.0f, 1.0f, 1.0f, 1.0f);
            button1.BorderWidth = 0;
            SetCursor(CursorType.Standard);
        }

        private void _guiFuseeLink_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea)
        {
            button1.ButtonColor = new float4(0.0f, 0.6f, 0.2f, 0.4f);
            button1.BorderWidth = 1;
            SetCursor(CursorType.Hand);
        }

        void _guiFuseeLink_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea)
        {
            Random random = new Random();
            for (int i = 0; i < 10; i++) {
                listTowers.Add(new Tower(DeepCopy(_tower), new float3(random.Next(0, 1000), random.Next(0, 1000), random.Next(0, 1000)), 0, 0, 0));
            }
        }
#endif

        public static T DeepCopy<T>(T source) where T : class
        {
            var ser = new Serializer();
            var stream = new MemoryStream();
            ser.Serialize(stream, source);
            stream.Position = 0;
            return ser.Deserialize(stream, null, typeof(T)) as T;
        }
    }
}