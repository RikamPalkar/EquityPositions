# Equity Positions System Requirements Specification (SRS)

## 1. Document Control

- Project: Equity Positions
- Solution: EquityPositions.sln
- Version: 1.0
- Date: 2026-04-14
- Authoring basis: implemented behavior in API, Domain, Persistence, and React client

## 2. Purpose

This document defines the software requirements for the Equity Positions application.

The system ingests trade transactions (INSERT, UPDATE, CANCEL), maintains the current state of each trade, and calculates net positions per security.

## 3. Scope

The application includes:

- Backend API built with ASP.NET Core and Entity Framework Core (SQLite)
- Domain logic for transaction processing and position calculation
- Persistence layer for transactions, trade state, and positions
- Frontend web client (React + TypeScript + Vite) for data entry and monitoring

Out of scope:

- Authentication/authorization
- Multi-user access control
- Batch file ingestion
- Message queues/stream processing

## 4. Definitions

- Trade: Business identifier represented by TradeId.
- Transaction: An event that affects a trade (INSERT, UPDATE, CANCEL).
- Trade State: Latest known state of a TradeId, including version and cancellation status.
- Position: Net quantity per security code.
- Side: Buy or Sell.

## 5. Stakeholders and User Roles

- Trader/Operator: submits transaction events.
- Reviewer/Analyst: monitors resulting positions and transaction history.
- Technical user: runs and validates API/client locally.

## 6. Overall Description

### 6.1 Product Perspective

The system is a local, single-environment application with a REST API and a browser client.

### 6.2 Operating Environment

- .NET 8
- Node.js (for frontend build/runtime)
- SQLite file database (EquityPositions.db)
- Localhost endpoints:
  - API: http://localhost:5277
  - Client (dev): http://localhost:5173

### 6.3 Assumptions and Dependencies

- Database is initialized automatically at API startup (EnsureCreated).
- Client expects API at http://localhost:5277/api.
- CORS allows http://localhost:3000 and http://localhost:5173.

## 7. Functional Requirements

### FR-1 Transaction Submission

The system shall accept POST /api/transactions requests with:

- tradeId (integer > 0)
- version (integer > 0)
- securityCode (1-20 chars)
- quantity (integer > 0)
- action (INSERT, UPDATE, CANCEL)
- side (Buy, Sell)

Validation failures shall return HTTP 400 with error details.

### FR-2 INSERT Processing

When action is INSERT:

- The system shall reject duplicate TradeId if trade state already exists.
- The system shall create trade state with current transaction details.
- The system shall update position for securityCode by quantity delta:
  - Buy => +quantity
  - Sell => -quantity

### FR-3 UPDATE Processing

When action is UPDATE:

- The system shall reject if trade does not exist.
- The system shall reject if trade is already cancelled.
- The system shall reject if version is not greater than current version.
- The system shall reverse the previous trade contribution from old security/side/quantity.
- The system shall apply the new contribution from new security/side/quantity.
- The system shall update trade state to the latest values.

### FR-4 CANCEL Processing

When action is CANCEL:

- The system shall reject if trade does not exist.
- The system shall reject if trade is already cancelled.
- The system shall reverse the current trade contribution from positions.
- The system shall mark trade as cancelled.

Note: current implementation updates trade state version on CANCEL but does not enforce monotonic version checks for CANCEL.

### FR-5 Transaction Persistence and Status

- Every accepted transaction shall be persisted.
- Processing shall run in the same request after persistence.
- On successful processing, transaction IsProcessed shall be set to true.

### FR-6 Position Retrieval

The system shall provide:

- GET /api/positions -> all positions
- GET /api/positions/{securityCode} -> specific security position

If security is not found for the second endpoint, return HTTP 404.

### FR-7 Transaction Retrieval

The system shall provide:

- GET /api/transactions -> all transactions
- GET /api/transactions/{id} -> by transaction id
- GET /api/transactions/trade/{tradeId} -> by trade

Missing transaction id shall return HTTP 404.

### FR-8 UI Behavior

The frontend shall:

- Provide a form to submit transactions.
- Show success/error messages from API responses.
- Refresh positions and transaction history after successful submission.
- Support manual refresh for both tables.
- Display timestamps converted from UTC to local time.

## 8. Non-Functional Requirements

### NFR-1 Performance

- Target local response times under normal development load should be near real-time (interactive).

### NFR-2 Reliability

- Business rule violations shall return clear HTTP 400 messages.
- Unexpected server failures shall return HTTP 500 with generic error message.

