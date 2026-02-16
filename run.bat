@echo off
REM =============================================================
REM Fut de Quarta - Rodar Backend + Frontend (Windows)
REM =============================================================

echo ==========================================
echo   Fut de Quarta - Iniciando
echo ==========================================
echo.

echo ^>^> Iniciando Backend...
cd /d "%~dp0backend"
start "FutDeQuarta-Backend" cmd /k "dotnet run --project FutDeQuarta.Api --urls http://localhost:5000"

echo    Aguardando backend (5 segundos)...
timeout /t 5 /nobreak >nul

echo ^>^> Iniciando Frontend...
cd /d "%~dp0frontend"
start "FutDeQuarta-Frontend" cmd /k "npx ng serve --port 4200 --open"

echo.
echo ==========================================
echo   Tudo rodando!
echo ==========================================
echo.
echo   API:     http://localhost:5000
echo   Swagger: http://localhost:5000/swagger
echo   App:     http://localhost:4200
echo.
echo   Feche as janelas do terminal para parar.
echo.
pause
