# Aplicação FIX

- Pequena aplicação desenvolvida para estudos do protocolo FIX

## Descrição

- Acceptor que recebe como argumento a configuração
- Mensagens que o aceptor trata: NewOrderSingle, OrderCancelRequest, OrderCancelReplaceRequest
- O Acceptor recebe as mensagens acima, e responde a todos os clientes da sessão com outra mensagem

## Configuração do aceptor

- Porta: 5001
- SenderCompID: EXECUTOR
- Clientes:
  * TargetCompID: CLIENT1, CLIENT2, CLIENT3
- Protocolo: 4.4

## Iniciando o docker

- docker-compose build
- docker-compose up -d
