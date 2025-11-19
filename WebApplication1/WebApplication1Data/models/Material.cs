using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.Models;

namespace WebApplication1Data.models
{
    public class Material
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; } = default!;

        public string? Title { get; set; } = default!;
        public string? Description { get; set; } = string.Empty;
        public string? ContentUrl { get; set; } = default!; // шлях або URL до файлу

        // Хто завантажив
        public string? UploadedById { get; set; } = default!;
        public User UploadedBy { get; set; } = default!;


    }
}
