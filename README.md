# FastApiMvc.NET

一个基于 .NET 8.0 构建的现代化 Web API 项目，采用领域驱动设计（DDD）和最佳实践。

## 项目特点

- ✨ 基于 .NET 8.0
- 🏗️ 清晰的分层架构（API、Service、Model、Common）
- 🔒 集成身份认证和授权
- 📝 Swagger/OpenAPI 文档
- 🗄️ Entity Framework Core
- 🚀 Redis 缓存支持
- 🔍 FluentValidation 数据验证
- 🗺️ AutoMapper 对象映射
- 📊 统一的响应格式

## 项目结构

```
FastApiMvc.NET/
├── FastApiMvc.Api/          # API 层：控制器和中间件
├── FastApiMvc.Service/      # 服务层：业务逻辑实现
├── FastApiMvc.Model/        # 模型层：实体类和数据传输对象
└── FastApiMvc.Common/       # 公共层：工具类和扩展方法
```

## 快速开始

### 前置条件

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) 或 [MySQL](https://www.mysql.com/)
- [Redis](https://redis.io/) (可选，用于缓存)

### 安装

1. 克隆仓库
```bash
git clone https://github.com/Cheemsleep/FASTAPINET.git
cd FASTAPINET
```

2. 还原依赖包
```bash
dotnet restore
```

3. 更新数据库
```bash
cd FastApiMvc.Api
dotnet ef database update
```

4. 运行项目
```bash
dotnet run
```

访问 https://localhost:5001/swagger 查看 API 文档

## 配置

主要配置文件位于 `FastApiMvc.Api/appsettings.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "你的数据库连接字符串"
  },
  "Redis": {
    "ConnectionString": "你的Redis连接字符串"
  }
}
```

## 主要功能

- 用户管理 CRUD 操作
- JWT 身份认证
- Redis 缓存集成
- 统一的异常处理
- 请求验证
- 审计日志

## 开发规范

- 遵循 REST API 设计规范
- 使用 Entity Framework Core 进行数据访问
- 实现仓储模式和工作单元模式
- 使用依赖注入管理服务生命周期
- 统一的异常处理和日志记录

## 贡献指南

1. Fork 项目
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 提交 Pull Request

## 许可证

[MIT License](LICENSE)

## 联系方式

- 项目链接：[https://github.com/Cheemsleep/FASTAPINET](https://github.com/Cheemsleep/FASTAPINET)
- 作者：Cheemsleep

# FastApiMvc - .NET Core Web API 脚手架

FastApiMvc 是一个基于 .NET Core 8.0 的 Web API 脚手架项目，采用模块化架构设计，实现了常见的企业级应用开发模式和最佳实践。本项目的目标是提供一个功能完整、结构清晰的起点，帮助开发团队快速启动新项目。

## 项目特点

- 模块化架构，关注点分离
- 完整的仓储模式实现
- 通用的CRUD操作封装
- 全局异常处理
- 身份认证准备
- FluentValidation 数据验证
- Entity Framework Core
- AutoMapper 对象映射
- Redis 缓存支持
- Swagger API 文档

## 详细架构

```
FastApiMvc/
├── FastApiMvc.Api/              # Web API 层
│   ├── Controllers/            # API控制器
│   ├── Filters/               # 动作过滤器
│   ├── Middlewares/          # 中间件
│   ├── Extensions/           # 扩展方法
│   ├── Validators/           # 请求验证器
│   └── appsettings.json      # 配置文件
│
├── FastApiMvc.Common/           # 通用层
│   ├── Extensions/           # 通用扩展方法
│   ├── Helpers/             # 工具类
│   └── Constants/           # 常量定义
│
├── FastApiMvc.Model/            # 数据层
│   ├── Entities/            # 实体类
│   ├── Dto/                # 数据传输对象
│   ├── Data/               # 数据库上下文
│   └── Mapping/            # AutoMapper配置
│
└── FastApiMvc.Service/          # 服务层
    ├── Interfaces/          # 服务接口
    ├── Services/           # 服务实现
    └── Helpers/            # 服务层工具类
```

## 核心概念

### 1. 基础设施层

#### 1.1 实体基类 (BaseEntity)
所有实体类都继承自`BaseEntity`，提供基础属性：
```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

#### 1.2 仓储模式
通用仓储接口定义了基本的CRUD操作：
```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

#### 1.3 服务层基类
服务层基类封装了通用的业务逻辑：
```csharp
public abstract class BaseService<T> where T : BaseEntity
{
    protected readonly IRepository<T> _repository;
    
    protected BaseService(IRepository<T> repository)
    {
        _repository = repository;
    }
    
    // 通用CRUD方法实现...
}
```

### 2. 依赖注入配置

服务注册采用扩展方法方式，便于管理：

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }

    public static IServiceCollection AddRedisCache(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetSection("Redis")["ConnectionString"];
        });
        return services;
    }
}
```

### 3. 验证机制

使用FluentValidation进行请求验证：

```csharp
public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Matches("[A-Z]").WithMessage("密码必须包含大写字母")
            .Matches("[a-z]").WithMessage("密码必须包含小写字母")
            .Matches("[0-9]").WithMessage("密码必须包含数字");
    }
}
```

### 4. 异常处理

全局异常处理中间件：

```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

