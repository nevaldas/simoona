﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Shrooms.WebViewModels.Models.User;

namespace Shrooms.WebViewModels.Models
{
    public class QualificationLevelViewModel : AbstractViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Common), ErrorMessageResourceName = "MessageRequiredField")]
        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        public virtual IEnumerable<ApplicationUserViewModel> ApplicationUsers { get; set; }

        public int SortOrder { get; set; }
    }
}