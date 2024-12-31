# MyTodoList

## Description
MyTodoList is a simple and efficient API designed to help users manage their tasks. It allows users to create, update, delete, and retrieve tasks. The API provides RESTful endpoints for interacting with the to-do list, making it easy to integrate with front-end applications or other services.

## Features
- **Create Task**: Add new tasks with a title, description, and due date.
- **Read Tasks**: Fetch all tasks or a specific task by ID.
- **Update Task**: Modify task details like title, description, or completion status.
- **Delete Task**: Remove tasks from the list.
- **Filter Tasks**: Filter tasks by completion status.

## Technologies Used
- **.NET 8.0**
- **Entity Framework Core**
- **MySQL**
- **OpenAPI**
<!-- - **JWT Authentication** (if implemented) -->

## Installation

0. Prerequisites
    - [.NET SDK 8.0 or higher](https://dotnet.microsoft.com/en-us/download) is installed
    - [MySQL](https://dev.mysql.com/downloads/installer/) is installed (at least server)
    - [dotnet-ef](https://www.nuget.org/packages/dotnet-ef) tool is installed
1. Clone the repository
   ```bash
   git clone https://github.com/zlojKolhoznik/MyTodoList-Learn.git
   ```
2. Navigate to the project directory
    ```bash
    cd MyTodoList-Learn
    ```
3. Restore dependencies
    ```bash
    dotnet restore
    ```
4. Apply database migrations
    ```bash
    dotnet ef database update
    ```
5. Run the API
    ```bash
    dotnet run
    ```



Feel free to customize it according to your project specifics!