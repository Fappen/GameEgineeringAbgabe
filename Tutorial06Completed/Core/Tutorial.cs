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

        private Dictionary<int, Tower> listTowers;
        private List<Wuggy> listWuggys;

        private bool isTowerSelected;
        private bool isUgradeMode;
        private int currentSelectedTower;

        private int towerCosts;

#if GUI_SIMPLE
        private GUIHandler guiHandler;
        private GUIPanel guiPanelStatus;
        private GUIText moneyText;
        private GUIText healthText;
        private GUIPanel guiPanelShop;
        private GUIPanel guiPanelWaves;
        
        private GUIButton startButton;
        private Font fontCabin;
        private FontMap _guiCabinBlack;
        private FontMap _guiCabinBlackBig;
        private GUIText guiText;

        private GUIButton previousSelectedTowerButton;
        private int previousSelectedTower;

        private float4 redColor;
        private float4 greenColor;
        private float4 blueColor;
        private float4 yellowColor;
        private float4 whiteColor;
        private float4 cyanColor;

        private List<GUIButton> mapButtons;
        //private GUIButton towerButton1;
#endif

#if GUI_SIMPLE
        private GUIHandler guiHandlerTowers;
        private GUIButton buyButton;
#endif

#if GUI_SIMPLE
        private GUIHandler guiHandlerTowersUpgrade;
        private GUIButton speedUpgradeText;
        private GUIButton speedUpgradeButton;
        private GUIButton rangeUpgradeText;
        private GUIButton rangeUpgradeButton;
        private GUIButton damageUpgradeText;
        private GUIButton damageUpgradeButton;
