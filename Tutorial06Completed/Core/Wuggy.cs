using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Tutorial.Core
{
    class Wuggy
    {
        private SceneContainer model;
        private float3 position;
        private int speed;
        private int health;
        private float3 color;
        private int size;
        private int money;

        private AnimationComponent animList;
        

        public Wuggy(SceneContainer _model, float3 _position, int _size, float3 _color, int _speed, int _health, int _money)
        {
            animList = new AnimationComponent();

            model = _model;
            position = _position;
            size = _size;
            color = _color;
            speed = _speed;
            health = _health;
            money = _money;

            model.Children.First().GetTransform().Translation = position;
            model.Children.FindNodes(n => n.Name == "Body").First().GetMaterial().Diffuse.Color = _color;
            var scaleFactor = size * 0.1f;
            model.Children.First().GetTransform().Scale = new float3(scaleFactor, scaleFactor, scaleFactor);
        }

        public void SetUpAnimations()
        {
            AnimationTrackContainer anim1 = new AnimationTrackContainer();
            AnimationKeyContainerFloat3 tempAF3;

            tempAF3 = new AnimationKeyContainerFloat3(); tempAF3.Time = 0; tempAF3.Value = new float3(0, 0, 0); anim1.KeyFrames.Add(tempAF3);
        }

        public SceneContainer Model { get { return model; } set { model = value; } }



    }
}