## 开发流程指南

### 1. 添加新功能

以添加一个新的业务模块为例，比如添加"产品"功能：

1. **创建实体**
```csharp
// FastApiMvc.Model/Entities/Product.cs
public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
```

2. **创建DTO**
```csharp
// FastApiMvc.Model/Dto/Product/ProductDto.cs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}

// FastApiMvc.Model/Dto/Product/CreateProductDto.cs
public class CreateProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
```

3. **配置AutoMapper**
```csharp
// FastApiMvc.Model/Mapping/ProductMappingProfile.cs
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
    }
}
```

4. **创建服务接口**
```csharp
// FastApiMvc.Service/Interfaces/IProductService.cs
public interface IProductService : IService<Product>
{
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto> UpdateAsync(int id, CreateProductDto dto);
    Task<IEnumerable<ProductDto>> SearchAsync(string keyword);
}
```

5. **实现服务**
```csharp
// FastApiMvc.Service/Services/ProductService.cs
public class ProductService : BaseService<Product>, IProductService
{
    private readonly IMapper _mapper;

    public ProductService(IRepository<Product> repository, IMapper mapper) 
        : base(repository)
    {
        _mapper = mapper;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        await _repository.AddAsync(product);
        return _mapper.Map<ProductDto>(product);
    }
}
```

6. **添加控制器**
```csharp
// FastApiMvc.Api/Controllers/ProductsController.cs
[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseApiController
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var result = await _productService.CreateAsync(dto);
        return Success(result);
    }
}
```

### 2. 使用缓存

```csharp
public class CachedProductService : IProductService
{
    private readonly IProductService _productService;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachedProductService> _logger;

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        var cacheKey = $"product_{id}";
        var product = await _cache.GetAsync<ProductDto>(cacheKey);
        
        if (product == null)
        {
            product = await _productService.GetByIdAsync(id);
            await _cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(10));
        }
        
        return product;
    }
}
```

## 性能优化建议

1. **使用异步方法**
- 所有I/O操作都应该是异步的
- 使用Task.WhenAll并行处理多个独立的异步操作

2. **合理使用缓存**
- 对频繁访问的数据使用Redis缓存
- 实现缓存更新策略（如：Cache-Aside模式）

3. **数据库优化**
- 使用适当的索引
- 避免N+1查询问题
- 使用Include进行预加载

4. **API响应优化**
- 使用DTOs避免过度暴露
- 实现分页机制
- 使用压缩中间件

## 调试技巧

1. **使用日志**
```csharp
public class ProductService
{
    private readonly ILogger<ProductService> _logger;

    public async Task<Product> CreateAsync(CreateProductDto dto)
    {
        _logger.LogInformation("Creating new product: {@ProductDto}", dto);
        try
        {
            // 创建产品逻辑
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {@ProductDto}", dto);
            throw;
        }
    }
}
```

2. **使用中间件记录请求**
```csharp
app.Use(async (context, next) =>
{
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    _logger.LogInformation($"Request Body: {requestBody}");
    await next();
});
```

## 安全最佳实践

1. **输入验证**
- 使用FluentValidation验证所有输入
- 实施跨站脚本(XSS)防护
- 实施SQL注入防护

2. **认证授权**
- 使用JWT进行身份认证
- 实施基于角色的访问控制(RBAC)
- 使用HTTPS

3. **数据保护**
- 使用加密存储敏感数据
- 实施密码哈希
- 保护配置文件中的机密信息

## 部署指南

1. **准备环境**
- 安装.NET 8.0 SDK
- 配置数据库
- 配置Redis（可选）

2. **配置文件**
- 更新appsettings.json中的连接字符串
- 配置日志级别
- 设置应用程序URL

3. **发布命令**
```bash
dotnet publish -c Release -o ./publish
```

## 贡献指南

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启一个 Pull Request

## 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情
