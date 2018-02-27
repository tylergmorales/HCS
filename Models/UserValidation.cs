using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ispProject.Models
{
    [MetadataType(typeof(UserValidation))]
    public partial class user
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public class UserValidation
    {
        // Name the field the same as EF named the property - "FirstName" for example.
        // Also, the type needs to match.  Basically just redeclare it.
        // Note that this is a field.  I think it can be a property too, but fields definitely should work.

        [Required]
        [Display(Name = "First Name")]
        public string firstName;
        [Required]
        [Display(Name = "Last Name")]
        public string lastName;
    }
}