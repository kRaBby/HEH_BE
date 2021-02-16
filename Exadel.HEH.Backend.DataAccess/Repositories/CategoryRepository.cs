﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;

namespace Exadel.HEH.Backend.DataAccess.Repositories
{
    public class CategoryRepository : MongoRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(IDbContext context)
            : base(context)
        {
        }

        public Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return Context.GetAsync<Category>(c => ids.Contains(c.Id));
        }
    }
}
