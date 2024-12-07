# FastApiMvc 框架学习指南 - 从入门到精通

## 第一章：框架基础概念

### 1.1 为什么要使用这个框架？

这个框架采用了领域驱动设计(DDD)的思想，将应用程序分为不同的层次：
- **表示层**（Api层）：处理用户请求和响应
- **业务层**（Service层）：处理业务逻辑
- **数据层**（Model层）：处理数据存储和访问
- **公共层**（Common层）：提供通用功能

这种分层设计的好处是：
1. 代码职责清晰，易于维护
2. 各层之间松耦合，便于修改和扩展
3. 遵循了面向对象的SOLID原则

### 1.2 核心概念解析

#### a) 实体（Entity）
实体是业务数据的载体，比如User、Order等。所有实体都继承自BaseEntity：
```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }            // 唯一标识符
    public DateTime CreatedAt { get; set; } // 创建时间
    public DateTime? UpdatedAt { get; set; }// 更新时间
    public bool IsDeleted { get; set; }     // 软删除标记
}
```

为什么要有BaseEntity？
- 统一管理公共属性
- 支持审计功能（创建时间、更新时间）
- 实现软删除机制

#### b) 仓储模式（Repository Pattern）
仓储模式是数据访问的抽象层，它隐藏了数据访问的具体细节：
```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

为什么要使用仓储模式？
- 统一数据访问接口
- 方便切换数据源（如从SQL Server切换到MongoDB）
- 便于单元测试（可以轻松模拟数据访问）

#### c) 服务层（Service Layer）
服务层封装业务逻辑，是应用程序的核心：
```csharp
public interface IService<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

为什么需要服务层？
- 封装业务规则和逻辑
- 处理跨实体的业务操作
- 实现事务管理

## 第二章：动手实践 - 创建第一个功能模块

### 2.1 创建待办事项（Todo）功能

让我们通过创建一个简单的待办事项功能来理解整个框架的工作流程：

1. **创建实体**
```csharp
// FastApiMvc.Model/Entities/Todo.cs
public class Todo : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
}
```

2. **创建仓储**
```csharp
// FastApiMvc.Model/Repositories/TodoRepository.cs
public class TodoRepository : BaseRepository<Todo>, ITodoRepository
{
    public TodoRepository(DbContext context) : base(context)
    {
    }
    
    // 可以添加特定于Todo的查询方法
    public async Task<IEnumerable<Todo>> GetOverdueTodosAsync()
    {
        return await _dbSet
            .Where(t => t.DueDate < DateTime.Now && !t.IsCompleted)
            .ToListAsync();
    }
}
```

3. **创建服务**
```csharp
// FastApiMvc.Service/Services/TodoService.cs
public class TodoService : BaseService<Todo>, ITodoService
{
    private readonly ITodoRepository _todoRepository;
    
    public TodoService(ITodoRepository repository) : base(repository)
    {
        _todoRepository = repository;
    }
    
    public async Task<IEnumerable<Todo>> GetOverdueTodosAsync()
    {
        return await _todoRepository.GetOverdueTodosAsync();
    }
}
```

4. **创建控制器**
```csharp
// FastApiMvc.Api/Controllers/TodoController.cs
[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    
    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var todos = await _todoService.GetAllAsync();
        return Ok(todos);
    }
    
    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
    {
        var overdueTodos = await _todoService.GetOverdueTodosAsync();
        return Ok(overdueTodos);
    }
}
```

## 第三章：深入理解依赖注入

### 3.1 为什么要使用依赖注入？

依赖注入（DI）是实现控制反转（IoC）的一种方式，它有以下好处：
1. 降低代码耦合度
2. 便于单元测试
3. 提高代码可维护性

### 3.2 在框架中使用依赖注入

```csharp
// Program.cs 或 Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // 注册DbContext
    services.AddDbContext<AppDbContext>();
    
    // 注册仓储
    services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
    services.AddScoped<ITodoRepository, TodoRepository>();
    
    // 注册服务
    services.AddScoped<ITodoService, TodoService>();
}
```

生命周期说明：
- **Transient**：每次请求都创建新实例
- **Scoped**：每个请求作用域创建一个实例
- **Singleton**：整个应用程序只创建一个实例

## 第四章：异常处理和日志记录

### 4.1 全局异常处理

框架使用中间件统一处理异常：
```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

### 4.2 日志记录

使用内置的ILogger接口记录日志：
```csharp
public class TodoService
{
    private readonly ILogger<TodoService> _logger;
    
    public async Task<Todo> CreateAsync(Todo todo)
    {
        _logger.LogInformation($"Creating new todo: {todo.Title}");
        // 创建逻辑
    }
}
```

## 第五章：高级特性

### 5.1 缓存机制

框架支持多级缓存：
1. 内存缓存（IMemoryCache）
2. 分布式缓存（Redis）

### 5.2 验证机制

使用FluentValidation进行数据验证：
```csharp
public class TodoValidator : AbstractValidator<Todo>
{
    public TodoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);
            
        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.Now)
            .When(x => x.DueDate.HasValue);
    }
}
```

## 实战练习

1. 创建一个新的实体（如Product）
2. 实现完整的CRUD操作
3. 添加自定义业务逻辑
4. 实现缓存机制
5. 添加单元测试

## 调试技巧

1. 使用断点调试服务层逻辑
2. 查看EF Core生成的SQL语句
3. 使用日志追踪请求流程

## 最佳实践

1. 始终使用异步方法
2. 正确处理异常
3. 使用适当的日志级别
4. 遵循代码规范
5. 编写单元测试

## 下一步学习

1. 深入了解Entity Framework Core
2. 学习高级LINQ查询
3. 掌握性能优化技巧
4. 了解分布式系统设计

记住：学习过程中遇到问题时，可以：
1. 查看源代码中的注释和文档
2. 使用调试器跟踪代码执行
3. 查看日志输出
4. 编写简单的测试代码验证想法

祝你学习愉快！如有任何问题，随时提出。
