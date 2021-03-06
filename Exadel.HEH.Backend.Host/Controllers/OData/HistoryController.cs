﻿using System.Linq;
using System.Threading.Tasks;
using Exadel.HEH.Backend.BusinessLogic.DTOs;
using Exadel.HEH.Backend.BusinessLogic.Services.Abstract;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.Host.Infrastructure;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;

namespace Exadel.HEH.Backend.Host.Controllers.OData
{
    [ODataRoutePrefix("History")]
    [ODataAuthorize(Roles = nameof(UserRole.Administrator))]
    public class HistoryController : ODataController
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        /// <summary>
        /// Gets history ordered by date (descending). Filtering, sorting, pagination enabled via OData. For users with admin role.
        /// </summary>
        [EnableQuery(
            HandleNullPropagation = HandleNullPropagationOption.False,
            EnsureStableOrdering = false)]
        [ODataRoute]
        public async Task<IQueryable<HistoryDto>> Get()
        {
            return await _historyService.GetAllAsync();
        }
    }
}