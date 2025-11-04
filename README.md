# Chimera
# Chimera
Trabalho 

Nome do projeto: Chimera
Nome dos integrantes: 

Otávio Amâncio N Gonçalves 12302856, 

Gabriel Horta Lima 12401145, 

Luiz Henrique 12303135, 

Rafael Correia Costa 12303461, 

João Augusto M V De Souza 12302465,

Arthur Lorentz Duarte 12301523.

-----
Instruções:
  Criar uma controller para cada classe da model;
  Banco de Dados 
------
Atualização:
Alterei o repositorio do back-end para o novo mas ainda ta dando alguns erros que tenho que concertar. Depois so lincar com o front

---
## Requisitos Funcionais Implementados (20 RFs)

Abaixo estão listados os 20 requisitos funcionais desenvolvidos no projeto, com base na prioridade definida:

| ID | Descrição do Requisito Funcional | Prioridade | Implementação Evidenciada |
|:---|:---|:---|:---|
| RF01 | O sistema deve permitir o **cadastro de usuários**. | Alta | Controllers e páginas de Cadastro. |
| RF02 | O usuário deve poder **redefinir a senha**. | Baixo | Endpoints e páginas para Recuperação e Redefinição de Senha. |
| RF03 | O sistema possui a estrutura de banco para **enviar notificações de lembretes**. | Alta | Tabela `notification` no esquema SQL. |
| RF04 | O usuário deve conseguir **visualizar um histórico de streaming**. | Média | Tabela `access_history` no esquema SQL. |
| RF05 | O sistema deve permitir **integração com redes sociais**. | Baixa | Tabela `social_network_integration` no esquema SQL. |
| RF06 | O usuário deve poder **personalizar as configurações da interface** (via tela de perfil). | Média | Estrutura da página `perfil.html` com seções de configurações. |
| RF07 | O sistema deve permitir o **login e logout de usuários**. | Alta | Endpoint `login` no `UsuariosController` e lógica de front-end em `header.js`. |
| RF08 | O sistema deve permitir o **envio de e-mail** (utilizado no processo de recuperação de senha). | Baixa | `EmailService.cs` e integração no `UsuarioService`. |
| RF09 | O sistema deve conter **conexão com sites de análise** (OMDB/TMDB) para dados de mídia e ratings. | Média | Serviços `OmdbService.cs` e `TmdbService.cs` para busca e agregação de dados. |
| RF10 | O sistema deve ter um **sistema de busca eficiente** (com sugestões). | Alta | `SearchController` usando o padrão Decorator e lógica de sugestões em `header.js`. |
| RF11 | O sistema deve permitir que os usuários **adicionem e visualizem comentários** no conteúdo. | Média | `CommentsController` e lógica de front-end em `comment.js`. |
| RF12 | O sistema deve permitir **filtrar buscas por gênero** (via listagens da home). | Alta | Endpoint `byGenre` no `OmdbMoviesController` e carregamento em `apicatalago.js`. |
| RF13 | O sistema deve permitir que os usuários **classifiquem o conteúdo** (like/dislike). | Média | Lógica de Like/Dislike client-side e tentativa de envio de dados no `comment.js`. |
| RF14 | O sistema deve permitir a criação de **listas de "assistir mais tarde" (favoritos)**. | Baixa | Lógica de Favoritos em `comment.js` (usando `localStorage`) e estrutura de lista no banco de dados. |
| RF15 | O sistema deve permitir **filtrar buscas por tipo de mídia** ("movie" ou "series"). | Alta | `TypeFilterDecorator` usado no `SearchController` e links de filtro no `apicatalago.js`. |
| RF16 | O sistema deve exibir **trailers e informações detalhadas** sobre os títulos. | Média | Lógica de agregação de dados OMDB/TMDB e URL do trailer em `OmdbMoviesController` e visualização em `filmeSerie.html`. |
| RF17 | O sistema deve permitir **filtrar buscas por ano de lançamento mínimo**. | Média | `MinYearFilterDecorator` usado no `SearchController`. |
| RF18 | O sistema possui a estrutura de banco para **monitorar o status do conteúdo assistido** (completo, assistindo, etc.). | Alta | Campos `minutos_assistidos` e `conclusao` na tabela `access_history`. |
| RF19 | O sistema permite a **criação de perfis de usuário** para personalização de preferências. | Média | Estrutura de `Usuario` com campos como `Foto_Perfil` e tela de perfil dedicada. |
| RF20 | O sistema oferece **recomendações básicas** (baseadas em gênero). | Média | Endpoint `byGenre` no `OmdbMoviesController` para listagens por categoria na home, simulando um sistema de recomendação. |
