# Student Management System

A comprehensive student management system built with C# and WPF, featuring role-based access control, student and course management, and course registration capabilities.

## Features

- Role-based access control (Admin, Super-Admin, Clerk)
- Student management (CRUD operations)
- Course management (CRUD operations)
- Course registration system
- User authentication and authorization
- Modern WPF UI with Material Design

## Prerequisites

- .NET 6.0 or later
- Visual Studio 2022 or later
- SQL Server (LocalDB or full instance)

## Setup Instructions

1. Clone the repository:
   ```bash
   git clone [repository-url]
   ```

2. Open the solution in Visual Studio

3. Update the connection string in `appsettings.json` to point to your SQL Server instance

4. Run the following commands in Package Manager Console:
   ```powershell
   Update-Database
   ```

5. Build and run the application

## Project Structure

- `LoginSystem/` - Main application project
  - `Models/` - Data models and entities
  - `Views/` - WPF windows and user interfaces
  - `ViewModels/` - MVVM view models
  - `Services/` - Business logic and services
  - `Data/` - Database context and migrations

## Contributing

1. Create a new branch for your feature
2. Make your changes
3. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 