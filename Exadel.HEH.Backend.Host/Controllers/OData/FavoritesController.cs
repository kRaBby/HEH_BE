﻿using System.Linq;
using System.Threading.Tasks;
using Exadel.HEH.Backend.BusinessLogic.DTOs;
using Exadel.HEH.Backend.BusinessLogic.Services.Abstract;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.Host.Infrastructure;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.HEH.Backend.Host.Controllers.OData
{
    [ODataRoutePrefix("Favorites")]
    [ODataAuthorize(Roles = nameof(UserRole.Employee))]
    public class FavoritesController : ODataController
    {
        private readonly IFavoritesService _favoritesService;

        public FavoritesController(IFavoritesService favoritesService)
        {
            _favoritesService = favoritesService;
        }

        /// <summary>
        /// Gets favorites. Filtering, sorting, pagination enabled via OData. For users with employee role.
        /// </summary>
        /// <param name="searchText">
        /// For searching by conditions, vendor, category, tags, countries, cities, streets.</param>
        [EnableQuery(EnsureStableOrdering = false)]
        [ODataRoute]
        public Task<IQueryable<FavoritesDto>> GetAsync([FromQuery] string searchText)
        {
            return _favoritesService.GetAsync(searchText);
        }
    }
}