#endif


        // Init is called on startup. 
        public override void Init()
        {
            
            Width = 1295;
            Height = 760;

            listTowers = new Dictionary<int, Tower>();
            listWuggys = new List<Wuggy>();

            isTowerSelected = false;
            isUgradeMode = false;
            towerCosts = 100;

            // Load the scene
            _scene = AssetStorage.Get<SceneContainer>("TDMAPinWuggyFINAL.fus");
            _tower = AssetStorage.Get<SceneContainer>("TowerRed.fus");
            _wuggy = AssetStorage.Get<SceneContainer>("WuggyFromLand.fus");

            _sceneScale = float4x4.CreateScale(0.04f);

            listWuggys.Add(new Wuggy(DeepCopy(_wuggy), new float3(0, 0, 750), 8, new float3(0.2f, 0.9f, 0.2f), 0, 1, 100));

            // Instantiate our self-written renderer
            _renderer = new Renderer(RC);

            redColor = new float4(1.0f, 0.0f, 0.2f, 1.0f);
            greenColor = new float4(0.4f, 1.0f, 0.0f, 1.0f);
            blueColor = new float4(0.0f, 0.4f, 1.0f, 1.0f);
            yellowColor = new float4(1.0f, 0.9f, 0.0f, 1.0f);
            whiteColor = new float4(1.0f, 1.0f, 1.0f, 1.0f);
            cyanColor = new float4(0.0f, 0.7f, 0.7f, 1.0f);

#if GUI_SIMPLE
            guiHandler = new GUIHandler();
            guiHandler.AttachToContext(RC);

            fontCabin = AssetStorage.Get<Font>("AmericanCaptain.ttf");
            fontCabin.UseKerning = true;
            _guiCabinBlack = new FontMap(fontCabin, 18);
            _guiCabinBlackBig = new FontMap(fontCabin, 25);

            guiText = new GUIText("Defend!", _guiCabinBlack, 310, 35);
            guiText.TextColor = new float4(0.05f, 0.25f, 0.15f, 0.8f);
            guiHandler.Add(guiText);

            guiPanelStatus = new GUIPanel("Defend the Village", _guiCabinBlack, 880, 0, 400, 120);
            guiPanelStatus.PanelColor = new float4(0.0f, 0.38f, 0.69f, 1.0f);
            guiPanelStatus.BorderWidth = 0;
            guiHandler.Add(guiPanelStatus);

            healthText = new GUIText("Health Village: 10", _guiCabinBlackBig, 900, 60);
            healthText.TextColor = new float4(0.9f, 0.0f, 0.0f, 1.0f);
            guiHandler.Add(healthText);

            moneyText = new GUIText("Money: 1000", _guiCabinBlackBig, 900, 90);
            moneyText.TextColor = new float4(0.0f, 0.9f, 0.1f, 1.0f);
            guiHandler.Add(moneyText);

            guiPanelShop = new GUIPanel("Shop", _guiCabinBlack, 880, 370, 400, 250);
            guiPanelShop.PanelColor = new float4(0.0f, 0.38f, 0.69f, 1.0f);
            guiPanelShop.BorderWidth = 0;
            guiHandler.Add(guiPanelShop);

            guiPanelWaves = new GUIPanel("Wave", _guiCabinBlack, 880, 620, 400, 100);
            guiPanelWaves.PanelColor = new float4(0.0f, 0.38f, 0.69f, 1.0f);
            guiPanelWaves.BorderWidth = 0;
            guiHandler.Add(guiPanelWaves);

            startButton = new GUIButton("Start Wave", _guiCabinBlack, 900, 650, 360, 50);
            //mapButtons[i].OnGUIButtonDown += mapOnGUIButtonDown;
            startButton.OnGUIButtonEnter += shopButtonEnter;
            startButton.OnGUIButtonLeave += shopButtonLeave;
            guiHandler.Add(startButton);

            mapButtons = new List<GUIButton>();

            for (int i = 0; i < 146; i++)
            {
                mapButtons.Add(new GUIButton(500, 500, 8, 8));
                mapButtons[i].ButtonColor = blueColor;
                mapButtons[i].BorderWidth = 0;
                mapButtons[i].OnGUIButtonDown += mapOnGUIButtonDown;
                mapButtons[i].OnGUIButtonEnter += mapOnGUIButtonEnter;
                mapButtons[i].OnGUIButtonLeave += mapOnGUIButtonLeave;
            }

#region MapButtons
            //mapButtons[136].ButtonColor = new float4(0, 0, 1.0f, 1.0f);


            setButtonPosition(mapButtons[0], 945, 219);
            setButtonPosition(mapButtons[1], 932, 219);
            setButtonPosition(mapButtons[2], 957, 219);
            setButtonPosition(mapButtons[3], 969, 219);
            setButtonPosition(mapButtons[4], 981, 219);
            setButtonPosition(mapButtons[5], 981, 197);
            setButtonPosition(mapButtons[6], 967, 197);
            setButtonPosition(mapButtons[7], 953, 197);
            setButtonPosition(mapButtons[8], 939, 197);
            setButtonPosition(mapButtons[9], 979, 290);
            setButtonPosition(mapButtons[10], 967, 290);
            setButtonPosition(mapButtons[11], 955, 290);
            setButtonPosition(mapButtons[12], 942, 290);
            setButtonPosition(mapButtons[13], 942, 314);
            setButtonPosition(mapButtons[14], 955, 314);
            setButtonPosition(mapButtons[15], 967, 314);
            setButtonPosition(mapButtons[16], 979, 314);
            setButtonPosition(mapButtons[17], 1044, 314);
            setButtonPosition(mapButtons[18], 1032, 314);
            setButtonPosition(mapButtons[19], 1020, 314);
            setButtonPosition(mapButtons[20], 1008, 314);
            setButtonPosition(mapButtons[21], 1057, 314);
            setButtonPosition(mapButtons[22], 1069, 314);
            setButtonPosition(mapButtons[23], 1081, 314);
            setButtonPosition(mapButtons[24], 1175, 282);
            setButtonPosition(mapButtons[25], 1163, 282);
            setButtonPosition(mapButtons[26], 1151, 282);
            setButtonPosition(mapButtons[27], 1103, 282);
            setButtonPosition(mapButtons[28], 1115, 282);
            setButtonPosition(mapButtons[29], 1127, 282);
            setButtonPosition(mapButtons[30], 1139, 282);
            setButtonPosition(mapButtons[31], 1149, 307);
            setButtonPosition(mapButtons[32], 1137, 307);
            setButtonPosition(mapButtons[33], 1125, 307);
            setButtonPosition(mapButtons[34], 1113, 307);
            setButtonPosition(mapButtons[35], 1161, 307);
            setButtonPosition(mapButtons[36], 1173, 307);
            setButtonPosition(mapButtons[37], 1185, 307);
            setButtonPosition(mapButtons[38], 1081, 337);
            setButtonPosition(mapButtons[39], 1069, 337);
            setButtonPosition(mapButtons[40], 1057, 337);
            setButtonPosition(mapButtons[41], 1008, 337);
            setButtonPosition(mapButtons[42], 1020, 337);
            setButtonPosition(mapButtons[43], 1032, 337);
            setButtonPosition(mapButtons[44], 1044, 337);
            setButtonPosition(mapButtons[45], 1059, 259);
            setButtonPosition(mapButtons[46], 1047, 259);
            setButtonPosition(mapButtons[47], 1035, 259);
            setButtonPosition(mapButtons[48], 1023, 259);
            setButtonPosition(mapButtons[49], 1071, 259);
            setButtonPosition(mapButtons[50], 1091, 242);
            setButtonPosition(mapButtons[51], 1103, 242);
            setButtonPosition(mapButtons[52], 1127, 242);
            setButtonPosition(mapButtons[53], 1115, 242);
            setButtonPosition(mapButtons[54], 1167, 222);
            setButtonPosition(mapButtons[55], 1179, 222);
            setButtonPosition(mapButtons[56], 1155, 222);
            setButtonPosition(mapButtons[57], 1143, 222);
            setButtonPosition(mapButtons[58], 1143, 194);
            setButtonPosition(mapButtons[59], 1155, 194);
            setButtonPosition(mapButtons[60], 1182, 194);
            setButtonPosition(mapButtons[61], 1168, 194);
            setButtonPosition(mapButtons[62], 1102, 216);
            setButtonPosition(mapButtons[63], 1114, 216);
            setButtonPosition(mapButtons[64], 1090, 216);
            setButtonPosition(mapButtons[65], 1078, 216);
            setButtonPosition(mapButtons[66], 1023, 236);
            setButtonPosition(mapButtons[67], 1035, 236);
            setButtonPosition(mapButtons[68], 1047, 236);
            setButtonPosition(mapButtons[69], 1059, 236);
            setButtonPosition(mapButtons[70], 1008, 301);
            setButtonPosition(mapButtons[71], 1008, 287);
            setButtonPosition(mapButtons[72], 999, 243);
            setButtonPosition(mapButtons[73], 999, 258);
            setButtonPosition(mapButtons[74], 1058, 183);
            setButtonPosition(mapButtons[75], 1046, 183);
            setButtonPosition(mapButtons[76], 1034, 183);
            setButtonPosition(mapButtons[77], 1022, 183);
            setButtonPosition(mapButtons[78], 1070, 183);
            setButtonPosition(mapButtons[79], 1082, 183);
            setButtonPosition(mapButtons[80], 1147, 176);
            setButtonPosition(mapButtons[81], 1070, 157);
            setButtonPosition(mapButtons[82], 1022, 157);
            setButtonPosition(mapButtons[83], 1034, 157);
            setButtonPosition(mapButtons[84], 1046, 157);
            setButtonPosition(mapButtons[85], 1058, 157);
            setButtonPosition(mapButtons[86], 1135, 176);
            setButtonPosition(mapButtons[87], 1099, 176);
            setButtonPosition(mapButtons[88], 1111, 176);
            setButtonPosition(mapButtons[89], 1123, 176);
            setButtonPosition(mapButtons[90], 1113, 151);
            setButtonPosition(mapButtons[91], 1101, 151);
            setButtonPosition(mapButtons[92], 1089, 151);
            setButtonPosition(mapButtons[93], 1125, 151);
            setButtonPosition(mapButtons[94], 1137, 151);
            setButtonPosition(mapButtons[95], 1206, 146);
            setButtonPosition(mapButtons[96], 1194, 146);
            setButtonPosition(mapButtons[97], 1158, 146);
            setButtonPosition(mapButtons[98], 1170, 146);
            setButtonPosition(mapButtons[99], 1182, 146);
            setButtonPosition(mapButtons[100], 1179, 237);
            setButtonPosition(mapButtons[101], 1179, 252);
            setButtonPosition(mapButtons[102], 1206, 252);
            setButtonPosition(mapButtons[103], 1206, 237);
            setButtonPosition(mapButtons[104], 1206, 222);
            setButtonPosition(mapButtons[105], 1200, 278);
            setButtonPosition(mapButtons[106], 1200, 292);
            setButtonPosition(mapButtons[107], 1195, 194);
            setButtonPosition(mapButtons[108], 1182, 168);
            setButtonPosition(mapButtons[109], 1252, 194);
            setButtonPosition(mapButtons[110], 1264, 194);
            setButtonPosition(mapButtons[111], 1240, 194);
            setButtonPosition(mapButtons[112], 1228, 194);
            setButtonPosition(mapButtons[113], 1228, 168);
            setButtonPosition(mapButtons[114], 1240, 168);
            setButtonPosition(mapButtons[115], 1252, 168);
            setButtonPosition(mapButtons[116], 1264, 168);
            setButtonPosition(mapButtons[117], 1170, 168);
            setButtonPosition(mapButtons[118], 1194, 168);
            setButtonPosition(mapButtons[119], 1158, 168);
            setButtonPosition(mapButtons[120], 1023, 202);
            setButtonPosition(mapButtons[121], 1023, 216);
            setButtonPosition(mapButtons[122], 999, 194);
            setButtonPosition(mapButtons[123], 999, 179);
            setButtonPosition(mapButtons[124], 942, 276);
            setButtonPosition(mapButtons[125], 942, 261);
            setButtonPosition(mapButtons[126], 915, 209);
            setButtonPosition(mapButtons[127], 915, 227);
            setButtonPosition(mapButtons[128], 915, 245);
            setButtonPosition(mapButtons[129], 915, 191);
            setButtonPosition(mapButtons[130], 889, 191);
            setButtonPosition(mapButtons[131], 889, 245);
            setButtonPosition(mapButtons[132], 889, 227);
            setButtonPosition(mapButtons[133], 889, 209);
            setButtonPosition(mapButtons[134], 912, 276);
            setButtonPosition(mapButtons[135], 912, 290);
            setButtonPosition(mapButtons[136], 928, 314);
            setButtonPosition(mapButtons[137], 912, 305);
            setButtonPosition(mapButtons[138], 928, 245);
            setButtonPosition(mapButtons[139], 915, 161);
            setButtonPosition(mapButtons[140], 928, 161);
            setButtonPosition(mapButtons[141], 901, 161);
            setButtonPosition(mapButtons[142], 889, 174);
            setButtonPosition(mapButtons[143], 939, 178);
            setButtonPosition(mapButtons[144], 899, 276);
            setButtonPosition(mapButtons[145], 886, 276);
#endregion

            foreach (GUIButton gb in mapButtons)
            {
                guiHandler.Add(gb);
            }



            guiHandlerTowers = new GUIHandler();
            guiHandlerTowers.AttachToContext(RC);
            buyButton = new GUIButton("Buy Tower (100)", _guiCabinBlack, 900, 400, 360, 50);
            buyButton.OnGUIButtonDown += buyTower;
            buyButton.OnGUIButtonEnter += shopButtonEnter;
            buyButton.OnGUIButtonLeave += shopButtonLeave;
            guiHandlerTowers.Add(buyButton);

            guiHandlerTowersUpgrade = new GUIHandler();
            guiHandlerTowersUpgrade.AttachToContext(RC);

            speedUpgradeText = new GUIButton("Shot Frequency: 1", _guiCabinBlack, 900, 400, 180, 50);
            speedUpgradeText.ButtonColor = new float4(0.0f, 0.38f, 0.69f, 1.0f);
            speedUpgradeText.TextColor = new float4(1.0f, 1.0f, 1.0f, 1.0f);
            speedUpgradeText.BorderWidth = 0;
            guiHandlerTowersUpgrade.Add(speedUpgradeText);

            speedUpgradeButton = new GUIButton("Speed Upgrade", _guiCabinBlack, 1080, 400, 180, 50);
            //mapButtons[i].OnGUIButtonDown += mapOnGUIButtonDown;
            speedUpgradeButton.OnGUIButtonEnter += shopButtonEnter;
            speedUpgradeButton.OnGUIButtonLeave += shopButtonLeave;
            guiHandlerTowersUpgrade.Add(speedUpgradeButton);

            rangeUpgradeText = new GUIButton("Range: 1", _guiCabinBlack, 900, 475, 180, 50);
            rangeUpgradeText.ButtonColor = new float4(0.0f, 0.38f, 0.69f, 1.0f);
            rangeUpgradeText.TextColor = new float4(1.0f, 1.0f, 1.0f, 1.0f);
            rangeUpgradeText.BorderWidth = 0;
            guiHandlerTowersUpgrade.Add(rangeUpgradeText);

            rangeUpgradeButton = new GUIButton("Range Upgrade", _guiCabinBlack, 1080, 475, 180, 50);
            //mapButtons[i].OnGUIButtonDown += mapOnGUIButtonDown;
            rangeUpgradeButton.OnGUIButtonEnter += shopButtonEnter;
            rangeUpgradeButton.OnGUIButtonLeave += shopButtonLeave;
            guiHandlerTowersUpgrade.Add(rangeUpgradeButton);

            damageUpgradeText = new GUIButton("Damage: 1", _guiCabinBlack, 900, 550, 180, 50);
            damageUpgradeText.ButtonColor = new float4(0.0f, 0.38f, 0.69f, 1.0f);
            damageUpgradeText.TextColor = new float4(1.0f, 1.0f, 1.0f, 1.0f);
            damageUpgradeText.BorderWidth = 0;
            guiHandlerTowersUpgrade.Add(damageUpgradeText);

            damageUpgradeButton = new GUIButton("Damage Upgrade", _guiCabinBlack, 1080, 550, 180, 50);
            //mapButtons[i].OnGUIButtonDown += mapOnGUIButtonDown;
            damageUpgradeButton.OnGUIButtonEnter += shopButtonEnter;
            damageUpgradeButton.OnGUIButtonLeave += shopButtonLeave;
            guiHandlerTowersUpgrade.Add(damageUpgradeButton);


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
            //RC.Projection = float4x4.CreateOrthographic(0, 0, 1280, 720);
            RC.Viewport(0, 0, 1280, 720);


#if GUI_SIMPLE
            if (isTowerSelected == true)
            {
                if (isUgradeMode == true)
                {
                    guiHandlerTowersUpgrade.RenderGUI();
                }
                else {
                    guiHandlerTowers.RenderGUI();
                }
            }
#endif

#if GUI_SIMPLE
            guiHandler.RenderGUI();
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

            foreach (Tower t in listTowers.Values)
            {
                _renderer.Traverse(t.Model.Children);
            }

            foreach (Wuggy w in listWuggys)
            {
                _renderer.Traverse(w.Model.Children);
            }

            // Setup Minimap
            RC.Projection = float4x4.CreateOrthographic(12750, 6955, -1000000.00f, 50000);
            _renderer.View = float4x4.CreateRotationX(-3.141592f / 2) * float4x4.CreateTranslation(0, 0, -300);

            RC.Viewport(885, 355, 390, 240);

            RC.SetShader(_renderer.shader);
            _renderer.Traverse(_scene.Children);


            foreach (Tower t in listTowers.Values)
            {
                _renderer.Traverse(t.Model.Children);
            }

            foreach (Wuggy w in listWuggys)
            {
                _renderer.Traverse(w.Model.Children);
            }

            updateStatusPanel();

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
            var aspectRatio = (Width-400) / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            _projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
        }



