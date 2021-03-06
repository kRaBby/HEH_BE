﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exadel.HEH.Backend.BusinessLogic.DTOs;

namespace Exadel.HEH.Backend.BusinessLogic.Services.Abstract
{
    public interface ITagService : IService<TagDto>
    {
        Task RemoveByCategoryAsync(Guid categoryId);

        Task<IEnumerable<TagDto>> GetByIdsAsync(IEnumerable<Guid> ids);

        Task RemoveAsync(Guid id);

        Task CreateAsync(TagDto item);

        Task UpdateAsync(TagDto item);
    }
}
