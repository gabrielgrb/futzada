#!/bin/bash
# =============================================================
# Fut de Quarta - Rodar Backend + Frontend juntos
# =============================================================
set -e

cd "$(dirname "$0")"
PROJETO_ROOT="$(pwd)"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

cleanup() {
    echo ""
    echo -e "${YELLOW}Parando servicos...${NC}"
    kill $PID_BACKEND 2>/dev/null || true
    kill $PID_FRONTEND 2>/dev/null || true
    echo -e "${GREEN}Tudo parado.${NC}"
    exit 0
}

trap cleanup SIGINT SIGTERM

echo "=========================================="
echo "  Fut de Quarta - Iniciando"
echo "=========================================="
echo ""

# Start Backend
echo -e "${GREEN}>> Iniciando Backend (.NET 8)...${NC}"
cd "$PROJETO_ROOT/backend"
dotnet run --project FutDeQuarta.Api --urls "http://localhost:5000" &
PID_BACKEND=$!
echo "   PID Backend: $PID_BACKEND"

# Wait for backend to be ready
echo -n "   Aguardando backend"
for i in $(seq 1 30); do
    if curl -s http://localhost:5000/swagger/index.html > /dev/null 2>&1; then
        echo ""
        echo -e "   ${GREEN}Backend pronto!${NC}"
        break
    fi
    echo -n "."
    sleep 1
done
echo ""

# Start Frontend
echo -e "${GREEN}>> Iniciando Frontend (Angular + Ionic)...${NC}"
cd "$PROJETO_ROOT/frontend"
npx ng serve --port 4200 --open &
PID_FRONTEND=$!
echo "   PID Frontend: $PID_FRONTEND"

echo ""
echo "=========================================="
echo -e "${GREEN}  Tudo rodando!${NC}"
echo "=========================================="
echo ""
echo "  API:     http://localhost:5000"
echo "  Swagger: http://localhost:5000/swagger"
echo "  App:     http://localhost:4200"
echo ""
echo "  Pressione Ctrl+C para parar tudo"
echo ""

# Wait for either process to exit
wait $PID_BACKEND $PID_FRONTEND
