# Distributed Pizza Hut

This Distributed Pizza Hut is my attempt to practice with some distributed principles. A frontend where one can order pizza's, some queuing and processing of orders and deliveries. I might add some chaos too.

## Context Diagram

```mermaid
graph TB
  Customer--"Orders pizza"-->CustomerApp("Frontend application")
  CustomerApp("Frontend application")--New Order-->MessageBus
  MessageBus--"New Order"-->PaymentService
  MessageBus--"New Order"-->BakingService
  MessageBus--"PizzaReadyForDelivery"-->DeliveryService
```

### Different states / events

* PizzasOrdered
* OrderPaid
* OrderPrepared
* OrderReturned
* OrderDelivered
* OrderDelayed
* OrderCompleted

## Order process

```mermaid
sequenceDiagram
  participant Customer
  participant Frontend Application
  participant Message Queue
  participant Payment Service
  participant Baking Service
  participant Delivery Service

  Customer->>Frontend Application: Orders pizzas
  Frontend Application->>Message Queue: Pizzas ordered
  Message Queue->>Payment Service: Pizzas ordered
  Payment Service->>Message Queue: Payment requested
  Message Queue->>Frontend Application: Payment requested
  Frontend Application->>Customer: Payment requested  
  Customer->>Frontend Application: Pays order
  Frontend Application->>Message Queue: Order paid
  Message Queue->>Payment Service: Order paid
  Payment Service->>Message Queue: Order ready for processing
  Message Queue->>Baking Service: Order ready for processing
  Baking Service->>Message Queue: Order ready for delivery
  Message Queue->>Delivery Service: Order ready for delivery
  Delivery Service->>Message Queue: Order on its way to delivery
  Message Queue->>Communication Service: Order on its way to delivery
  Communication Service->>Customer: Order on its way to delivery
  Delivery Service->>Message Queue: Order delivered
  Message Queue->>Payment Service: Order delivered
  Payment Service->>Message Queue: Order completed
  Message Queue->>Communication Service: Order completed
  Communication Service->>Customer: Order completed
  
  
```

## Example mermaid

```mermaid
sequenceDiagram
    participant dotcom
    participant iframe
    participant viewscreen
    dotcom->>iframe: loads html w/ iframe url
    iframe->>viewscreen: request template
    viewscreen->>iframe: html & javascript
    iframe->>dotcom: iframe ready
    dotcom->>iframe: set mermaid data on iframe
    iframe->>iframe: render mermaid
```
