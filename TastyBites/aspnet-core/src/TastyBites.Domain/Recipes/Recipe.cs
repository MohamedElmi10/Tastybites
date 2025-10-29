using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace TastyBites.Recipes
{
    public class Recipe : Entity<int>

    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    
}
}