#!/usr/bin/env pwsh
<#
.SYNOPSIS
    快速项目创建脚本 - 支持多种项目类型

.DESCRIPTION
    这个脚本可以快速创建各种类型的项目，包括：
    - .NET项目 (webapp, webapi, console, classlib等)
    - Node.js项目 (React, Vue, Express, Next.js等)
    - Python项目 (Flask, Django, FastAPI, Streamlit等)
    - 区块链项目 (Hardhat, Foundry, Truffle等)
    - 移动应用项目 (React Native, Expo, Flutter等)

.PARAMETER ProjectType
    项目类型，例如：react, vue, webapi, flask等

.PARAMETER ProjectName
    项目名称

.PARAMETER TargetDirectory
    目标目录（可选，默认为当前目录）

.EXAMPLE
    .\create_project.ps1 -ProjectType react -ProjectName my-app
    .\create_project.ps1 -ProjectType webapi -ProjectName my-api
    .\create_project.ps1 -ProjectType flask -ProjectName my-flask-app
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectType,
    
    [Parameter(Mandatory=$true)]
    [string]$ProjectName,
    
    [Parameter(Mandatory=$false)]
    [string]$TargetDirectory = (Get-Location).Path
)

# 颜色输出函数
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

# 检查项目名称是否有效
if ($ProjectName -match '[<>:"/\\|?*]') {
    Write-ColorOutput "❌ 项目名称包含无效字符" "Red"
    exit 1
}

# 检查目标目录是否存在
if (!(Test-Path $TargetDirectory)) {
    Write-ColorOutput "❌ 目标目录不存在: $TargetDirectory" "Red"
    exit 1
}

# 检查项目目录是否已存在
$projectPath = Join-Path $TargetDirectory $ProjectName
if (Test-Path $projectPath) {
    Write-ColorOutput "❌ 项目目录已存在: $projectPath" "Red"
    exit 1
}

Write-ColorOutput "🚀 开始创建项目..." "Green"
Write-ColorOutput "项目类型: $ProjectType" "Cyan"
Write-ColorOutput "项目名称: $ProjectName" "Cyan"
Write-ColorOutput "目标目录: $TargetDirectory" "Cyan"
Write-ColorOutput "项目路径: $projectPath" "Cyan"
Write-Host ""

