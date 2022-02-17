Feature: Creating orders
    As a customer
    When ordering one or more pizzas 
    I want an order to be created
    So I can pay for my order

    Scenario: Ordering a pizza
        Given Julia orders a Pizza Pepperoni with a price of 9.95
        And she orders a Pizza Hawai with a price of 8.95
        When she finalizes her order
        Then an order will be created 