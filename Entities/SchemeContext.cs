using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDS
{
    public class SchemeContext : DbContext
    {
        public SchemeContext() : base("SchemesDatabaseConnection")
        {
        }

        public DbSet<Scheme> Schemes { get; set; }
        
    }
}