try {
    $success = $false
    $errorMessage = ""

    # .NET 项目
    if ($ProjectType -match "^(webapp|webapi|console|classlib|mvc|blazorserver|blazorwasm)$") {
        Write-ColorOutput "📦 创建 .NET $ProjectType 项目..." "Yellow"
        $result = & dotnet new $ProjectType -n $ProjectName -o $projectPath
        if ($LASTEXITCODE -eq 0) {
            $success = $true
        } else {
            $errorMessage = "dotnet new 命令执行失败"
        }
    }
    # Node.js 项目
    elseif ($ProjectType -eq "react") {
        Write-ColorOutput "⚛️ 创建 React 项目..." "Yellow"
        Push-Location $TargetDirectory
        try {
            $result = & npx create-react-app $ProjectName --yes
            if ($LASTEXITCODE -eq 0) {
                $success = $true
            } else {
                $errorMessage = "create-react-app 命令执行失败"
            }
        } finally {
            Pop-Location
        }
    }
    elseif ($ProjectType -eq "vue") {
        Write-ColorOutput "💚 创建 Vue 项目..." "Yellow"
        Push-Location $TargetDirectory
        try {
            $result = & npm create vue@latest $ProjectName -- --yes
            if ($LASTEXITCODE -eq 0) {
                $success = $true
            } else {
                $errorMessage = "create vue 命令执行失败"
            }
        } finally {
            Pop-Location
        }
    }
    elseif ($ProjectType -eq "next") {
        Write-ColorOutput "⚡ 创建 Next.js 项目..." "Yellow"
        Push-Location $TargetDirectory
        try {
            $result = & npx create-next-app@latest $ProjectName --yes
            if ($LASTEXITCODE -eq 0) {
                $success = $true
            } else {
                $errorMessage = "create-next-app 命令执行失败"
            }
        } finally {
            Pop-Location
        }
    }
    elseif ($ProjectType -eq "express") {
        Write-ColorOutput "🚂 创建 Express 项目..." "Yellow"
        Push-Location $TargetDirectory
        try {
            $result = & npx create-express-app $ProjectName --yes
            if ($LASTEXITCODE -eq 0) {
                $success = $true
            } else {
                $errorMessage = "create-express-app 命令执行失败"
            }
        } finally {
            Pop-Location
        }
    }
    # Python 项目
    elseif ($ProjectType -eq "flask") {
        Write-ColorOutput "🐍 创建 Flask 项目..." "Yellow"
        if (Get-Command python -ErrorAction SilentlyContinue) {
            Push-Location $TargetDirectory
            try {
                $result = & python -m flask startproject $ProjectName
                if ($LASTEXITCODE -eq 0) {
                    $success = $true
                } else {
                    $errorMessage = "Flask 项目创建失败"
                }
            } finally {
                Pop-Location
            }
        } else {
            $errorMessage = "Python 未安装或不在 PATH 中"
        }
    }
    elseif ($ProjectType -eq "django") {
        Write-ColorOutput "🐍 创建 Django 项目..." "Yellow"
        if (Get-Command django-admin -ErrorAction SilentlyContinue) {
            Push-Location $TargetDirectory
            try {
                $result = & django-admin startproject $ProjectName
                if ($LASTEXITCODE -eq 0) {
                    $success = $true
                } else {
                    $errorMessage = "Django 项目创建失败"
                }
            } finally {
                Pop-Location
            }
        } else {
            $errorMessage = "Django 未安装或不在 PATH 中"
        }
    }
    elseif ($ProjectType -eq "streamlit") {
        Write-ColorOutput "🎨 创建 Streamlit 项目..." "Yellow"
        $success = Create-StreamlitProject
    }
    # 区块链项目
    elseif ($ProjectType -eq "hardhat") {
        Write-ColorOutput "🔗 创建 Hardhat 项目..." "Yellow"
        Push-Location $TargetDirectory
        try {
            $result = & npx hardhat@latest init $ProjectName --yes
            if ($LASTEXITCODE -eq 0) {
                $success = $true
            } else {
                $errorMessage = "Hardhat 项目创建失败"
            }
        } finally {
            Pop-Location
        }
    }
    elseif ($ProjectType -eq "foundry") {
        Write-ColorOutput "🔗 创建 Foundry 项目..." "Yellow"
        if (Get-Command forge -ErrorAction SilentlyContinue) {
            Push-Location $TargetDirectory
            try {
                $result = & forge init $ProjectName
                if ($LASTEXITCODE -eq 0) {
                    $success = $true
                } else {
                    $errorMessage = "Foundry 项目创建失败"
                }
            } finally {
                Pop-Location
            }
        } else {
            $errorMessage = "Foundry 未安装或不在 PATH 中"
        }
    }
    # 移动应用项目
    elseif ($ProjectType -eq "react-native") {
        Write-ColorOutput "📱 创建 React Native 项目..." "Yellow"
        Push-Location $TargetDirectory
        try {
            $result = & npx react-native@latest init $ProjectName --yes
            if ($LASTEXITCODE -eq 0) {
                $success = $true
            } else {
                $errorMessage = "React Native 项目创建失败"
            }
        } finally {
            Pop-Location
        }
    }
    elseif ($ProjectType -eq "expo") {
        Write-ColorOutput "📱 创建 Expo 项目..." "Yellow"
        Push-Location $TargetDirectory
        try {
            $result = & npx create-expo-app@latest $ProjectName --yes
            if ($LASTEXITCODE -eq 0) {
                $success = $true
            } else {
                $errorMessage = "Expo 项目创建失败"
            }
        } finally {
            Pop-Location
        }
    }
    else {
        $errorMessage = "不支持的项目类型: $ProjectType"
    }

    if ($success) {
        Write-ColorOutput "✅ 项目创建成功！" "Green"
        Write-ColorOutput "📁 项目路径: $projectPath" "Cyan"
        
        # 显示项目信息
        if (Test-Path $projectPath) {
            Write-Host ""
            Write-ColorOutput "📋 项目内容:" "Yellow"
            Get-ChildItem $projectPath -Recurse | ForEach-Object {
                $relativePath = $_.FullName.Substring($projectPath.Length + 1)
                if ($_.PSIsContainer) {
                    Write-ColorOutput "  📁 $relativePath\" "Blue"
                } else {
                    Write-ColorOutput "  📄 $relativePath" "White"
                }
            }
        }
        
        Write-Host ""
        Write-ColorOutput "🎉 项目创建完成！" "Green"
        Write-ColorOutput "💡 提示: 进入项目目录并查看 README 文件了解如何运行项目" "Yellow"
    } else {
        Write-ColorOutput "❌ 项目创建失败: $errorMessage" "Red"
        exit 1
    }
}
catch {
    Write-ColorOutput "❌ 创建项目时发生异常: $($_.Exception.Message)" "Red"
    exit 1
}

