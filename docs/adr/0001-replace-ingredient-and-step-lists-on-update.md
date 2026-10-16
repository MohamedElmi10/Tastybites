# Updating a recipe replaces its ingredient and step lists

When a Recipe is updated, the API deletes its existing Ingredients and Steps and saves the lists sent in the request, in the order given (list position becomes `SortOrder`). We chose this over matching each item by id (insert / update / delete per row) because a recipe has at most 50 of each, so the extra writes cost nothing, and the code stays small and predictable.

## Consequences

- The client must always send the **full** recipe, including both lists. A request without the lists empties them. The Admin app's form always loads the whole recipe before saving, so this holds today.
- Ingredient and step DTOs carry no `Id` or `SortOrder`; ids change on every save, so nothing outside the recipe should reference a single ingredient or step by id.
- If that ever becomes necessary (e.g. linking a shopping list to a specific ingredient), switch to per-item matching.
