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
<<<<<<< HEAD
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
            
=======
            AnimationTrackContainer anim1 = new AnimationTrackContainer();
            AnimationKeyContainerFloat3 tempAF3;

            tempAF3 = new AnimationKeyContainerFloat3(); tempAF3.Time = 0; tempAF3.Value = new float3(0, 0, 0); anim1.KeyFrames.Add(tempAF3);
>>>>>>> parent of 76709e3... Push MIT FEHLER in den Animationen
        }

        public SceneContainer Model { get { return model; } set { model = value; } }



    }
}