### NFR-3 Maintainability

- Layered architecture shall be preserved (API, Domain, Persistence, Client).
- Domain rules shall remain centralized in PositionCalculator.

### NFR-4 Portability

- The system shall run on major developer OS environments supported by .NET and Node.js.

## 9. Business Rules

### BR-1 Quantity Delta Rule

- Buy contributes +Q.
- Sell contributes -Q.

### BR-2 Net Position Rule

For each security S:

position(S) = sum of active trade deltas mapped to S

### BR-3 Version Rule

- UPDATE must have version strictly greater than current trade state version.
- INSERT requires no existing trade state for the TradeId.

### BR-4 Trade Identity Rule

TradeId uniquely identifies a logical trade lifecycle.

## 10. Data Requirements

### 10.1 Entities

- Transactions
  - TransactionId (PK)
  - TradeId
  - Version
  - SecurityCode
  - Quantity
  - Action
  - Side
  - CreatedAt
  - IsProcessed
  - Unique index: (TradeId, Version)

- TradeStates
  - Id (PK)
  - TradeId (unique)
  - CurrentVersion
  - SecurityCode
  - Quantity
  - Side
  - IsCancelled
  - LastUpdatedAt

- Positions
  - Id (PK)
  - SecurityCode (unique)
  - Quantity
  - LastUpdatedAt

### 10.2 Data Constraints

- SecurityCode max length: 20
- Quantity > 0 at API input
- Version > 0 at API input
- TradeId > 0 at API input

## 11. API Contract (Examples)

### 11.1 Create Transaction

Endpoint: POST /api/transactions

Request example:

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

Successful response example (HTTP 201):

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

Validation failure example (HTTP 400):

```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "Action must be INSERT, UPDATE, or CANCEL"
  ]
}
```

Business rule failure example (HTTP 400):

```json
{
  "success": false,
  "message": "Trade 101 already exists. Cannot insert duplicate trade.",
  "data": null,
  "errors": []
}
```

### 11.2 Get Positions

Endpoint: GET /api/positions

Response example (HTTP 200):

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": [
    {
      "securityCode": "REL",
      "quantity": 120,
      "lastUpdatedAt": "2026-04-14T10:05:00Z"
    },
    {
      "securityCode": "ITC",
      "quantity": -40,
      "lastUpdatedAt": "2026-04-14T10:06:00Z"
    }
  ],
  "errors": []
}
```

## 12. End-to-End Processing Example

### Scenario A: Insert then Update

1. INSERT trade 101, version 1, REL, Buy, 150
2. UPDATE trade 101, version 2, REL, Buy, 120

Expected position impact:

- Step 1 applies +150 to REL.
- Step 2 reverses old +150, then applies +120.
- Final REL position contribution for trade 101 is +120.

### Scenario B: Insert Sell then Cancel

1. INSERT trade 202, version 1, ITC, Sell, 40
2. CANCEL trade 202, version 2, ITC, Sell, 40

Expected position impact:

- Step 1 applies -40 to ITC.
- Step 2 reverses -40 by applying +40.
- Final ITC contribution for trade 202 is 0.

## 13. Error Handling Requirements

- Invalid payload format or field rules -> HTTP 400 with validation errors.
- Domain rule violations (duplicate insert, invalid update/cancel flow) -> HTTP 400 with business message.
- Not found for GET by id/security -> HTTP 404.
- Unhandled exceptions -> HTTP 500 with generic message.

## 14. Security and Compliance

Current baseline:

- No authentication/authorization controls.
- No encryption-at-rest requirements defined for local SQLite.
- No audit retention policy beyond stored transactions.

## 15. Deployment and Run Requirements

### 15.1 Backend

From solution root:

```bash
cd EquityPositions.Api
dotnet restore
dotnet run
```

### 15.2 Frontend

From client directory:

```bash
cd client
npm install
npm run dev
```

### 15.3 Access

- API Swagger UI: http://localhost:5277
- Frontend UI: http://localhost:5173

## 16. Acceptance Criteria

- Transaction API enforces input validation and business rules per FR-1 to FR-4.
- Positions are updated correctly for INSERT/UPDATE/CANCEL flows.
- API retrieval endpoints return consistent response wrapper format.
- UI submits transactions and reflects updates in both tables.
- Example scenarios in this document can be reproduced manually.

## 17. Known Limitations

- No authentication.
- No pagination/filtering for list endpoints.
- No automated reconciliation against external trade source.
- CANCEL currently depends on existing trade state and does not validate version progression.
