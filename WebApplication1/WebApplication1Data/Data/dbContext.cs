using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace WebApplication1Data.Data
{
    public class dbContext : IdentityDbContext<User>
    {
        public dbContext(DbContextOptions<dbContext> options)
        : base(options)
        {
        }

    }
}
