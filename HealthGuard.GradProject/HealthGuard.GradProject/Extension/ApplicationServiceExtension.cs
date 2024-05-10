﻿using HealthGuard.Core;
using HealthGuard.Core.Repository.contract;
using HealthGuard.Core.Services.contract;
using HealthGuard.GradProject.Errors;
using HealthGuard.GradProject.Helpers;
using HealthGuard.Service.AuthService;
using HealthGuard.Service.OrderService;
using HealthGuard.Service.PaymentService;
using HealthGurad.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HealthGuard.GradProject.Extension
{
    public static class ApplicationServiceExtension
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));
            services.Configure<ApiBehaviorOptions>(options =>
             options.InvalidModelStateResponseFactory = (actionContext) =>
             {
                 var errors = actionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
                                                      .SelectMany(p => p.Value.Errors)
                                                      .Select(e => e.ErrorMessage)
                                                      .ToList();
                 var response = new ApiValidationErrorResponse()
                 {
                     Errors = errors
                 };
                 return new BadRequestObjectResult(response);
             }
             
            );
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            //services.AddScoped(typeof(IAuthService), typeof(AuthService));
            //services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            //services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            return services;
        }
        public static WebApplication UseSwaggarMiddleWare(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
