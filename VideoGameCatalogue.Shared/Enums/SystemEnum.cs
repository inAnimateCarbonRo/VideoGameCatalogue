using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoGameCatalogue.Shared.Enums
{

    // Will be used in  VideoGameCatalogue.Shared.Config.SystemConfig to determine which API endpoint to use for the application.
    public enum SystemEnum
    {

        [Display(Name = "Live Production DB")]
        [Description("PRODDBNAME")]
        SystemProduction = 0,

        [Display(Name = "Development DB")]
        [Description("DEVDBNAME")]
        SystemDev = 1
    }

}
