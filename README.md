# Car Auction Management System

![Build and Test Status](https://github.com/renatolopes/CarAuctionManagementSystem/actions/workflows/build_and_test.yml/badge.svg)

## Project Overview

This project is a simple car auction management system developed for illustrative purposes.  
It manages vehicle inventory and the auction process entirely in memory, without a database.  
Database support can be added easily if required.

The system follows Clean Architecture principles and Domain-Driven Design (DDD) concepts to ensure modularity, scalability, and maintainability.

---

## Architecture

The application is divided into distinct layers, each with specific responsibilities:

- **API Layer**: Handles HTTP requests, delegates processing to the application layer, and returns responses.  
- **Application Layer**: Contains application-specific business logic and coordinates use cases.  
- **Domain Layer**: Defines core business entities and rules, independent of infrastructure.  
- **Infrastructure Layer**: Manages data persistence and external integrations. In this implementation, data is held in memory but can be extended to use external databases or services.

---

## Domain Model

Two main entities represent the core domain:

- **Vehicle**: Represents the vehicle being auctioned, uniquely identified by its license plate.  
- **Auction**: Represents the auction process for a vehicle and contains multiple bids.

These entities are managed by dedicated application services and stored in memory, using repository and specification patterns for separation of concerns and testability.

---

## Business Rules Overview

The system enforces validation rules to ensure data integrity and domain consistency:

### 🔧 Vehicle Rules
- Each vehicle must have a **unique license plate** — duplicates are not allowed.
- The manufacturing year of a vehicle must be greater than or equal to 1886 (the year the first modern vehicle, the Benz Patent-Motorwagen, was constructed) and cannot exceed the current year.
- **Number of doors** is required for:
  - Hatchbacks and Sedans: must be between **3 and 5**.
- **Number of seats** is required for:
  - SUVs: must be between **5 and 8**.
- **Load capacity** is required for:
  - Trucks: must be between **10,000 and 50,000**.
- Invalid field combinations:
  - **Hatchbacks, Sedans, and SUVs** must **not** define a load capacity.
  - **Hatchbacks, Sedans, and Trucks** must **not** define number of seats.
  - **SUVs and Trucks** must **not** define number of doors.

### 🏷️ Auction Rules
- An auction **must reference a valid vehicle** via license plate.
- It's not allowed to create more than **one auction per vehicle**.
- **Starting bid** must be at least **1**.
- An auction **cannot be created** for vehicles that are not in the inventory.

### 🔄 Auction Lifecycle Rules
- An auction **must be created** before it can be started or closed.
- An auction that is **already closed** cannot be started again.
- An auction that is **already started** cannot be started again.
- An auction that has **not yet started** cannot be closed.

### 💰 Bid Rules
- Bids can only be placed on **existing** and **started** auctions.
- A bid must be **greater than or equal to** the starting bid and **higher than the previous bid**.

---

## Tests

The project contains both **unit tests** and **integration tests**:

- **Unit tests** cover application layer services, focusing on business logic validation.  
- **Integration tests** verify interaction between API controllers, application services, and domain entities, ensuring end-to-end functionality.

---

## How to Run

To run the application locally, simply use Docker with "docker-compose up" command.

---

## Build and Test Pipeline
This project includes a continuous integration (CI) pipeline configured on GitHub Actions.
The pipeline automatically builds the project and runs all tests whenever a pull request is opened against the main branch, ensuring code quality and preventing regressions.

---

## Swagger
The API is fully documented with Swagger (OpenAPI).
You can explore the available endpoints and try requests directly through the Swagger UI when the application is running.