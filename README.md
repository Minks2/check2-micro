# Sistema de Gestão de Estoque (Perecíveis e Não-Perecíveis)

Este projeto é uma API .NET 8 para gerenciamento de estoque, implementada como parte de uma avaliação, utilizando C#, Entity Framework Core e MySQL (via Docker).

## 🚀 Como Executar o Projeto

1.  **Pré-requisitos:**
    * .NET 8 SDK
    * Docker Desktop
    * Visual Studio 2022 (ou VS Code)
    * MySQL Workbench (Opcional, para visualizar o banco)

2.  **Iniciar o Banco de Dados (MySQL):**
    Na pasta raiz do projeto (onde está o `docker-compose.yml`), execute:
    ```bash
    docker-compose up -d
    ```

3.  **Executar a Aplicação:**
    * Abra a solução (`.sln`) no Visual Studio 2022.
    * O Visual Studio aplicará as migrations automaticamente ao rodar (se não aplicar, rode `Update-Database` no Console do Gerenciador de Pacotes).
    * Pressione F5 ou o botão "Play" (IIS Express ou Kestrel) para iniciar a API.

4.  **Acessar a Documentação da API:**
    Após iniciar, acesse o Swagger para testar os endpoints:
    `http://localhost:[SUA_PORTA]/swagger`

## 📋 Regras de Negócio Implementadas

* **Produtos:** Podem ser `PERECIVEL` ou `NAO_PERECIVEL`.
* **Movimentação:**
    * Não é permitida movimentação (entrada ou saída) de quantidade negativa ou zero.
* **Produtos Perecíveis:**
    * Uma `ENTRADA` de produto perecível *deve* conter um `Lote` e uma `DataValidade`.
    * Não é permitido dar `ENTRADA` em produtos perecíveis já vencidos.
* **Controle de Saldo:**
    * O saldo (`QuantidadeAtual`) do produto é atualizado automaticamente em toda `ENTRADA` ou `SAIDA`.
    * Não é permitida uma `SAIDA` se a quantidade solicitada for maior que a `QuantidadeAtual` em estoque.
* **Alertas e Relatórios:**
    * O sistema identifica produtos com `QuantidadeAtual` abaixo da `QuantidadeMinimaEmEstoque`.
    * O sistema lista produtos perecíveis que possuem lotes com vencimento nos próximos 7 dias.
    * O sistema calcula o valor total do estoque (Soma de `QuantidadeAtual` * `PrecoUnitario`).

## 🏗️ Diagrama de Entidades (Texto)

[Produto]

CodigoSKU (PK)

Nome

Categoria (Enum: PERECIVEL, NAO_PERECIVEL)

PrecoUnitario

QuantidadeMinimaEmEstoque

QuantidadeAtual (Saldo)

DataCriacao | | 1..N | [MovimentacaoEstoque]

Id (PK)

Tipo (Enum: ENTRADA, SAIDA)

Quantidade

DataMovimentacao

Lote (Nulável, obrigatório para ENTRADA perecível)

DataValidade (Nulável, obrigatório para ENTRADA perecível)

ProdutoSKU (FK -> Produto)


## ⚙️ Exemplos de Requisições API

(Use o Swagger UI em `http://localhost:[SUA_PORTA]/swagger` para testes interativos)

### 1. Criar um Produto Perecível
`POST /api/Produtos`
```json
{
  "codigoSKU": "SKU-IOG-01",
  "nome": "Iogurte Natural",
  "categoria": "PERECIVEL",
  "precoUnitario": 3.50,
  "quantidadeMinimaEmEstoque": 20
}
2. Registrar Entrada de Lote (Perecível)
POST /api/Movimentacoes/entrada

JSON

{
  "produtoSKU": "SKU-IOG-01",
  "quantidade": 100,
  "lote": "LOTE-B",
  "dataValidade": "2025-11-30T00:00:00"
}
3. Registrar Saída (Venda)
POST /api/Movimentacoes/saida

JSON

{
  "produtoSKU": "SKU-IOG-01",
  "quantidade": 10
}
4. Relatório de Estoque Mínimo
GET /api/Relatorios/estoque-minimo

5. Relatório de Próximos Vencimentos
GET /api/Relatorios/proximos-vencimento

6. Relatório de Valor Total do Estoque
GET /api/Relatorios/valor-total