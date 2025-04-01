# My Book Shelf

![PT](https://img.shields.io/badge/🇧🇷-Português-green)
## Descrição
O **My Book Shelf** é uma aplicação web desenvolvida para auxiliar os utilizadores a criar e manter o hábito da leitura. Com esta ferramenta, os utilizadores podem:

- Gerir listas de livros.
- Acompanhar o progresso de leitura.
- Visualizar gráficos com estatísticas de leitura.

O projeto foi desenvolvido utilizando **C# e Blazor Web App**, e utiliza **SQL Server** como base de dados.

---

## Estrutura do Código
O projeto está organizado em diversas camadas para facilitar a manutenção e o desenvolvimento. Aqui está um resumo da estrutura de pastas:

```
MyBookShelf/
│── Components/        # Componentes reutilizáveis (layouts, menus, etc.)
│── Pages/             # Páginas principais (login, registo, etc.)
│── Controllers/       # Controladores backend para CRUD
│── Models/            # Entidades da base de dados e ViewModels
│── Data/              # Configuração do Entity Framework (AppDbContext.cs)
│── wwwroot/           # Arquivos estáticos (imagens, estilos, etc.)
│── Services/          # Serviços reutilizáveis e lógica de negócios
│── Properties/        # Arquivos de configuração (launchSettings.json)
│── Dependencies/      # Pacotes NuGet utilizados
```

### Arquivos Principais
- **AppDbContext.cs**: Configuração do contexto da base de dados.
- **BooksController.cs e UsersController.cs**: Controladores responsáveis pelo CRUD de livros e utilizadores.
- **Reading.razor**: Página para acompanhamento do progresso de leitura.
- **Program.cs**: Configuração inicial da aplicação.

---

## Tecnologias Utilizadas

- **Blazor Web App** - Framework para desenvolvimento da aplicação.
- **Entity Framework Core (EF Core)** - Acesso à base de dados SQL Server.
- **Bootstrap** - Interface gráfica e componentes visuais.
- **SQL Server** - Base de dados relacional.

---

![EN](https://img.shields.io/badge/🇬🇧-English-blue)

## Overview
**My Book Shelf** is a web application designed to help users build and maintain reading habits. This tool enables readers to:
- Organize their book collections
- Track reading progress
- View detailed reading analytics and statistics

Built with **C# and Blazor Web App** with **SQL Server** database integration.

---

## Project Structure
The codebase follows a layered architecture for better maintainability:

```
MyBookShelf/
│── Components/        # UI building blocks (layouts, menus, etc.)
│── Pages/             # Core pages (login, signup, etc.)
│── Controllers/       # Backend CRUD operations
│── Models/            # Data entities and ViewModels
│── Data/              # Entity Framework setup (AppDbContext.cs)
│── wwwroot/           # Static assets (images, CSS, etc.)
│── Services/          # Business logic and shared services
│── Properties/        # Config settings (launchSettings.json)
│── Dependencies/      # NuGet package references
```

### Key Files
- **AppDbContext.cs**: Database configuration
- **BooksController.cs & UsersController.cs**: Book and user management logic
- **Reading.razor**: Reading progress tracker interface
- **Program.cs**: Application startup configuration

---

## Tech Stack
- **Blazor Web App** - Frontend and backend framework
- **Entity Framework Core** - SQL Server data access layer
- **Bootstrap** - UI components and styling
- **SQL Server** - Relational database storage

---
