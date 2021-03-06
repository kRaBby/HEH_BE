﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exadel.HEH.Backend.BusinessLogic.ValidationServices.Abstract;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;

namespace Exadel.HEH.Backend.BusinessLogic.ValidationServices
{
    public class CategoryValidationService : ICategoryValidationService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDiscountRepository _discountRepository;

        public CategoryValidationService(ICategoryRepository categoryRepository,
            IDiscountRepository discountRepository)
        {
            _categoryRepository = categoryRepository;
            _discountRepository = discountRepository;
        }

        public async Task<bool> CategoryNotInDiscountsAsync(Guid id)
        {
            var discounts = await _discountRepository.GetAllAsync();
            var anyDiscountWithCategory = discounts.Any(discount => discount.CategoryId == id);

            return !anyDiscountWithCategory;
        }

        public async Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken token = default)
        {
            return await _categoryRepository.GetByIdAsync(categoryId) != null;
        }

        public async Task<bool> CategoryIdNotExistsAsync(Guid categoryId, CancellationToken token = default)
        {
            return await _categoryRepository.GetByIdAsync(categoryId) is null;
        }

        public Task<bool> CategoryNameNotExistsAsync(string category,
            CancellationToken token = default)
        {
            return InnerCategoryNameNotExistsAsync(category);
        }

        public async Task<bool> CategoryNameChangedAndNotExistsAsync(Guid categoryId, string category,
            CancellationToken token = default)
        {
            var currentCategory = await _categoryRepository.GetByIdAsync(categoryId);

            if (currentCategory.Name == category)
            {
                return true;
            }

            return await InnerCategoryNameNotExistsAsync(category);
        }

        private async Task<bool> InnerCategoryNameNotExistsAsync(string category)
        {
            var categories = await _categoryRepository.GetAsync(c => c.Name == category);

            return !categories.Any();
        }
    }
}
