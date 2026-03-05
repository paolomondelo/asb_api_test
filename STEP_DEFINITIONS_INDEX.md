# Step definitions → code reference

Use this index to jump from a step to its implementation. **Ctrl+Click** (or Cmd+Click) the link to open the file at the correct line.

## PetApiSteps.cs

| Step text (pattern) | Go to definition |
|--------------------|------------------|
| Given a new pet with id … and name "…" | [PetApiSteps.cs:21](StepDefinitions/PetApiSteps.cs#L21) |
| When I send a POST request to create the pet | [PetApiSteps.cs:28](StepDefinitions/PetApiSteps.cs#L28) |
| When I send a PUT request to update the pet name to "…" | [PetApiSteps.cs:35](StepDefinitions/PetApiSteps.cs#L35) |
| When I send a GET request to retrieve the pet by id … | [PetApiSteps.cs:43](StepDefinitions/PetApiSteps.cs#L43) |
| When I send a DELETE request to delete the pet using the same pet id | [PetApiSteps.cs:50](StepDefinitions/PetApiSteps.cs#L50) |
| When I send another DELETE request to delete the pet using the same pet id | [PetApiSteps.cs:57](StepDefinitions/PetApiSteps.cs#L57) |
| When I send a DELETE request to delete the pet with id {int} | [PetApiSteps.cs:64](StepDefinitions/PetApiSteps.cs#L64) |
| **Then the response status code should be …** | [PetApiSteps.cs:71](StepDefinitions/PetApiSteps.cs#L71) |
| And the response should contain pet id … and name "…" | [PetApiSteps.cs:80](StepDefinitions/PetApiSteps.cs#L80) |
| And the delete response should contain pet id … | [PetApiSteps.cs:91](StepDefinitions/PetApiSteps.cs#L91) |
| Given the pet is "…" before deletion | [PetApiSteps.cs:99](StepDefinitions/PetApiSteps.cs#L99) |
| Then the response should contain error message "…" | [PetApiSteps.cs:117](StepDefinitions/PetApiSteps.cs#L117) |

## StoreApiSteps.cs

| Step text (pattern) | Go to definition |
|--------------------|------------------|
| When I send a GET request to /store/inventory | [StoreApiSteps.cs:19](StepDefinitions/StoreApiSteps.cs#L19) |
| Then the response should contain inventory data | [StoreApiSteps.cs:26](StepDefinitions/StoreApiSteps.cs#L26) |

---

**Tip:** You can also **Search in workspace** (Ctrl+Shift+F) for a step phrase (e.g. `the response status code should be`) to find the same line in the step definition file.
