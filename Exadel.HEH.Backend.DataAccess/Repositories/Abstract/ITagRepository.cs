﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exadel.HEH.Backend.DataAccess.Models;

namespace Exadel.HEH.Backend.DataAccess.Repositories.Abstract
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<Guid> ids);
    }
}