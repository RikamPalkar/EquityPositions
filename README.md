# Equity Positions

Equity Positions is a full-stack sample project that processes trade transactions and calculates net equity positions by security.

This repository is designed to be easy to fork, run locally, and extend.

## Why This Project Exists

The project demonstrates how to:

- process INSERT, UPDATE, and CANCEL trade events
- maintain trade state with versioning
- compute net positions in real time
- expose clean REST APIs
- present a simple React UI for operators and reviewers

## Tech Stack

- Backend: ASP.NET Core (.NET 8)
- Domain logic: C# class library
- Persistence: Entity Framework Core + SQLite
- Frontend: React + TypeScript + Vite

## Repository Structure

- EquityPositions.Api: API host, controllers, app wiring
- EquityPositions.Domain: entities, enums, business rules
- EquityPositions.Persistence: EF Core context, configurations, repositories
- client: React web app
- SRS.md: full Software Requirements Specification

## Quick Start

### 1. Clone/Fork

Fork this repo on GitHub, then clone your fork locally.

### 2. Start Backend API

```bash
cd EquityPositions/EquityPositions.Api
dotnet restore
dotnet run
```

API and Swagger:

- http://localhost:5277

### 3. Start Frontend

```bash
cd EquityPositions/client
npm install
npm run dev
```

Frontend URL:

- http://localhost:5173

## API Overview

Base URL:

- http://localhost:5277/api

Main endpoints:

- GET /transactions
- GET /transactions/{id}
- GET /transactions/trade/{tradeId}
- POST /transactions
- GET /positions
- GET /positions/{securityCode}

Example POST /transactions payload:

```json
{
  "tradeId": 101,
  "version": 1,
  "securityCode": "REL",
  "quantity": 150,
  "action": "INSERT",
  "side": "Buy"
}
```

## Business Behavior (High Level)

- INSERT creates a new trade and applies quantity to position.
- UPDATE requires a higher version, reverses old contribution, and applies new contribution.
- CANCEL reverses the trade contribution and marks the trade as cancelled.

Buy contributes positive quantity, Sell contributes negative quantity.

## Documentation Map

- Project overview and onboarding: README.md (this file)
- Full requirements and acceptance criteria: SRS.md
- Frontend-specific notes: client/README.md

## Common Local Issues

- Port conflict on 5277 or 5173: stop the process using that port or change launch/dev config.
- CORS errors: ensure frontend runs on http://localhost:5173 and backend on http://localhost:5277.
- API unreachable from UI: verify the API is running before loading the frontend.

## How to Contribute

1. Fork the repository.
2. Create a feature branch.
3. Keep changes scoped and include clear commit messages.
4. Validate API and UI locally.
5. Open a pull request with a short problem/solution summary.

## Roadmap Ideas

- Add authentication and authorization.
- Add pagination/filtering for transaction history.
- Add automated tests for domain rules and controllers.
- Add Docker compose for one-command startup.
