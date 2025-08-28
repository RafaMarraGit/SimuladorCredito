# Simulador de Crédito

API desenvolvida em .NET 8 para simulação de crédito, utilizando SQLite e SQL Server como bancos de dados, Dapper para acesso performático, OpenTelemetry para observabilidade e Health Checks para monitoramento de saúde dos serviços.

---

## Como executar via Docker

1. **Construa a imagem:**
   docker build -t simulador-credito .

2. **Execute o container:**
   docker run -p 8080:8080 simulador-credito


4. **Acesse a API:**
   - Swagger: [http://localhost:8080/swagger](http://localhost:8080/swagger)
   - Health Check: [http://localhost:8080/health](http://localhost:8080/health)

> Configuração de conexão ao banco de dados SQL Server hackathon em appsettings.js apenas por se tratar de projeto desafio, não seguindo assim regras de segurança, localmente estava usando user-secrets em DES ou PRD user keyValts.

---

## Uso da biblioteca Dapper

O Dapper é utilizado como micro-ORM para acesso aos dados. Ele se destaca por ser extremamente leve e performático, pois executa consultas SQL diretamente, sem o overhead de mapeamento complexo de entidades como ocorre em ORMs tradicionais.

**Por que Dapper é mais performático?**
- Executa comandos SQL diretamente, reduzindo o tempo de processamento.
- Menor consumo de memória.
- Respostas mais rápidas, aumentando a responsividade da aplicação.
- Ideal para cenários onde o tempo de resposta é crítico e o volume de dados é elevado.

---

## Observabilidade com OpenTelemetry

A aplicação utiliza o OpenTelemetry para coleta de métricas e rastreamento de requisições. Isso permite:
- Monitorar o desempenho da API em tempo real.
- Identificar gargalos e pontos de falha rapidamente.
- Integrar facilmente com sistemas de monitoramento como Prometheus, Jaeger, entre outros.
- End-point: [http://localhost:8080/Telemetria](http://localhost:8080/Telemetria)
---

## Health Check

O projeto implementa health checks para monitorar a saúde dos bancos de dados utilizados.  
Endpoints de health check permitem que sistemas de orquestração (como Kubernetes ou Docker Compose) verifiquem se a aplicação está saudável e pronta para receber requisições.

- O health check verifica a conexão com o banco de dados principal e o banco de dados de hackathon.
- Caso algum serviço esteja indisponível, o endpoint `/health` retorna status `Unhealthy`, facilitando automações de restart ou alerta.

---


3. **Acesse a API:**
   - Swagger: [http://localhost:8080/swagger](http://localhost:8080/swagger)
   - Health Check: [http://localhost:8080/health](http://localhost:8080/health)

> **Nota:** A configuração de conexão ao banco de dados SQL Server está em `appsettings.json` apenas para fins de demonstração. Em ambientes de produção, recomenda-se o uso de ferramentas como [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) ou [Azure Key Vault](https://learn.microsoft.com/azure/key-vault/) para gerenciar credenciais de forma segura.

---

## Melhorias a serem implementadas

### 1. **Testes Unitários**
- **Situação Atual:** O projeto possui testes unitários básicos para o controlador principal (`SimuladorCreditoController`).
- **Melhorias Necessárias:**
  - Cobertura de testes para todos os serviços, repositórios e controladores.
  - Mock de dependências utilizando bibliotecas como `Moq`.
  - Configuração de relatórios de cobertura de testes (ex.: `coverlet` ou `ReportGenerator`).
  - Integração com pipelines de CI/CD para execução automática dos testes.

### 2. **Banco de Dados em Container**
- **Situação Atual:** O projeto utiliza SQLite e SQL Server localmente.
- **Melhorias Necessárias:**
  - Configurar o banco de dados em um container Docker para facilitar o desenvolvimento e testes.
  - Criar um `docker-compose.yml` para orquestrar a API e o banco de dados.
  - Exemplo de configuração para SQL Server:

### 3. **Integração Contínua (CI/CD)**
- Configurar pipelines de CI/CD para:
  - Build e execução de testes automatizados.
  - Análise de qualidade de código (ex.: `SonarQube` ou `CodeQL`).
  - Deploy automatizado para ambientes de desenvolvimento e produção.

### 4. **Segurança**
- Remover credenciais sensíveis do `appsettings.json` e utilizar:
  - [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) para desenvolvimento local.
  - [Azure Key Vault](https://learn.microsoft.com/azure/key-vault/) ou equivalente para ambientes de produção.
- Implementar autenticação e autorização na API (ex.: JWT ou OAuth2).

### 5. **Documentação**
- Expandir a documentação do Swagger para incluir exemplos de requisições e respostas.
- Adicionar um guia de contribuição para desenvolvedores que desejam colaborar no projeto.

### 6. **Melhorias de Observabilidade**
- Adicionar logs estruturados com `Serilog` ou `NLog`.
- Configurar dashboards para monitoramento de métricas no Prometheus ou Grafana.


---

