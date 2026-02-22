using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoGameCatalogue.Shared.Base
{
    public class EntityBase  
    {
        [Key]
        public int Id { get; set; }
        // Base class entity for soft delete as an option
        public bool isDeleted { get; set; } = false;
        public DateTimeOffset? DeletedOnDts { get; set; } //UTC Friendly.

        // No need for deleted by user id as we dont have a user system, but would be here if we did.
        //public int? DeletedByUserID { get; set; }  

    }
}
