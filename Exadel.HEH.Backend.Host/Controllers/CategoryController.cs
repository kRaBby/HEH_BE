﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exadel.HEH.Backend.BusinessLogic.DTOs;
using Exadel.HEH.Backend.BusinessLogic.Services.Abstract;
using Exadel.HEH.Backend.BusinessLogic.ValidationServices.Abstract;
using Exadel.HEH.Backend.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.HEH.Backend.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Employee))]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ICategoryValidationService _validationService;

        public CategoryController(ICategoryService categoryService, ICategoryValidationService validationService)
        {
            _validationService = validationService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// Gets tree of categories with their tags. For users with employee role.
        /// </summary>
        /// <returns>List of categories with tags.</returns>
        [HttpGet]
        public Task<IEnumerable<CategoryDto>> GetCategoriesWithTagsAsync()
        {
            return _categoryService.GetCategoriesWithTagsAsync();
        }

        /// <summary>
        /// Removes category. For users with moderator role.
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = nameof(UserRole.Moderator))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveAsync(Guid id)
        {
            if (await _validationService.CategoryExistsAsync(id))
            {
                if (await _validationService.CategoryNotInDiscountsAsync(id))
                {
                    await _categoryService.RemoveAsync(id);
                    return Ok();
                }

                return BadRequest("There are discounts with this category");
            }

            return NotFound();
        }

        /// <summary>
        /// Creates category. For users with moderator role.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Moderator))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync(CategoryDto item)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateAsync(item);
                return Created(string.Empty, item);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Updates category. For users with moderator role.
        /// </summary>
        [HttpPut]
        [Authorize(Roles = nameof(UserRole.Moderator))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAsync(CategoryDto item)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateAsync(item);
                return Ok(item);
            }

            return BadRequest(ModelState);
        }
    }
}