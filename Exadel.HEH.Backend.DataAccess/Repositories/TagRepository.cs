﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;

namespace Exadel.HEH.Backend.DataAccess.Repositories
{
    public class TagRepository : BaseRepository<Tag>, ITagRepository
    {
        public TagRepository(IDbContext context)
            : base(context)
        {
        }

        public Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return Context.GetAsync<Tag>(t => ids.Contains(t.Id));
        }
    }
}
