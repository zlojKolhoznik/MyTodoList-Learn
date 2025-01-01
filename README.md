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
3. Restore dependencies in each of the projects
    ```bash
    cd MyTodoList.Api && dotnet restore && cd ../MyTodoList.ApiTests && dotnet restore && cd ../MyTodoList.Data && dotnet restore && cd ../MyTodoList.DataTests && dotnet restore && cd../
    ```
4. Make sure that connection string in `MyTodoList.Api/launchSettings.json` contains your credentials
5. Apply database migrations
    ```bash
    cd MyTodoList.Data && dotnet ef database update --startup-project ../MyTodoList.Api && cd../
    ```
6. Set required user secrets (make sure your key is at least 256 bits)
    ```bash
    cd MyTodoList.Api
    ```
    ```bash
    dotnet user-secrets init
    ```
    ```bash
    dotnet user-secrets set "Jwt:Key" "[YOUR_JWT_KEY]"
    ```
    ```bash
    dotnet user-secrets set "Jwt:Issuer" "[YOUR_JWT_ISSUER]"
    ```
    ```bash
    dotnet user-secrets set "Jwt:Audience" "[YOUR_JWT_AUDIENCE]"
    ```
7. Run the API
    ```bash
    dotnet run
    ```
----------------------------
# Feedback

`Q:` Was it easy to complete the task using AI?

`A:` Yes, it was a lot easier to get almost ready code instead of writing it 
from scratch.

`Q:` How long did task take you to complete?

`A:` It took me approximately 4-5 hours due to my lack of experience when
implementing authorization myself (not with Microsoft Identity).

`Q:`  Was the code ready to run after generation? What did you have to change 
to make it usable?

`A:` No, the code wasn't ready to run after generation. Sometimes I needed to change
small bits of code like variable or class names to make it run, and sometimes 
the algorithm itself was outright wrong and I needed to completely change the code.

`Q:` Which challenges did you face during completion of the task?

`A:` The main challenge was to understand how to work with self-developed user
authorization and JWT tokens. Regarding AI there was little to no challenge
since I have used LLMs earlier.

`Q:` Which specific prompts you learned as a good practice to complete the task?

`A:` Good prompts that I learned were 'I am developing ..., how should I implement
...' and prompts asking general technology usage advice