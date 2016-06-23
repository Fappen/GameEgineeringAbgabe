using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Tutorial.Core
{
    public static class Player
    {
        private static int money = 1000;
        private static int villageHealth = 10;

        public static int Money { get { return money; } set { money = value; } }
        public static int VillageHealth { get { return villageHealth; } set { villageHealth = value; } }

    }
}
