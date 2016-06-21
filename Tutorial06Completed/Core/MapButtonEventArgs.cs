using Fusee.Engine.Core.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Tutorial.Core
{
    class MapButtonEventArgs : GUIButtonEventArgs
    {
        public int index;

        public MapButtonEventArgs(int i)
        {
            index = i;
        }
    }
}
