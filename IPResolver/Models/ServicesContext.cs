using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Models
{
    public class ServicesContext : DbContext
    {
        public ServicesContext(DbContextOptions<ServicesContext> options) : base(options)
        {}
        public DbSet<ServiceRow> ServiseRows { get; set; }
    }
}
