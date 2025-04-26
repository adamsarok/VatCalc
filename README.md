# VatCalc API

This project provides a simple VAT calculation service.

## Design Choices

- Vertical slice architecture with Carter and Minimal API is implemented to enable extensibility, clarity and ease of maintenance
- CQRS with MediatR ensures loose coupling and separation of concerns
- A rich domain entity is built in VatResult. Validation and business logic is encapsulated in the constructor to ensure the entity is never in an invalid state

## Testing

- To ensure correctness of financial calculations, a full test suite is included
- Unit tests validate the business logic in isolation
- System tests validate the API end-to-end
