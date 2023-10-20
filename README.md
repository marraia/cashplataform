
# Cash Plataform 

API de fluxo de caixa diário com os lançamentos (débitos e créditos), e relatório com o saldo diário consolidado.


## Arquitetura

* Desenho de arquitetura macro
![Relatorio Consolidado](https://marraiarolha.blob.core.windows.net/images/currentaccount6.png)

* Design de Arquitetura - Arquitetura Hexagonal
![Design de Arquitetura](https://marraiarolha.blob.core.windows.net/images/currentaccount7.png)
## Documentação da API

#### Cria uma conta para ser movimentada com dados de movimentação

```http
  POST /api/CurrentAccount
```

| Authorization   | Tipo       | Descrição                           |
| :---------- | :--------- | :---------------------------------- |
| `Basic` | `string` | **Obrigatório**. Usuario e senha de licença |

#### Corpo da requisição
```bash
{
    "accountName": "Conta 0001"
}
```

#### Retorno da requisição
```bash
{
    "success": true,
    "data": {
        "accountName": "Conta 0001",
        "balance": 0,
        "id": "d91260b9-abf1-4d97-bfd3-ca14d107c3fe"
    }
}
```


#### Cria uma transação de crédito ou débito para a conta

```http
  POST /api/Transaction/{accountId}
```
| Authorization   | Tipo       | Descrição                           |
| :---------- | :--------- | :---------------------------------- |
| `Basic` | `string` | **Obrigatório**. Usuario e senha de licença |

| Parâmetro Rota  | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `accountId`      | `string` | **Obrigatório**. O ID da conta que irá ser movimentada |

#### Corpo da requisição
```bash
{
  "description": "Salário do mês",
  "value": 10000,
  "operation": *(0 Crédito | 1 Débito)*
}
```

#### Retorno da requisição
```bash
{
    "success": true,
    "data": {
        "currentAccountId": "d91260b9-abf1-4d97-bfd3-ca14d107c3fe",
        "description": "Salário do mês",
        "value": 10000,
        "balance": 30000,
        "operationType": 0,
        "date": "2023-10-20T17:52:11.3266982+00:00",
        "id": "9d570d7d-3d8b-4c48-a1ab-22b46ce72f24"
    }
}
```
#### Obtem relatório consolidado de movimentação da conta
```http
  POST /api/Transaction/{accountId}/Report
```
| Authorization   | Tipo       | Descrição                           |
| :---------- | :--------- | :---------------------------------- |
| `Basic` | `string` | **Obrigatório**. Usuario e senha de licença |

| Parâmetro  | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `accountId` `[rota]`      | `string` | **Obrigatório**. O ID da conta que irá ser 
| `from` `[querystring]`      | `string` | **Obrigatório**. Data de início como filtro para pesquisa 
| `to` `[querystring]`      | `string` | **Obrigatório**. Data fim como filtro para pesquisa |

#### Retorno da requisição
Download de arquivo de extensão **.xls**

## Testes unitários
O resultado dos testes unitários estão em: **testunits\TestResults\177db37c-cdba-4494-be32-072ab446036b\coveragereport\index.html**  

![Relatório Testes Unitários](https://marraiarolha.blob.core.windows.net/images/currentaccount8.png)


## Execução local


Clone o projeto

```bash
  git clone https://github.com/marraia/cashplataform
```

Entre no diretório do projeto

```bash
  cd cashplataform
```

Execute em seu terminal o comando

```bash
  docker-compose up -d
```

Caso queira que os serviços sejam parados, execute em seu terminal na pasta raiz do projeto o comando
```bash
  docker-compose down -d
```

#### Execução via Postman
**Observação**
* *Como na descrição do desafio não foi explicado qual tipo de serviço (FRONT/APP) deveria ser desenvolvido para que o usuário pudesse se interagir, coloquei a interação via **Postman**, para compreender o resultado;*

Importar arquivo [Cash Plataform.postman_collection.json] que está na pasta **postman** na raiz do repositório

* Primeiro passo é criar uma conta para ser movimentada
![Primeiro passo](https://marraiarolha.blob.core.windows.net/images/currentaccount.png)

* Depois iniciar a movimentar a conta, tanto para crédito **(Operation: 0)** como para débito **(Operation: 1)**
![](https://marraiarolha.blob.core.windows.net/images/currentaccount2.png)

* E depois realizar o download do relatório em excel, clique no **postman** em **Send or Download**
![Relatorio em Excel](https://marraiarolha.blob.core.windows.net/images/currentaccount3.png)

* Relatório com a aba **Consolidado** em aberto  

![Relatorio Consolidado](https://marraiarolha.blob.core.windows.net/images/currentaccount4.png)

* Relatório com a aba **Extrato** em aberto  

![Relatorio Extrato](https://marraiarolha.blob.core.windows.net/images/currentaccount5.png)


## Stack utilizada

**Back-end:** 
* .NET Core 6.0

**Banco de dados**
* MongoDb


**Bibliotecas autorais** 
* Marraia.Notifications [https://www.nuget.org/packages/Marraia.Notifications]
Não utilizei para validações ou regras a geração de exceções, mas sim utilizei a forma de notificação, e essa biblioteca é de minha autoria;

* Marraia.MongoDb [https://www.nuget.org/packages/Marraia.MongoDb]
Para facilitar a conexão com o banco de dados MongoDb, utilizei essa biblioteca que é de minha autoria;

**Bibliotecas adicionais** 

* EPPlus [https://www.nuget.org/packages/EPPlus.Core]
Para criação do relatório em excel;
## Autores

- [@marraia](https://github.com/marraia)

