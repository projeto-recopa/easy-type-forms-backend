# Easy Typing Forms
## Back-End


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
