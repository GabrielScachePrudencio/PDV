# 🧾 PDV Lanchonete – WPF + ASP.NET + MOBILE

Sistema de **PDV (Ponto de Venda)** desenvolvido em **C# com WPF**, integrado a um **servidor ASP.NET**, responsável por gerenciar pedidos, produtos, vendas e relatórios.

O projeto foi pensado para funcionar de forma distribuída, permitindo que diferentes plataformas (desktop, web e futuramente mobile) consumam a mesma API.

---

## 🏗️ Arquitetura do Sistema

O sistema é dividido em três partes principais:

### 🖥️ PDV Desktop (WPF)
- Interface usada no caixa
- Cadastro e seleção de produtos
- Criação de pedidos
- Finalização de vendas
- Comunicação com o servidor via HTTP (API REST)

### 🌐 Servidor (ASP.NET / ASP.NET Core)
- API REST central
- Regras de negócio
- Validação de dados
- Autenticação (opcional)
- Comunicação com o banco de dados

### 🗄️ Banco de Dados
- Armazena produtos, pedidos, itens, usuários e vendas
- Pode ser SQL Server, MySQL ou outro compatível

---

## 🔄 Fluxo de Funcionamento

1. O PDV (WPF) envia requisições HTTP para a API
2. A API processa as regras de negócio
3. Os dados são salvos ou consultados no banco
4. A API retorna as respostas para o PDV
5. O PDV atualiza a interface em tempo real

---

## 🧪 Tecnologias Utilizadas

### Backend
- C#
- ASP.NET / ASP.NET Core
- Entity Framework
- API REST
- JSON

### Frontend Desktop
- C#
- WPF
- MVVM
- HttpClient

### Banco de Dados
- SQL Server (ou equivalente)

---

## 📁 Estrutura do Projeto

```txt
/PDV-WPF
 ├── Views
 ├── ViewModels
 ├── Models
 ├── Services
 └── App.xaml

/Servidor-ASPNet
 ├── Controllers
 ├── Models
 ├── Services
 ├── Repositories
 └── Program.cs
