# Equity Positions Client

This folder contains the React frontend for the Equity Positions system.

For project onboarding, see the root README. For complete requirements and business rules, see the SRS at the solution root:

- `../README.md`
- `../SRS.md`

## What This UI Does

- Submits transaction events (INSERT, UPDATE, CANCEL)
- Displays current net positions by security
- Displays transaction history
- Refreshes tables after successful transaction submission

## Local Run

```bash
npm install
npm run dev
```

Default dev URL:

- http://localhost:5173

Expected backend API URL:

- http://localhost:5277/api

## Example Payload (from UI/API contract)

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

## Notes

- Timestamps are converted from UTC to the browser local time zone.
- If API is unavailable, the UI shows a network error message.
