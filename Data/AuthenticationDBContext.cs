using AuthenticationWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationWebApi.Data
{
    public class AuthenticationDBContext : DbContext
    {
        //DbSet
        public AuthenticationDBContext(DbContextOptions options) : base(options)
        {
                
        }
       public DbSet<User> Users { get; set; }
    }
}
