﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.models;

namespace WebApplication1Data.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Вказуємо роль користувача (Student / Teacher / Admin)
        public string Role { get; set; } = "Student";

        // Навігаційні властивості
        public ICollection<Grade>? Grades { get; set; }
        public ICollection<Material>? UploadedMaterials { get; set; }
    }
}