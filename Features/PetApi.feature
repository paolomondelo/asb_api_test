Feature: Pet API CRUD
  In order to manage pets in the store
  As an API tester
  I want to create, update, get and delete a pet

  @crud @pet @smoke
  Scenario: Happy path CRUD for a single pet
    Given a new pet with id 8073 and name "shihtzu"
    When I send a POST request to create the pet
    Then the response status code should be 200
    And the response should contain pet id 8073 and name "shihtzu"

    When I send a PUT request to update the pet name to "bulldog"
    Then the response status code should be 200
    And the response should contain pet id 8073 and name "bulldog"

    When I send a GET request to retrieve the pet by id 8073
    Then the response status code should be 200
    And the response should contain pet id 8073 and name "bulldog"

    When I send a DELETE request to delete the pet using the same pet id
    Then the response status code should be 200
    And the delete response should contain pet id 8073

    When I send a GET request to retrieve the pet by id 8073
    Then the response status code should be 404
    And the response should contain error message "Pet not found"

  @post @pet @smoke
  Scenario: Create pet using POST /pet and response code should be 200
    Given a new pet with id 8073 and name "shihtzu"
    When I send a POST request to create the pet
    Then the response status code should be 200
    And the response should contain pet id 8073 and name "shihtzu"

  @get @pet @smoke
  Scenario: Get pet using GET /pet/{petid} and response code should be 200
    Given a new pet with id 8073 and name "shihtzu"
    When I send a POST request to create the pet
    And I send a GET request to retrieve the pet by id 8073
    Then the response status code should be 200
    And the response should contain pet id 8073 and name "shihtzu"

  @delete @pet @smoke
  Scenario: Delete pet using DELETE /pet/{petid} and response code should be 200
    Given a new pet with id 8073 and name "shihtzu"
    When I send a POST request to create the pet
    And I send a DELETE request to delete the pet using the same pet id
    Then the response status code should be 200
    And the delete response should contain pet id 8073

  @delete @pet @smoke
  Scenario: Delete pet twice using DELETE /pet/{petid} and response code of second delete should be 404
    Given a new pet with id 8073 and name "shihtzu"
    When I send a POST request to create the pet
    And I send a DELETE request to delete the pet using the same pet id
    Then the response status code should be 200
    When I send another DELETE request to delete the pet using the same pet id
    Then the response status code should be 404
   
    
  @post @pet @smoke
  Scenario Outline: Create pet using POST /pet and validate the correct response codes
    Given a new pet with id <id> and name "<name>"
    When I send a POST request to create the pet
    Then the response status code should be <expectedStatusCode>
    And the response should contain pet id <id> and name "<name>"

    Examples:
      | id   | name     | expectedStatusCode |
      | 8073 | shihtzu  | 200                |
      | 9999 | poodle   | 200                |
      | 3434 | chowchow | 200                |
      | 6445 | beagle   | 200                |
      | 2324 | bulldog  | 200                |
      | 1111 | lion     | 200                |
      | 2222 | tiger    | 200                |
      | 3333 | bear     | 200                |
      | 4444 | wolf     | 200                |
      | 5555 | fox      | 200                |
 
 

    

