using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.Shared.Enums;

namespace VideoGameCatalogue.Shared.Config
{
    public static class SystemConfig
    {
        // Set the current system enum here
        // for possible multiple API endpoiints, its just 1 place to change then.
        // obv just one exists for now so this is overkill.
        public static SystemEnum CurrentSystemEnum { get; set; } = SystemEnum.SystemDev;
    }
}
