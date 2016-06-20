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

        public Wuggy(SceneContainer _model, float3 _position, int _size, float3 _color, int _speed, int _health)
        {
            model = _model;
            position = _position;
            size = _size;
            color = _color;
            speed = _speed;
            health = _health;

            model.Children.First().GetTransform().Translation = position;
            model.Children.FindNodes(n => n.Name == "Body").First().GetMaterial().Diffuse.Color = _color;
        }

        public SceneContainer Model { get { return model; } set { model = value; } }

    }
}
