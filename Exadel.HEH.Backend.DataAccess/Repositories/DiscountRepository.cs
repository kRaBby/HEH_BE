﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;

namespace Exadel.HEH.Backend.DataAccess.Repositories
{
    public class DiscountRepository : MongoRepository<Discount>, IDiscountRepository
    {
        public DiscountRepository(IDbContext context)
            : base(context)
        {
        }

        public IQueryable<Discount> Get()
        {
            return Context.GetAll<Discount>();
        }

        public async Task RemoveTagsFromDiscounts(Guid tagId)
        {
            var collection = Context.GetAll<Discount>().Where(x => x.TagsIds.Contains(tagId));
            foreach (var item in collection)
            {
                await Task.Run(() => item.TagsIds.Remove(tagId));
            }

        }
    }
}