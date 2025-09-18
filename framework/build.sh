#!/bin/bash

# ASP.NET Core 设计模式框架构建脚本

echo "开始构建 ASP.NET Core 设计模式框架..."

# 清理
echo "清理项目..."
dotnet clean

# 还原包
echo "还原 NuGet 包..."
dotnet restore

# 构建项目
echo "构建项目..."
dotnet build --configuration Release --no-restore

if [ $? -eq 0 ]; then
    echo "构建成功！"
    
    # 运行测试（如果有的话）
    echo "运行测试..."
    dotnet test --configuration Release --no-build --verbosity normal
    
    if [ $? -eq 0 ]; then
        echo "所有测试通过！"
    else
        echo "测试失败！"
    fi
    
    # 运行示例项目
    echo "运行示例项目..."
    cd samples/Framework.Samples
    dotnet run &
    cd ../..
    
else
    echo "构建失败！"
    exit 1
fi

echo "构建完成！"
