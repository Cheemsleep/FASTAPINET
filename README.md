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

## 贡献指南

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启一个 Pull Request

## 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情
