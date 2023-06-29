using System.Collections.Generic;
using Rocket.API;

namespace Taser
{
    public class Config : IRocketPluginConfiguration
    {
        public List<ushort> TaserIDs;
        public ushort TaserEffectID;
        public float TasingLength;

        public bool MakePlayerSurrender;
        public bool MakePlayerStop;
        public bool MakePlayerProne;
        public bool MakePlayerExitVehicle;

        public void LoadDefaults()
        {
            TaserIDs = new List<ushort> {1165};
            TaserEffectID = 61;
            TasingLength = 10;
            MakePlayerStop = true;
            MakePlayerSurrender = true;
            MakePlayerProne = false;
            MakePlayerExitVehicle = true;
        }
    }
}