#if GUI_SIMPLE
        public void setButtonPosition(GUIButton button, int posX, int posY)
        {
            button.PosX = posX;
            button.PosY = posY;
        }

        private void mapOnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea)
        {
            int tempTowerNumber = mapButtons.FindIndex(a => a == sender);

            if (currentSelectedTower != tempTowerNumber)
            {
                if (listTowers.ContainsKey(tempTowerNumber))
                {
                    sender.ButtonColor = redColor;
                }
                else
                {
                    sender.ButtonColor = blueColor;
                }
            }
            else if (currentSelectedTower == tempTowerNumber)
            {
                sender.ButtonColor = yellowColor;
            }
            
            SetCursor(CursorType.Standard);
        }

        private void mapOnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea)
        {
            int tempTowerNumber = mapButtons.FindIndex(a => a == sender);

            if (isTowerSelected && currentSelectedTower == tempTowerNumber)
            {
                sender.ButtonColor = yellowColor;
            }
            else
            {
                sender.ButtonColor = cyanColor;
            }

            SetCursor(CursorType.Hand);
        }

        void mapOnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea)
        {
            previousSelectedButtonColor();


            if (currentSelectedTower == mapButtons.FindIndex(a => a == sender) || isTowerSelected == false)
            {
                isTowerSelected = !isTowerSelected;
            }

            if (isTowerSelected)
            {
                currentSelectedTower = mapButtons.FindIndex(a => a == sender);
                previousSelectedTower = currentSelectedTower;
                previousSelectedTowerButton = sender;
            }
            else
            {
                currentSelectedTower = -1;
            }

            if (listTowers.ContainsKey(currentSelectedTower))
            {
                isUgradeMode = true;
            }
            else
            {
                isUgradeMode = false;
            }

            sender.ButtonColor = yellowColor;
        }

        void buyTower(GUIButton sender, GUIButtonEventArgs mea)
        {
            if (Player.Money >= towerCosts && isTowerSelected == true && isUgradeMode == false)
            {
                string name;
                if (currentSelectedTower == 0)
                {
                    name = "Würfel";
                }
                else {
                    name = "Würfel." + currentSelectedTower.ToString();
                }

                float3 position = _scene.Children.FindNodes(n => n.Name == name).First().GetTransform().Translation;
                position.y = position.y + 100.0f;
                listTowers.Add(currentSelectedTower, new Tower(DeepCopy(_tower), position, 0, 0, 0));

                Player.Money -= towerCosts;
                isUgradeMode = true;
                //mapButtons[currentSelectedTower].ButtonColor = redColor;
            }
        }

        void shopButtonEnter(GUIButton sender, GUIButtonEventArgs mea)
        {
            sender.ButtonColor = cyanColor;
        }

        void shopButtonLeave(GUIButton sender, GUIButtonEventArgs mea)
        {
            sender.ButtonColor = whiteColor;
        }

        void updateStatusPanel()
        {
            moneyText.Text = "Money: " + Player.Money.ToString();
            healthText.Text = "Health Village: " + Player.VillageHealth.ToString();
        }

        void previousSelectedButtonColor()
        {
            if (previousSelectedTowerButton != null)
            {
                if (listTowers.ContainsKey(previousSelectedTower))
                {
                    previousSelectedTowerButton.ButtonColor = redColor;
                }
                else
                {
                    previousSelectedTowerButton.ButtonColor = blueColor;
                }
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