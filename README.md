# Mimic

**Mimic** is a lightweight, declarative mock API server powered by **YAML**.

It lets you define HTTP endpoints in configuration and serve them dynamically without writing controllers or custom mock code.

Mimic is designed to be:

- **declarative**
- **language-agnostic**
- **container-friendly**
- **simple to run locally or in tests**

The goal is to make API mocking easy for development, testing, and integration scenarios.

---

## Why Mimic?

Many teams need to simulate external APIs during development and testing, but existing approaches often fall into one of these categories:

- handwritten mock services
- framework-specific test doubles
- tools that require a UI or imperative setup
- mocks tied to one programming language or runtime

Mimic takes a different approach.

You define mock APIs in **YAML**, run Mimic, and it serves those endpoints over HTTP.

That makes it especially useful for:

- local development
- frontend development before the real backend exists
- integration testing
- contract testing
- containerized test environments

A particularly strong use case for Mimic is **integration testing with Testcontainers**.

In the future, Mimic should work very well as a disposable mock dependency started inside tests, similar in spirit to WireMock-based setups, but with a more **declarative** and **language-agnostic** configuration model.

That means teams can:

- store mock APIs in version control
- review changes in pull requests
- reuse the same mock definitions across different languages and test suites
- run the same mock server locally, in CI, or in Docker

This is intended to be one of Mimic’s core strengths.

---

## Project Status

🚧 **Work in progress**

Mimic is still in active development and is not ready for real use yet.

At this stage, the project is only a skeleton for the planned MVP.

The current goal is to build the foundations for a version that can eventually:

- load mock API definitions from YAML
- match incoming HTTP requests
- return configured responses

It is not feature-complete and is not ready for production or testing use yet.

Not included in the MVP yet:

- response delays
- conditional rules
- failure simulation
- invocation counters
- request logging
- admin endpoints
- OpenAPI import

These can be added later once the base engine is stable.

---

## MVP Configuration Example

Example `mocks.yaml`:

```yaml
name: users-api

endpoints:
  - method: GET
    path: /hello
    respond:
      status: 200
      body:
        message: hello from mimic
```

---

## Planned Core Concepts

### `MockApiConfig`
Represents the root configuration loaded from YAML.

### `MockEndpoint`
Defines a mock endpoint, including HTTP method and path.

### `MockResponse`
Defines the response returned by Mimic.

### `MockEngine`
Responsible for selecting the correct endpoint and returning the configured response.

### `PathMatcher`
Matches incoming request paths against configured templates, such as `/users/{id}`.

---

## Intended Future Usage

Once the basic server flow is implemented, Mimic is expected to be used like this:

```bash
dotnet run --project src/Mimic.Server
```

Then call a configured endpoint:

```bash
curl http://localhost:5000/hello
```

Example response:

```json
{
  "message": "hello from mimic"
}
```

This is not working yet and is included only to illustrate the intended direction of the project.

---

## Design Principles

Mimic is being designed around a few simple principles:

### 1. Declarative first
Mock behavior should be defined in configuration rather than handwritten mock code.

### 2. Language agnostic
Mock definitions should be reusable regardless of whether the consumer is written in C#, Java, JavaScript, Python, Go, or anything else.

### 3. Container friendly
Mimic should run cleanly in Docker and later integrate naturally with Testcontainers.

### 4. Core separated from hosting
The mock engine should be reusable independently from the HTTP server and config loading.

### 5. Simple MVP, extensible future
The first version should solve the basic problem well and leave room for more advanced behaviors later.

---

## Future Direction

Planned future features may include:

- response delays
- conditional rules
- fail-after / retry simulation
- invocation counters
- request templating
- request logs
- admin endpoints
- OpenAPI import
- Docker-first workflows
- stronger integration-test support

---

## Future Integration Testing Story

One of the most valuable future goals for Mimic is first-class support for **integration testing**.

The idea is that Mimic can be started as a test dependency, for example through **Testcontainers**, and serve mock APIs defined by YAML.

This would make Mimic useful as a:

- local mock server
- CI test dependency
- Dockerized integration-test dependency
- shared declarative mock layer across teams

That makes Mimic particularly attractive compared to handwritten mocks or mocks bound to one framework or language.

A strong long-term positioning could be:

> **Mimic is a declarative, language-agnostic mock API server for development and integration testing.**

---

## Roadmap

### MVP
- YAML config loading
- basic endpoint matching
- static responses
- path parameter support
- Docker support

### Next
- delay support
- richer routing rules
- request logs
- admin/reset endpoints
- integration-test friendly workflows

### Later
- OpenAPI import
- scenario/state support
- failure simulation
- hosted workflows or UI tooling

---

## License

MIT License.
