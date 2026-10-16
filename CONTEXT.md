# TastyBites

A recipe website: an admin curates recipes, and visitors browse them on a public site.

## People

**Admin**:
A signed-in person who creates, edits and deletes recipes.
_Avoid_: User, owner, editor

**Visitor**:
Anyone browsing the public site without signing in; can only read recipes.
_Avoid_: Guest, customer, user

## Apps

**Admin app**:
The signed-in area where the Admin manages recipes (the course's `admin.angular`).
_Avoid_: Dashboard, backoffice, CMS

**Public site**:
The open website where Visitors explore and view recipes (the course's `fresh.angular`).
_Avoid_: Frontend, landing page, client

## Recipes

**Recipe**:
A dish with a name and a description that the Admin publishes for Visitors.
_Avoid_: Dish, post, item

**Ingredient**:
One line in a Recipe's ingredient list: a name, with an optional amount and unit (e.g. "200 g flour", "salt").
_Avoid_: Item, component, product

**Step**:
One instruction in a Recipe, shown to Visitors as part of a numbered list.
_Avoid_: Instruction, direction, method

**Recent recipes**:
The most recently added recipes, shown first.
_Avoid_: Latest, new recipes
