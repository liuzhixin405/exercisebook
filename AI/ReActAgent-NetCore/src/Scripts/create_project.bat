@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo.
echo ========================================
echo 🚀 快速项目创建工具
echo ========================================
echo.

if "%1"=="" (
    echo 使用方法: create_project.bat [项目类型] [项目名称]
    echo.
    echo 支持的项目类型:
    echo.
    echo 📦 .NET 项目:
    echo   webapp    - ASP.NET Core Web应用
    echo   webapi    - ASP.NET Core Web API
    echo   console   - 控制台应用
    echo   classlib  - 类库
    echo   mvc       - MVC应用
    echo.
    echo ⚛️  Node.js 项目:
    echo   react     - React应用
    echo   vue       - Vue应用
    echo   express   - Express.js应用
    echo   next      - Next.js应用
    echo.
    echo 🐍 Python 项目:
    echo   flask     - Flask应用
    echo   django    - Django应用
    echo   streamlit - Streamlit应用
    echo.
    echo 🔗 区块链项目:
    echo   hardhat   - Hardhat项目
    echo   foundry   - Foundry项目
    echo.
    echo 📱 移动应用:
    echo   react-native - React Native应用
    echo   expo        - Expo应用
    echo.
    echo 示例:
    echo   create_project.bat react my-app
    echo   create_project.bat webapi my-api
    echo   create_project.bat flask my-flask-app
    echo.
    pause
    exit /b 1
)

set PROJECT_TYPE=%1
set PROJECT_NAME=%2

if "%PROJECT_NAME%"=="" (
    echo ❌ 错误: 请提供项目名称
    echo 使用方法: create_project.bat [项目类型] [项目名称]
    pause
    exit /b 1
)

echo 🚀 开始创建项目...
echo 项目类型: %PROJECT_TYPE%
echo 项目名称: %PROJECT_NAME%
echo 当前目录: %CD%
echo.

set PROJECT_PATH=%CD%\%PROJECT_NAME%

if exist "%PROJECT_PATH%" (
    echo ❌ 错误: 项目目录已存在: %PROJECT_PATH%
    pause
    exit /b 1
)

set SUCCESS=false
set ERROR_MSG=

echo 📦 正在创建项目...

REM .NET 项目
if /i "%PROJECT_TYPE%"=="webapp" (
    echo 创建 ASP.NET Core Web 应用...
    dotnet new webapp -n %PROJECT_NAME% -o %PROJECT_PATH%
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=dotnet new webapp 命令执行失败
) else if /i "%PROJECT_TYPE%"=="webapi" (
    echo 创建 ASP.NET Core Web API...
    dotnet new webapi -n %PROJECT_NAME% -o %PROJECT_PATH%
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=dotnet new webapi 命令执行失败
) else if /i "%PROJECT_TYPE%"=="console" (
    echo 创建控制台应用...
    dotnet new console -n %PROJECT_NAME% -o %PROJECT_PATH%
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=dotnet new console 命令执行失败
) else if /i "%PROJECT_TYPE%"=="classlib" (
    echo 创建类库...
    dotnet new classlib -n %PROJECT_NAME% -o %PROJECT_PATH%
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=dotnet new classlib 命令执行失败
) else if /i "%PROJECT_TYPE%"=="mvc" (
    echo 创建 MVC 应用...
    dotnet new mvc -n %PROJECT_NAME% -o %PROJECT_PATH%
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=dotnet new mvc 命令执行失败
) else if /i "%PROJECT_TYPE%"=="blazorserver" (
    echo 创建 Blazor Server 应用...
    dotnet new blazorserver -n %PROJECT_NAME% -o %PROJECT_PATH%
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=dotnet new blazorserver 命令执行失败
) else if /i "%PROJECT_TYPE%"=="blazorwasm" (
    echo 创建 Blazor WebAssembly 应用...
    dotnet new blazorwasm -n %PROJECT_NAME% -o %PROJECT_PATH%
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=dotnet new blazorwasm 命令执行失败
REM Node.js 项目
) else if /i "%PROJECT_TYPE%"=="react" (
    echo 创建 React 应用...
    cd /d "%PROJECT_PATH%"
    npx create-react-app %PROJECT_NAME% --yes
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=create-react-app 命令执行失败
) else if /i "%PROJECT_TYPE%"=="vue" (
    echo 创建 Vue 应用...
    cd /d "%PROJECT_PATH%"
    npm create vue@latest %PROJECT_NAME% -- --yes
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=create vue 命令执行失败
) else if /i "%PROJECT_TYPE%"=="next" (
    echo 创建 Next.js 应用...
    cd /d "%PROJECT_PATH%"
    npx create-next-app@latest %PROJECT_NAME% --yes
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=create-next-app 命令执行失败
) else if /i "%PROJECT_TYPE%"=="express" (
    echo 创建 Express 应用...
    cd /d "%PROJECT_PATH%"
    npx create-express-app %PROJECT_NAME% --yes
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=create-express-app 命令执行失败
REM Python 项目
) else if /i "%PROJECT_TYPE%"=="flask" (
    echo 创建 Flask 应用...
    cd /d "%PROJECT_PATH%"
    python -m flask startproject %PROJECT_NAME%
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=Flask 项目创建失败
) else if /i "%PROJECT_TYPE%"=="django" (
    echo 创建 Django 应用...
    cd /d "%PROJECT_PATH%"
    django-admin startproject %PROJECT_NAME%
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=Django 项目创建失败
) else if /i "%PROJECT_TYPE%"=="streamlit" (
    echo 创建 Streamlit 应用...
    call :CreateStreamlitProject
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=Streamlit 项目创建失败
REM 区块链项目
) else if /i "%PROJECT_TYPE%"=="hardhat" (
    echo 创建 Hardhat 项目...
    cd /d "%PROJECT_PATH%"
    npx hardhat@latest init %PROJECT_NAME% --yes
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=Hardhat 项目创建失败
) else if /i "%PROJECT_TYPE%"=="foundry" (
    echo 创建 Foundry 项目...
    cd /d "%PROJECT_PATH%"
    forge init %PROJECT_NAME%
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=Foundry 项目创建失败
REM 移动应用
) else if /i "%PROJECT_TYPE%"=="react-native" (
    echo 创建 React Native 应用...
    cd /d "%PROJECT_PATH%"
    npx react-native@latest init %PROJECT_NAME% --yes
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=React Native 项目创建失败
) else if /i "%PROJECT_TYPE%"=="expo" (
    echo 创建 Expo 应用...
    cd /d "%PROJECT_PATH%"
    npx create-expo-app@latest %PROJECT_NAME% --yes
    if !errorlevel! equ 0 set SUCCESS=true
    if !errorlevel! neq 0 set ERROR_MSG=Expo 项目创建失败
) else (
    set ERROR_MSG=不支持的项目类型: %PROJECT_TYPE%
)

