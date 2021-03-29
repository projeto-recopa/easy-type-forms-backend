# Easy Typing Forms

<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#sobre-o-projeto">Sobre o projeto</a>
    </li>
    <li>
        <a href="#pre-requisitos">Pré-Requisitos</a>
    </li>
    <li>
        <a href="#configuracao">Configuração</a>
    </li>
    <li>
        <a href="#configuracao-da-api-google-cloud">Configuração da API Google Cloud</a>
    </li>
    <li>
        <a href="#build">Build</a>
    </li>
    <li>
        <a href="#dockerhub">DockerHub</a>
    </li>
  </ol>
</details>


<!-- ABOUT THE PROJECT -->
## Sobre o Projeto

A solução back-end é composta por dois projetos.
* **image-cloud-processor** - Aplicação responsável por prover serviços para o fron-end. Recebe o *upload* do arquivo e inicializa o processamento da imagem.
* **document-ml-predict** - Aplicação responsável por prover os serviços que aplicam os modelos de *Machine Learning* na predição dos campos do tipo de seleção.

Ambas aplicações possui a API documentada via swagger. As aplicações também possuem suporte ao docker e podem ser publicadas em containers linux.

## Pré-Requisitos

* Google.Cloud.Vision.V1
* MongoDB
* Microsoft ML

## Configuração
### Connection String
Para acesso ao banco de dados deve ser informada a connection string para o mongoDB: 
*CONNECTIONSTRINGS__MONGODB*

### Variáveis de Ambiente

Além da connection string do mongoDB, as seguintes variáveis de ambiente também devem ser informadas:
```
PredictEndPoint="Url do endpoint /Predict da aplicação doucument-ml-predict"
```

## Configuração da API Google Cloud
Para configuração da API do Google Cloud Vision, é necessário primeiro configurar a conta de serviço e baixa a chave da api (Conforme descrito em https://cloud.google.com/vision/docs/setup#sa-create).

Após realizar o download o arquivo JSON com a chave de serviço, salvamos o seu conteúdo numa variável de ambiente "GOOGLE_APPLICATION_CREDENTIALS_CONTENT".

## Build
As aplicações possuem suporte ao docker e podem ter suas imagens geradas pelo comando docker build executado na pasta raiz de cada aplicação:

```
docker build .
docker tag local-image:tagname [new-repo]:tagname
docker push [new-repo]:tagname
```

Substituindo [new-repo] pelo nome completo do repositório onde a imagem será registrada.

Para a aplicação image-cloud-processor, deve ser incluído o parâmetro --build-arg GOOGLE_APPLICATION_CREDENTIALS_CONTENT ao fim do comando de build do docker, para que a chave de autentiação da API Google seja configurada na imagem do container.

Desta forma o comando fica:

```
docker build . --build-arg GOOGLE_APPLICATION_CREDENTIALS_CONTENT
docker tag local-image:tagname [new-repo]:tagname
docker push [new-repo]:tagname
```


## Docker Hub

Resgistradas na organizacao recopampes, temos um repositorio com as imagens das aplicações, prontas para serem publicadas.
Podem ser acessadas por meio dos comandos:

* document-ml-predict (https://hub.docker.com/r/recopampes/document-ml-predict)

``` 
docker pull recopampes/document-ml-predict:latest
```
* image-cloud-processor (https://hub.docker.com/r/recopampes/image-cloud-processor)

```
  docker pull recopampes/image-cloud-processor:latest
```