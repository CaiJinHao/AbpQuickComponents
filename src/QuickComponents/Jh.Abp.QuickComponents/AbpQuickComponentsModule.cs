﻿using IdentityModel;
using Jh.Abp.QuickComponents.Cors;
using Jh.Abp.QuickComponents.Json;
using Jh.Abp.QuickComponents.JwtAuthentication;
using Jh.Abp.QuickComponents.Localization;
using Jh.Abp.QuickComponents.MiniProfiler;
using Jh.Abp.QuickComponents.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Json;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Jh.Abp.QuickComponents
{
    //必须在最下边依赖
    public class AbpQuickComponentsModule: AbpModule
    {
        private IConfiguration configuration { get; set; }

        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            //Id4 jwt
            //AbpClaimTypes.UserId = JwtClaimTypes.Subject;
            //AbpClaimTypes.UserName = JwtClaimTypes.Name;
            //AbpClaimTypes.Role = JwtClaimTypes.Role;
            //AbpClaimTypes.Email = JwtClaimTypes.Email;
            //AbpClaimTypes.Name = JwtClaimTypes.GivenName;
            //AbpClaimTypes.SurName = JwtClaimTypes.FamilyName;

            PreConfigure<AbpJsonOptions>(options =>
            {
                //use coustomer ContractResolver
                options.UseHybridSerializer = false;
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            configuration = context.Services.GetConfiguration();

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpQuickComponentsModule>();
            });

            Configure<AbpJsonOptions>(options =>
            {
                options.DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            });

            Configure<MvcNewtonsoftJsonOptions>(options =>
            {
                // 配合UseHybridSerializer=false 使用
                options.SerializerSettings.ContractResolver = new JhMvcJsonContractResolver(context.Services);
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<JhAbpQuickComponentsResource>("zh-Hans")
                    .AddBaseTypes(typeof(AbpValidationResource))
                     //模块资源按照项目名称+文件夹路径写
                     .AddVirtualJson("/Jh/Abp/QuickComponents/Localization/JhAbpQuickComponents");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("JhAbpQuickComponents", typeof(JhAbpQuickComponentsResource));
            });

            // 在前面控制
            //context.Services.AddMiniProfilerComponent();
            //context.Services.AddSwaggerComponent(configuration);
            //context.Services.AddCorsPolicy(configuration);
            //context.Services.AddLocalizationComponent();
            //context.Services.AddJwtAuthenticationComponent(configuration);
        }

        /*public override void OnApplicationInitialization(Volo.Abp.ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            // 在前面控制
            app.UseMiniProfiler();
            app.UseCors(CorsExtensions.DefaultCorsPolicyName);
        }*/
    }
}