if "%SUCCESS%"=="true" (
    echo.
    echo ✅ 项目创建成功！
    echo 📁 项目路径: %PROJECT_PATH%
    echo.
    
    if exist "%PROJECT_PATH%" (
        echo 📋 项目内容:
        dir /b /s "%PROJECT_PATH%"
    )
    
    echo.
    echo 🎉 项目创建完成！
    echo 💡 提示: 进入项目目录并查看 README 文件了解如何运行项目
) else (
    echo.
    echo ❌ 项目创建失败: %ERROR_MSG%
    pause
    exit /b 1
)

echo.
pause
exit /b 0

:CreateStreamlitProject
echo 创建 Streamlit 项目目录...
mkdir "%PROJECT_NAME%"
cd "%PROJECT_NAME%"

echo 创建 requirements.txt...
echo streamlit> requirements.txt
echo pandas>> requirements.txt
echo numpy>> requirements.txt
echo matplotlib>> requirements.txt

echo 创建 app.py...
echo import streamlit as st> app.py
echo import pandas as pd>> app.py
echo import numpy as np>> app.py
echo import matplotlib.pyplot as plt>> app.py
echo.>> app.py
echo st.set_page_config(page_title="%PROJECT_NAME%", page_icon="🚀", layout="wide")>> app.py
echo st.title("🚀 %PROJECT_NAME%")>> app.py
echo st.write("这是一个 Streamlit 应用项目")>> app.py
echo.>> app.py
echo # 生成示例数据>> app.py
echo dates = pd.date_range("2024-01-01", periods=50, freq="D")>> app.py
echo data = np.random.randn(50).cumsum()>> app.py
echo df = pd.DataFrame({"日期": dates, "数值": data})>> app.py
echo.>> app.py
echo st.dataframe(df.head(10))>> app.py
echo st.line_chart(df.set_index("日期"))>> app.py
echo st.success("🎉 应用运行成功！")>> app.py

echo 创建 README.md...
echo # %PROJECT_NAME%> README.md
echo.>> README.md
echo 这是一个 Streamlit 应用项目。>> README.md
echo.>> README.md
echo ## 运行方法>> README.md
echo.>> README.md
echo ```bash>> README.md
echo pip install -r requirements.txt>> README.md
echo streamlit run app.py>> README.md
echo ```>> README.md

cd ..
exit /b 0 