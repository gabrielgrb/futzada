#!/bin/bash
# =============================================================
# Fut de Quarta - Script de Setup Completo
# Roda no Linux, macOS ou WSL (Windows)
# =============================================================
set -e

echo "=========================================="
echo "  Fut de Quarta - Setup Local"
echo "=========================================="
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

check_command() {
    if command -v "$1" &> /dev/null; then
        echo -e "${GREEN}[OK]${NC} $1 encontrado: $($1 --version 2>&1 | head -1)"
        return 0
    else
        echo -e "${RED}[X]${NC} $1 nao encontrado"
        return 1
    fi
}

# -----------------------------------------------------------
# 1. Verificar e instalar Node.js
# -----------------------------------------------------------
echo ">> Verificando Node.js..."
if ! check_command node; then
    echo -e "${YELLOW}Instalando Node.js 22 LTS...${NC}"
    if [[ "$OSTYPE" == "darwin"* ]]; then
        if command -v brew &> /dev/null; then
            brew install node@22
        else
            echo "Instale o Homebrew primeiro: https://brew.sh"
            exit 1
        fi
    elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
        curl -fsSL https://deb.nodesource.com/setup_22.x | sudo -E bash -
        sudo apt-get install -y nodejs
    else
        echo "Baixe o Node.js em: https://nodejs.org/en/download/"
        exit 1
    fi
    echo -e "${GREEN}Node.js instalado!${NC}"
fi
echo ""

# -----------------------------------------------------------
# 2. Verificar e instalar .NET 8 SDK
# -----------------------------------------------------------
echo ">> Verificando .NET SDK..."
if ! check_command dotnet; then
    echo -e "${YELLOW}Instalando .NET 8 SDK...${NC}"
    if [[ "$OSTYPE" == "darwin"* ]]; then
        if command -v brew &> /dev/null; then
            brew install dotnet-sdk
        else
            curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0
            export PATH="$HOME/.dotnet:$PATH"
            echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.bashrc
        fi
    elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
        # Try package manager first
        if command -v apt-get &> /dev/null; then
            # Add Microsoft package repository
            wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
            sudo dpkg -i packages-microsoft-prod.deb
            rm packages-microsoft-prod.deb
            sudo apt-get update
            sudo apt-get install -y dotnet-sdk-8.0
        else
            # Fallback to install script
            curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0
            export PATH="$HOME/.dotnet:$PATH"
            echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.bashrc
        fi
    else
        echo "Baixe o .NET 8 SDK em: https://dotnet.microsoft.com/download/dotnet/8.0"
        exit 1
    fi
    echo -e "${GREEN}.NET SDK instalado!${NC}"
fi

# Verify .NET version
DOTNET_VERSION=$(dotnet --version 2>/dev/null || echo "0")
if [[ ! "$DOTNET_VERSION" == 8.* ]]; then
    echo -e "${YELLOW}Aviso: .NET $DOTNET_VERSION detectado. Este projeto usa .NET 8.${NC}"
fi
echo ""

# -----------------------------------------------------------
# 3. Instalar ferramentas globais
# -----------------------------------------------------------
echo ">> Instalando ferramentas globais..."

# Angular CLI
if ! command -v ng &> /dev/null; then
    echo "Instalando Angular CLI..."
    npm install -g @angular/cli@17
fi
echo -e "${GREEN}[OK]${NC} Angular CLI"

# Ionic CLI
if ! command -v ionic &> /dev/null; then
    echo "Instalando Ionic CLI..."
    npm install -g @ionic/cli
fi
echo -e "${GREEN}[OK]${NC} Ionic CLI"

# EF Core tools
if ! dotnet tool list -g | grep -q "dotnet-ef"; then
    echo "Instalando Entity Framework CLI..."
    dotnet tool install --global dotnet-ef
fi
echo -e "${GREEN}[OK]${NC} dotnet-ef"
echo ""

# -----------------------------------------------------------
# 4. Setup do Backend
# -----------------------------------------------------------
echo ">> Configurando Backend..."
cd "$(dirname "$0")"
PROJETO_ROOT="$(pwd)"

cd "$PROJETO_ROOT/backend"
dotnet restore
echo -e "${GREEN}[OK]${NC} Backend - dependencias restauradas"

# Create initial migration if it doesn't exist
if [ ! -d "FutDeQuarta.Api/Migrations" ]; then
    dotnet ef migrations add InitialCreate --project FutDeQuarta.Api
    echo -e "${GREEN}[OK]${NC} Migration inicial criada"
fi
echo ""

# -----------------------------------------------------------
# 5. Setup do Frontend
# -----------------------------------------------------------
echo ">> Configurando Frontend..."
cd "$PROJETO_ROOT/frontend"
npm install
echo -e "${GREEN}[OK]${NC} Frontend - dependencias instaladas"
echo ""

# -----------------------------------------------------------
# Done!
# -----------------------------------------------------------
echo "=========================================="
echo -e "${GREEN}  Setup completo!${NC}"
echo "=========================================="
echo ""
echo "Para rodar o projeto:"
echo ""
echo "  Terminal 1 (Backend):"
echo "    cd backend && dotnet run --project FutDeQuarta.Api"
echo ""
echo "  Terminal 2 (Frontend):"
echo "    cd frontend && npm start"
echo ""
echo "  Endpoints:"
echo "    API:     http://localhost:5000"
echo "    Swagger: http://localhost:5000/swagger"
echo "    App:     http://localhost:4200"
echo ""
