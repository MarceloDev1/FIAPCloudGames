# FIAP Cloud Games (FCG) - Plataforma de Jogos Educacionais

![.NET](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

API REST para gerenciamento de usuários e jogos educacionais em tecnologia, desenvolvida como parte do Tech Challenge da FIAP.

## 📋 Funcionalidades Principais

### 🔐 Autenticação e Autorização
- Registro de usuários com validação de e-mail e senha segura
- Autenticação via JWT
- Dois níveis de acesso: Usuário e Administrador

### 🎮 Gerenciamento de Jogos (Admin)
- CRUD completo de jogos
- Atualização específica de preços
- Catálogo de jogos educacionais

### 👥 Gerenciamento de Usuários
- Criação e atualização de perfis
- Controle de acesso baseado em roles

## 🛠️ Tecnologias Utilizadas
- .NET 8
- Entity Framework Core
- JWT Authentication
- xUnit (testes unitários)
- Swagger/OpenAPI

## 🏗️ Estrutura do Projeto

```
FIAPCloudGames/
├── Controllers/       # Endpoints da API
├── Services/          # Lógica de negócios
├── Models/            # DTOs e Entidades
├── Data/              # Camada de dados
├── Middleware/        # Middlewares customizados
└── UnitTests/         # Testes automatizados
```

## 🚀 Como Executar

### Pré-requisitos
- .NET 8 SDK
- SQL Server (ou compatível)

### Configuração
1. Clone o repositório
2. Configure a connection string em `appsettings.json`
3. Execute as migrations:
   ```bash
   dotnet ef database update
   ```
4. Inicie a aplicação:
   ```bash
   dotnet run
   ```
5. Acesse a documentação Swagger em `https://localhost:<port>/swagger`

## 🧪 Testes
- Cobertura mínima de 70% nos serviços principais
- Testes de controllers e serviços
- Execute com:
  ```bash
  dotnet test
  ```

## 📄 Licença
Este projeto está licenciado sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.
