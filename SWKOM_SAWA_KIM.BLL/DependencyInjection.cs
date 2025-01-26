using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWKOM_SAWA_KIM.BLL.Services;
using SWKOM_SAWA_KIM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDocumentService, DocumentService>();
            //services.AddScoped<IDocumentLogic, DocumentLogic>();

            // Register AutoMapper
            services.AddAutoMapper(typeof(DependencyInjection));

            // Register FluentValidation
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
