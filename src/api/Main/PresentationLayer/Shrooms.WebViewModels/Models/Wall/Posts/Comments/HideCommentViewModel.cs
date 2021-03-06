﻿using System.ComponentModel.DataAnnotations;
using Shrooms.WebViewModels.ValidationAttributes;

namespace Shrooms.WebViewModels.Models.Wall.Posts.Comments
{
    public class HideCommentViewModel
    {
        [Required]
        [MinValue(1)]
        public int Id { get; set; }
    }
}