# 创建 Streamlit 项目的辅助函数
function Create-StreamlitProject {
    try {
        # 创建项目目录
        New-Item -ItemType Directory -Path $projectPath -Force | Out-Null
        
        # 创建 requirements.txt
        $requirementsPath = Join-Path $projectPath "requirements.txt"
        @"
streamlit
pandas
numpy
matplotlib
"@ | Out-File -FilePath $requirementsPath -Encoding UTF8
        
        # 创建主应用文件
        $appPath = Join-Path $projectPath "app.py"
        @"
import streamlit as st
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt

st.set_page_config(
    page_title="$ProjectName",
    page_icon="🚀",
    layout="wide"
)

st.title("🚀 $ProjectName")
st.write("这是一个 Streamlit 应用项目")

# 侧边栏
st.sidebar.header("设置")
chart_type = st.sidebar.selectbox(
    "选择图表类型",
    ["折线图", "柱状图", "散点图"]
)

# 主内容
col1, col2 = st.columns(2)

with col1:
    st.header("数据生成")
    n_points = st.slider("数据点数量", 10, 100, 50)
    
    # 生成示例数据
    dates = pd.date_range("2024-01-01", periods=n_points, freq="D")
    data = np.random.randn(n_points).cumsum()
    df = pd.DataFrame({"日期": dates, "数值": data})
    
    st.dataframe(df.head(10))

with col2:
    st.header("数据可视化")
    
    fig, ax = plt.subplots(figsize=(10, 6))
    
    if chart_type == "折线图":
        ax.plot(df["日期"], df["数值"])
        ax.set_title("时间序列数据")
    elif chart_type == "柱状图":
        ax.bar(range(len(df)), df["数值"])
        ax.set_title("柱状图")
    else:
        ax.scatter(range(len(df)), df["数值"])
        ax.set_title("散点图")
    
    plt.xticks(rotation=45)
    plt.tight_layout()
    st.pyplot(fig)

# 下载数据
st.header("数据下载")
csv = df.to_csv(index=False)
st.download_button(
    label="下载 CSV 文件",
    data=csv,
    file_name="data.csv",
    mime="text/csv"
)

st.success("🎉 应用运行成功！")
"@ | Out-File -FilePath $appPath -Encoding UTF8
        
        # 创建 README
        $readmePath = Join-Path $projectPath "README.md"
        @"
# $ProjectName

这是一个 Streamlit 应用项目。

## 🚀 快速开始

### 安装依赖
```bash
pip install -r requirements.txt
```

### 运行应用
```bash
streamlit run app.py
```

## 📁 项目结构

```
$ProjectName/
├── app.py              # 主应用文件
├── requirements.txt    # Python 依赖
└── README.md          # 项目说明
```

## ✨ 功能特性

- 📊 数据可视化
- 🎨 交互式界面
- 📱 响应式设计
- 📥 数据下载
- ⚙️ 可配置图表类型

## 🛠️ 技术栈

- **Streamlit** - Web 应用框架
- **Pandas** - 数据处理
- **NumPy** - 数值计算
- **Matplotlib** - 数据可视化

## 📚 学习资源

- [Streamlit 官方文档](https://docs.streamlit.io/)
- [Pandas 教程](https://pandas.pydata.org/docs/)
- [Matplotlib 指南](https://matplotlib.org/stable/tutorials/)

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📄 许可证

MIT License
"@ | Out-File -FilePath $readmePath -Encoding UTF8
        
        return $true
    }
    catch {
        Write-ColorOutput "❌ 创建 Streamlit 项目失败: $($_.Exception.Message)" "Red"
        return $false
    }
} 