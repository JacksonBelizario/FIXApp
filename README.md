# Aplica��o FIX

- Pequena aplica��o desenvolvida para estudos do protocolo FIX

## Descri��o

- Acceptor que recebe como argumento a configura��o
- Mensagens que o aceptor trata: NewOrderSingle, OrderCancelRequest, OrderCancelReplaceRequest
- O Acceptor recebe as mensagens acima, e responde a todos os clientes da sess�o com outra mensagem

## Configura��o do executor

- Porta: 5001
- SenderCompID: EXECUTOR
- Clientes:
  -TargetCompID: CLIENT1, CLIENT2, CLIENT3
- Protocolo: 4.4

## Inciando o docker

- docker-compose build
- docker-compose up -d