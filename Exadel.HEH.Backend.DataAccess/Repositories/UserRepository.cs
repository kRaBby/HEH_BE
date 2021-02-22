﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Exadel.HEH.Backend.DataAccess.Extensions;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;

namespace Exadel.HEH.Backend.DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IDbContext context)
            : base(context)
        {
        }

        public IQueryable<User> Get()
        {
            return Context.GetAll<User>();
        }

        public Task<User> GetByEmail(string email)
        {
            return Context.GetAll<User>().FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<IEnumerable<User>> GetWithSubscriptionsAsync(
            Expression<Func<User, IEnumerable<Guid>>> inField,
            Expression<Func<User, bool>> expression,
            IEnumerable<Guid> subscriptions)
        {
            return Context.GetAnyInAndWhereAsync(inField, subscriptions, expression);
        }

        public Task<IEnumerable<User>> GetWithSubscriptionAsync(
            Expression<Func<User, IEnumerable<Guid>>> inField,
            Expression<Func<User, bool>> expression,
            Guid subscription)
        {
            return Context.GetAnyEqAndWhereAsync(inField, subscription, expression);
        }
    }
}