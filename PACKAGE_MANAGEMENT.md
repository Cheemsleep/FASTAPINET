# .NET Core 中央包管理指南

## 概述
本项目使用 .NET Core 的中央包管理（Central Package Management，CPM）功能来统一管理所有项目的 NuGet 包版本。这种方式类似于 Maven 的依赖管理，可以确保所有项目使用相同版本的包，减少版本冲突。

## 关键文件

### 1. Directory.Build.props
位于解决方案根目录，用于启用中央包管理并设置通用项目属性：
```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

### 2. Directory.Packages.props
位于解决方案根目录，集中管理所有 NuGet 包的版本：
```xml
<Project>
  <PropertyGroup>
    <CentralPackageVersionMajor>8</CentralPackageVersionMajor>
    <CentralPackageVersionMinor>0</CentralPackageVersionMinor>
    <CentralPackageVersionPatch>0</CentralPackageVersionPatch>
    <CentralPackageVersion>$(CentralPackageVersionMajor).$(CentralPackageVersionMinor).$(CentralPackageVersionPatch)</CentralPackageVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="Package.Name" Version="x.y.z" />
  </ItemGroup>
</Project>
```

## 常用操作

### 添加新包

1. **命令行方式**：
   ```bash
   dotnet add package PackageName
   ```

2. **手动方式**：
   1. 在 Directory.Packages.props 中添加版本定义：
   ```xml
   <PackageVersion Include="PackageName" Version="x.y.z" />
   ```
   2. 在项目的 .csproj 文件中添加引用：
   ```xml
   <PackageReference Include="PackageName" />
   ```

### 删除包

1. **命令行方式**：
   ```bash
   dotnet remove package PackageName
   ```

2. **手动方式**：
   1. 从项目的 .csproj 文件中移除 PackageReference 行
   2. 如果所有项目都不再使用该包，从 Directory.Packages.props 中移除 PackageVersion 定义

### 更新包版本

1. 仅需在 Directory.Packages.props 中更新版本号：
   ```xml
   <PackageVersion Include="PackageName" Version="新版本号" />
   ```

2. 如果使用变量管理版本：
   ```xml
   <!-- 更新变量值 -->
   <CentralPackageVersionMajor>8</CentralPackageVersionMajor>
   ```

## 最佳实践

1. **版本统一**
   - 对于同一个包，在所有项目中使用相同版本
   - 使用变量管理常用包的版本号

2. **版本管理**
   - 将相关的包版本组织在一起（例如所有 EntityFramework 包）
   - 使用注释标注不同包组的用途

3. **项目文件（.csproj）**
   - 不要在项目文件中指定版本号
   - 保持项目文件简洁，只包含必要的包引用

4. **包更新**
   - 定期检查包更新
   - 在更新包版本前进行充分测试
   - 保持版本更新记录

## 常见问题

1. **Q: 包引用报错找不到版本？**
   A: 检查 Directory.Packages.props 中是否定义了该包的版本

2. **Q: 如何让某个项目使用不同版本的包？**
   A: 在该项目的 .csproj 中可以覆盖版本：
   ```xml
   <PackageReference Include="PackageName" Version="特定版本" />
   ```

3. **Q: 是否所有包都需要在 Directory.Packages.props 中定义？**
   A: 是的，使用中央包管理时，所有包的版本都应该在此定义

## 注意事项

1. 添加新包时，确保在 Directory.Packages.props 中定义版本
2. 删除包时，检查其他项目是否仍在使用
3. 更新版本时，考虑依赖关系
4. 保持版本定义文件的整洁和有序
5. 定期更新包版本以获取安全修复和新特性

## 相关资源

- [.NET Central Package Management 官方文档](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
- [NuGet 包管理最佳实践](https://learn.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files)
