# Simulador de Crédito

API desenvolvida em .NET 8 para simulação de crédito, utilizando SQLite e SQL Server como bancos de dados, Dapper para acesso performático, OpenTelemetry para observabilidade e Health Checks para monitoramento de saúde dos serviços.

---

## Como executar via Docker

1. **Construa a imagem:**
   docker build -t simulador-credito .

2. **Execute o container:**
   docker run -p 8080:8080 simuladorcredito


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

Sinta-se à vontade para adaptar conforme as particularidades do seu projeto!
