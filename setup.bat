@echo off
REM =============================================================
REM Fut de Quarta - Setup para Windows (sem WSL)
REM Execute como Administrador se necessario
REM =============================================================

echo ==========================================
echo   Fut de Quarta - Setup Windows
echo ==========================================
echo.

REM -----------------------------------------------------------
REM 1. Verificar Node.js
REM -----------------------------------------------------------
echo ^>^> Verificando Node.js...
where node >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [X] Node.js nao encontrado.
    echo     Baixe em: https://nodejs.org/en/download/
    echo     Instale o Node.js 22 LTS e rode este script novamente.
    pause
    exit /b 1
)
echo [OK] Node.js encontrado
node --version
echo.

REM -----------------------------------------------------------
REM 2. Verificar .NET SDK
REM -----------------------------------------------------------
echo ^>^> Verificando .NET SDK...
where dotnet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [X] .NET SDK nao encontrado.
    echo     Baixe em: https://dotnet.microsoft.com/download/dotnet/8.0
    echo     Instale o .NET 8 SDK e rode este script novamente.
    pause
    exit /b 1
)
echo [OK] .NET SDK encontrado
dotnet --version
echo.

REM -----------------------------------------------------------
REM 3. Instalar ferramentas globais
REM -----------------------------------------------------------
echo ^>^> Instalando ferramentas globais...

call npm list -g @angular/cli >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Instalando Angular CLI...
    call npm install -g @angular/cli@17
)
echo [OK] Angular CLI

call npm list -g @ionic/cli >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Instalando Ionic CLI...
    call npm install -g @ionic/cli
)
echo [OK] Ionic CLI

dotnet tool list -g | findstr "dotnet-ef" >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Instalando Entity Framework CLI...
    dotnet tool install --global dotnet-ef
)
echo [OK] dotnet-ef
echo.

REM -----------------------------------------------------------
REM 4. Setup Backend
REM -----------------------------------------------------------
echo ^>^> Configurando Backend...
cd /d "%~dp0backend"
dotnet restore
echo [OK] Backend - dependencias restauradas

if not exist "FutDeQuarta.Api\Migrations" (
    dotnet ef migrations add InitialCreate --project FutDeQuarta.Api
    echo [OK] Migration inicial criada
)
echo.

REM -----------------------------------------------------------
REM 5. Setup Frontend
REM -----------------------------------------------------------
echo ^>^> Configurando Frontend...
cd /d "%~dp0frontend"
call npm install
echo [OK] Frontend - dependencias instaladas
echo.

REM -----------------------------------------------------------
REM Done
REM -----------------------------------------------------------
echo ==========================================
echo   Setup completo!
echo ==========================================
echo.
echo Para rodar o projeto:
echo.
echo   Terminal 1 (Backend):
echo     cd backend
echo     dotnet run --project FutDeQuarta.Api
echo.
echo   Terminal 2 (Frontend):
echo     cd frontend
echo     npm start
echo.
echo   Endpoints:
echo     API:     http://localhost:5000
echo     Swagger: http://localhost:5000/swagger
echo     App:     http://localhost:4200
echo.
pause
