# TastyBites

## Versions

These versions work together. Upgrading one usually means upgrading the others.

| Tool | Version | Why this one |
|------|---------|--------------|
| ABP (backend + `@abp/ng.*`) | 7.4.5 | The version the project was created with |
| Angular | 16.2.12 | Matches ABP 7.4.x |
| Node | 18.20.8 | Angular 16 supports Node 16 and 18 (pinned in `.nvmrc`) |
| .NET SDK | 7.0 | ABP 7.4 targets `net7.0` |
| ABP CLI | 7.4.5 | Same as the ABP packages, so generated code matches |
| Database | MariaDB (XAMPP) on port 3306 | Works with the MySQL provider |

Run `./scripts/dev.sh setup` from the repo root to install these.


This is a startup project based on the ABP framework. For more information, visit <a href="https://abp.io/" target="_blank">abp.io</a>

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.
