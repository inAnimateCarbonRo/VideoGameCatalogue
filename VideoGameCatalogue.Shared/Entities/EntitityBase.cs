using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoGameCatalogue.Shared.Entities
{
    public class EntitityBase
    {
        [Key]
        public int Id { get; set; }

        // Would usually InsertedOnDts and UpdatedOnDts, And InsertedByUserId and UpdatedByUserId
        // But no need for that here, we dont have a user system.

    }
}
