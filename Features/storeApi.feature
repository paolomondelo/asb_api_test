Feature: Store API CRUD Tests
    To validate Store API functionality
    As an API tester
    I want to test CRUD operations and inventory management


    @store @smoke
    Scenario: GET /store/inventory returns 200 status code
        When I send a GET request to /store/inventory
        Then the response status code should be 200
    
    @store @smoke
    Scenario: POST /store/order returns 200 status code
        Given a new order with id 123 ,pet id 8073,quantity 1,status "placed",and complete true
        When I send a POST request to /store/order
        Then the response status code should be 200
        And the response should contain order id 123
        And the response should contain pet id 8073
        And the response should contain quantity 1
        And the response should contain status "placed"
        And the response should contain complete true
        
        
