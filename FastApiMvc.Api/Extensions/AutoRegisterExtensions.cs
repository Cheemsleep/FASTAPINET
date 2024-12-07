using System.Reflection;
using FastApiMvc.Model.Data;
using FastApiMvc.Model.Interfaces;
using FastApiMvc.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace FastApiMvc.Api.Extensions;

public static class AutoRegisterExtensions
{
    public static IServiceCollection AutoRegisterServices(this IServiceCollection services)
    {
        // 获取Service程序集
        var serviceAssembly = Assembly.Load("FastApiMvc.Service");
        
        // 获取所有实现了IService<>接口的具体服务类
        var serviceTypes = serviceAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                && t.GetInterfaces().Any(i => i.IsGenericType 
                    && i.GetGenericTypeDefinition() == typeof(IService<>)));

        foreach (var serviceType in serviceTypes)
        {
            // 获取服务类实现的所有接口
            var interfaces = serviceType.GetInterfaces();
            
            // 找到直接继承的服务接口（不包括IService<>）
            var serviceInterface = interfaces.FirstOrDefault(i => 
                i.GetInterfaces().Any(ii => ii.IsGenericType 
                    && ii.GetGenericTypeDefinition() == typeof(IService<>)));

            if (serviceInterface != null)
            {
                // 注册服务
                services.AddScoped(serviceInterface, serviceType);
            }
        }

        return services;
    }

    public static IServiceCollection AutoRegisterRepositories(this IServiceCollection services)
    {
        // 注册DbContext
        services.AddScoped<AppDbContext>();

        // 获取Model程序集
        var modelAssembly = Assembly.Load("FastApiMvc.Model");
        
        // 获取所有实现了IRepository<>接口的具体仓储类
        var repositoryTypes = modelAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                && t.GetInterfaces().Any(i => i.IsGenericType 
                    && i.GetGenericTypeDefinition() == typeof(IRepository<>)));

        foreach (var repositoryType in repositoryTypes)
        {
            // 获取仓储类实现的所有接口
            var interfaces = repositoryType.GetInterfaces();
            
            // 找到IRepository<>接口
            var repositoryInterface = interfaces.FirstOrDefault(i => 
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>));

            if (repositoryInterface != null)
            {
                // 注册具体的仓储实现
                services.AddScoped(repositoryInterface, repositoryType);
            }
        }

        return services;
    }

    public static IServiceCollection AutoRegisterValidators(this IServiceCollection services)
    {
        // 获取Api程序集
        var apiAssembly = Assembly.Load("FastApiMvc.Api");
        
        // 注册所有验证器
        services.AddValidatorsFromAssembly(apiAssembly);

        return services;
    }
}
