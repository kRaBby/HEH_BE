﻿#nullable enable
using System.Diagnostics.CodeAnalysis;
using Exadel.HEH.Backend.BusinessLogic.DTOs;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.HEH.Backend.Host.Configuration
{
    [ExcludeFromCodeCoverage]
    public class StatisticsModelConfiguration : IModelConfiguration
    {
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
        {
            builder.EntitySet<DiscountStatisticsDto>("Statistics");

            builder.EntityType<DiscountStatisticsDto>().Collection
                .Function("Excel")
                .ReturnsCollectionFromEntitySet<DiscountStatisticsDto>("Statistics");
        }
    }
}