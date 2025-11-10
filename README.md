# WexPurchaseTransaction

A .NET 9 API for managing purchase transactions, including currency conversion using US Treasury exchange rates.

## Features

- Create purchase transactions.
- List all transactions.
- Retrieve transactions converted to a specific currency.
- Delete transactions by ID.
- Currency conversion based on US Treasury exchange rates (Treasury Reporting Rates of Exchange).

## Technologies

- .NET 9 / C#
- SQLite with Entity Framework Core
- Swagger for API documentation
- xUnit + Moq for unit testing
- Docker & Docker Compose for containerized deployment

## Project Structure

- **Domain**: Entities and core interfaces.
- **Application**: Services, DTOs, and interfaces.
- **Infrastructure**: Repositories, external clients, and data access.
- **WebApi**: Controllers and API setup.

## API Endpoints

### Create a Transaction

**POST** `/api/transaction`

**Request Body Example:**
```json
{
  "description": "Purchase 1",
  "transactionDate": "2025-11-06T14:30:00Z",
  "value": 150.75
}
```

### List All Transactions

**GET** `/api/transaction`

### Get Transaction Converted to a Currency

**GET** `/api/transaction/{id}/convert?currency=Canada-Dollar`

### Delete a Transaction

**DELETE** `/api/transaction/{id}`

---

## Running Locally (without Docker)

Clone the repository:
```bash
git clone https://github.com/eduardozanardo/WexPurchaseTransaction.git
cd WexPurchaseTransaction/src/WebApi
```

Restore dependencies and run:
```bash
dotnet restore
dotnet run
```

---

## Running Tests

Run all unit tests with:
```bash
dotnet test
```

---

## 🚀 Running in Production (Docker)

The application can run fully containerized using **Docker Compose**, including persistence via SQLite.

### 1. Project Structure

```
WexPurchaseTransaction/
│
├── Data/
│   └── purchases.db             # SQLite database (mounted as a Docker volume)
│
├── docker-compose.yml           # Compose definition
└── src/
    ├── Application/
    ├── Domain/
    ├── Infrastructure/
    └── WebApi/
        └── Dockerfile
```

### 2. Build the container

From the **project root**, run:
```bash
docker compose build
```

### 3. Run the container

```bash
docker compose up -d
```

This starts the API in **production mode**, exposing port **5000**.

### 4. Access the API

- Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)
- Health check (optional): [http://localhost:5000/api/transaction](http://localhost:5000/api/transaction)

### 5. Stopping and cleaning up

```bash
docker compose down
```

To also remove the SQLite volume (⚠️ this deletes data):
```bash
docker compose down -v
```

---

## Notes

- The API uses a persistent **SQLite** file mounted at `/app/Data/purchases.db`.
- Environment variables for the connection string are configured in `docker-compose.yml`.
- Swagger is enabled in all environments for easier inspection and testing.
- The container runs under the `Production` environment (`ASPNETCORE_ENVIRONMENT=Production`).
