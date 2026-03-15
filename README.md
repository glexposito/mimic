# Mimic

**Mimic** is a lightweight, declarative mock API server powered by **YAML**.

It lets you define HTTP endpoints in configuration and serve them dynamically without writing controllers or custom mock code.

Mimic is designed to be:

- **declarative**
- **language-agnostic**
- **container-friendly**
- **simple to run locally or in tests**

The goal is to make API mocking easy for development, testing, and integration scenarios.

Mimic’s long-term value is not only that it can mock APIs, but that mock definitions can live as portable YAML and run anywhere as a standalone dependency.

That means the main direction for Mimic is:

- define mocks in YAML
- keep them language-agnostic and version-controlled
- run them locally, in CI, or in Docker
- use them from any stack, including Testcontainers-based integration tests

---

## Why Mimic?

Many teams need to simulate external APIs during development and testing, but existing approaches often fall into one of these categories:

- handwritten mock services
- framework-specific test doubles
- tools that require a UI or imperative setup
- mocks tied to one programming language or runtime

Mimic takes a different approach.

You define mock APIs in **YAML**, run Mimic, and it serves those endpoints over HTTP.

The core idea is that the mock definition itself should be portable.

It should not matter whether the consumer is written in C#, Java, JavaScript, Python, Go, or something else. The same mock API definition should be reusable across all of them.

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

Another important goal is for Mimic to be easy to package and run in Docker, so that the same mock setup can be reused consistently across local development, CI pipelines, and container-based integration tests.

---

## Project Status

🚧 **Work in progress**

Mimic is still in active development and is not production-ready yet.

At this stage, the project is an early MVP foundation.

The current implementation can already:

- load mock API definitions from YAML
- match incoming HTTP requests
- return configured responses

It is still not feature-complete and is not ready for production use yet.

The main product direction is already clear, even though the implementation is still early:

- portable YAML mock definitions
- standalone HTTP mock server
- Docker-friendly runtime
- cross-language integration testing support
- strong Testcontainers fit

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
Represents a single mock API definition loaded from YAML.

### `MockEndpoint`
Defines a mock endpoint, including HTTP method and path.

### `MockResponse`
Defines the response returned by Mimic.

### `MockEngine`
Responsible for selecting the correct endpoint and returning the configured response.

### `PathMatcher`
Matches incoming request paths against configured templates, such as `/users/{id}`.

---

## Running Mimic

Mimic can currently load either a single YAML file or a folder containing multiple YAML files.

Run with the default `mocks.yaml` in the server project:

```bash
dotnet run --project Mimic.Server -- --urls http://localhost:5000
```

Run with an explicit file:

```bash
dotnet run --project Mimic.Server -- --urls http://localhost:5000 --config client.yaml
```

Run with a folder of YAML files:

```bash
dotnet run --project Mimic.Server -- --urls http://localhost:5000 --config ./clients
```

Then call a configured endpoint:

```bash
curl http://localhost:5213/hello
```

Example response:

```json
{
  "message": "hello from mimic"
}
```

---

## Testing

The server currently has end-to-end tests that verify:

- a configured endpoint returns the expected response
- an unmatched request returns `404`

Run the test suite with:

```bash
dotnet test
```

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
