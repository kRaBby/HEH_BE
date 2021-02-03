﻿using System;
using System.Threading.Tasks;
using Exadel.HEH.Backend.BusinessLogic.DTOs.Create;
using Exadel.HEH.Backend.BusinessLogic.DTOs.Get;
using Exadel.HEH.Backend.BusinessLogic.Services.Abstract;
using Exadel.HEH.Backend.Host.Controllers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.HEH.Backend.Host.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase /*BaseController<FavoritesDto>*/
    {
        private readonly IFavoritesService _favoritesService;

        public FavoritesController(IFavoritesService favoritesService)
            //: base(favoritesService)
        {
            _favoritesService = favoritesService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(FavoritesCreateUpdateDto favorites)
        {
            if (ModelState.IsValid)
            {
                await _favoritesService.CreateAsync(favorites);
                return Created(string.Empty, favorites);
            }

            return BadRequest(ModelState);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync(FavoritesCreateUpdateDto favorites)
        {
            if (ModelState.IsValid)
            {
                await _favoritesService.UpdateAsync(favorites);
                return Ok(favorites);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{discountId:guid}")]
        public Task RemoveAsync(Guid discountId)
        {
            return _favoritesService.RemoveAsync(discountId);
        }
    }
}