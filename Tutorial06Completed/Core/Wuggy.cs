using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Xirkit;

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

        AnimationTrackContainer animationContainer;
        //private AnimationComponent animationComponent;
        private Animation animation;
        private Channel<float3> channel;
        

        public Wuggy(SceneContainer _model, float3 _position, int _size, float3 _color, int _speed, int _health, int _money)
        {

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

            SetUpAnimations();
        }

        public void SetUpAnimations()
        {
            animationContainer = new AnimationTrackContainer();
            //animationComponent = new AnimationComponent();
            animation = new Animation();
            channel = new Channel<float3>(Lerp.Float3Lerp);

            animationContainer.KeyFrames = new List<AnimationKeyContainerBase>();
            animationContainer.KeyFrames.Add(new AnimationKeyContainerFloat3 { Time = 0, Value = new float3(0, 0, 0) });
            animationContainer.KeyFrames.Add(new AnimationKeyContainerFloat3 { Time = 2000, Value = new float3(80, 80, 80) });
            //animationComponent.AnimationTracks = new List<AnimationTrackContainer>();
            //animationComponent.AnimationTracks.Add(animationContainer);

            foreach (AnimationKeyContainerFloat3 key in animationContainer.KeyFrames)
            {
                channel.AddKeyframe(new Keyframe<float3>(key.Time, key.Value));
            }

            //model.Children.First().Components.Find(x => x.Name == "Animation") = animationComponent;
            animation.AddAnimation(channel, model.Children[0].GetTransform(), "Translation"); // _wuggy.Children.[0].Components.[0].Translation
            
        }

        public SceneContainer Model { get { return model; } set { model = value; } }
        public Animation Animation { get { return animation; } set { animation = value; } }



    }
}
