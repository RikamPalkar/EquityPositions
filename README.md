# EquityPositions

> A full-stack equity trade processing system built with .NET 8 & React —
> demonstrating real-world financial domain logic, clean architecture,
> & production-grade REST API design.

[![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/Language-C%23-green)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Frontend](https://img.shields.io/badge/Frontend-React+TypeScript-61DAFB)](https://reactjs.org/)
[![Database](https://img.shields.io/badge/Database-SQLite-lightgrey)](https://www.sqlite.org/)
[![License](https://img.shields.io/badge/License-MIT-yellow)](LICENSE)

---

## What This Is

Most .NET tutorials show you CRUD. This shows you something real.

EquityPositions is a trade transaction processing system — the kind of
domain logic that sits at the heart of every trading desk, brokerage
platform, & financial application. It ingests trade events (INSERT,
UPDATE, CANCEL), maintains versioned trade state, & computes net
equity positions per security in real time.

Bridging the gap between system design theory & real-world .NET
implementation — giving developers a hands-on path from concept to
working code.

Built with ASP.NET Core, Entity Framework Core, SQLite, & a React
TypeScript frontend. Clean architecture. Real business rules. Full
SRS documentation included.

---

## What It Does

| Action | What Happens |
|---|---|
| **INSERT** | Creates trade, applies quantity delta to position |
| **UPDATE** | Validates version, reverses old contribution, applies new |
| **CANCEL** | Reverses trade contribution, marks trade as cancelled |

**Buy** contributes **+quantity** to position.
**Sell** contributes **−quantity** to position.

Net position per security = sum of all active trade deltas.

### Real Example
INSERT  trade 101, version 1, REL, Buy,  150  →  REL position: +150
UPDATE  trade 101, version 2, REL, Buy,  120  →  REL position: +120  (old reversed, new applied)
INSERT  trade 202, version 1, ITC, Sell,  40  →  ITC position: -40
CANCEL  trade 202, version 2, ITC, Sell,  40  →  ITC position:   0  (contribution reversed)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core (.NET 8), Minimal APIs |
| Domain Logic | C# class library, `PositionCalculator` |
| Persistence | Entity Framework Core + SQLite |
| Frontend | React 18, TypeScript, Vite |
| API Docs | Swagger / OpenAPI |

---

## Architecture
EquityPositions/
│
├── EquityPositions.Api/         # API host, controllers, middleware, wiring
│   └── Controllers/
│       ├── TransactionsController.cs
│       └── PositionsController.cs
│
├── EquityPositions.Domain/      # Entities, enums, business rules
│   ├── Entities/
│   │   ├── Transaction.cs       # Trade event (INSERT/UPDATE/CANCEL)
│   │   ├── TradeState.cs        # Latest known state per TradeId
│   │   └── Position.cs          # Net quantity per SecurityCode
│   └── Services/
│       └── PositionCalculator.cs # Core business logic — all position math lives here
│
├── EquityPositions.Persistence/ # EF Core context, configs, repositories
│   ├── AppDbContext.cs
│   └── Configurations/
│
├── client/                      # React + TypeScript frontend
│   └── src/
│       ├── components/          # TransactionForm, PositionsTable, TransactionHistory
│       └── App.tsx
│
└── SRS.md                       # Full Software Requirements Specification

---

## Business Rules

### Version Control
- **INSERT** rejects if TradeId already exists
- **UPDATE** requires version strictly greater than current trade version
- **CANCEL** rejects if trade is already cancelled

### Position Calculation
Buy  side → position += quantity
Sell side → position -= quantity
UPDATE reverses old (security + side + quantity), applies new values
CANCEL reverses current (security + side + quantity), zeroes contribution

### Response Contract
Every API response follows a consistent envelope:
```json
{
  "success": true,
  "message": "Transaction created and processed successfully",
  "data": { ... },
  "errors": []
}
```

---

## API Reference

**Base URL:** `http://localhost:5277/api`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/transactions` | Submit a trade transaction |
| GET | `/transactions` | All transactions |
| GET | `/transactions/{id}` | Transaction by ID |
| GET | `/transactions/trade/{tradeId}` | All transactions for a trade |
| GET | `/positions` | All current positions |
| GET | `/positions/{securityCode}` | Position for specific security |

### Example: Submit Transaction

```http
POST /api/transactions
Content-Type: application/json

{
  "tradeId": 101,
  "version": 1,
  "securityCode": "REL",
  "quantity": 150,
  "action": "INSERT",
  "side": "Buy"
}
```

**201 Response:**
```json
{
  "success": true,
  "message": "Transaction created and processed successfully",
  "data": {
    "transactionId": 1,
    "tradeId": 101,
    "version": 1,
    "securityCode": "REL",
    "quantity": 150,
    "action": "INSERT",
    "side": "Buy",
    "createdAt": "2026-04-14T10:00:00Z",
    "isProcessed": true
  },
  "errors": []
}
```

**400 — Business Rule Violation:**
```json
{
  "success": false,
  "message": "Trade 101 already exists. Cannot insert duplicate trade.",
  "data": null,
  "errors": []
}
```

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)

### 1. Clone the Repository

```bash
git clone https://github.com/RikamPalkar/EquityPositions.git
cd EquityPositions
```

### 2. Start the Backend

```bash
cd EquityPositions.Api
dotnet restore
dotnet run
```

API runs at `http://localhost:5277`
Swagger UI at `http://localhost:5277/swagger`

Database is created automatically via EF Core `EnsureCreated()` on startup.

### 3. Start the Frontend

```bash
cd ../client
npm install
npm run dev
```

Frontend runs at `http://localhost:5173`

---

## Data Model

### Transactions
| Field | Type | Notes |
|---|---|---|
| TransactionId | PK | Auto-generated |
| TradeId | int | Business identifier |
| Version | int | Must be monotonically increasing per trade |
| SecurityCode | string | Max 20 chars |
| Quantity | int | > 0 |
| Action | enum | INSERT / UPDATE / CANCEL |
| Side | enum | Buy / Sell |
| IsProcessed | bool | Set true after successful processing |

### TradeStates
Tracks the latest known state per TradeId — version, security, side, quantity, & cancellation flag.

### Positions
Net quantity per SecurityCode — updated atomically with every processed transaction.

---

## Error Handling

| Scenario | HTTP Status |
|---|---|
| Validation failure (field rules) | 400 with validation errors |
| Business rule violation | 400 with descriptive message |
| Resource not found | 404 |
| Unhandled exception | 500 with generic message |

---

## Common Local Issues

**Port conflict on 5277 or 5173** — stop the process using that port or
change the launch / dev config.

**CORS errors** — ensure frontend runs on `http://localhost:5173` &
backend on `http://localhost:5277`.

**API unreachable from UI** — verify the backend is running before
loading the frontend.

---

## Documentation

- `README.md` — this file, project overview & onboarding
- `SRS.md` — full Software Requirements Specification with acceptance criteria
- `client/README.md` — frontend-specific notes

---

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-idea`)
3. Commit & push
4. Open a Pull Request

Ideas: add authentication, multi-user support, batch file ingestion,
message queue integration, real-time WebSocket position updates,
unit tests for `PositionCalculator`.

---

## License

MIT License — use this however you want.

---

## Connect

**Rikam Palkar** — Senior Software Engineer, Microsoft MVP

- 🌐 [rikampalkar.github.io](https://rikampalkar.github.io)
- 💼 [LinkedIn](https://www.linkedin.com/in/rikampalkar/)
- ✍️ [Medium](https://medium.com/@RikamPalkar)
- 🐙 [GitHub](https://github.com/RikamPalkar)

---

*Stop building fake CRUD. Build systems that model real domains.*
