# Product Management System (Console Application)

This console application is a complete system for managing an inventory of different product types. It was developed as a university project for an advanced C# course, with a focus on deeply understanding object-oriented programming, custom data structures, and algorithmic implementation.

The project serves as a strong demonstration of backend logic, robust software architecture, and fundamental computer science principles in a .NET environment.

---

## Key Features & Concepts Demonstrated

### Core C# and .NET
- **Object-Oriented Programming:** A well-defined class hierarchy with `abstract` classes, inheritance, and polymorphism.
- **Generics:** Custom data structures (`ArrayContainer`, `LinkedListContainer`) are fully generic.
- **Delegates & Events:** Used for custom sorting logic (`MyComparison<T>`) and for event-driven notifications (`INotifyTotalPriceChanged`).
- **LINQ:** Used for complex data queries and aggregations in the menu.
- **Exception Handling:** Custom exception types for robust error management.
- **File I/O:** Binary serialization to save and load the application state.

### Data Structures & Algorithms
- **Custom Dynamic Array:** Implemented from scratch with capacity management, insertion, and removal.
- **Custom Doubly Linked List:** A fully-featured linked list implementation.
- **Sorting Algorithms:**
  - **Insertion Sort** (for the array-based collection).
  - **Merge Sort** (for the linked-list-based collection).

---

## How to Run the Project

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/AntonKhPI2/Product-Management-System
    ```
2.  **Open the solution:**
    Navigate to the project folder and open the `ConsoleApp1.sln` file in Visual Studio or JetBrains Rider.
3.  **Build and Run:**
    Build the solution and run the project (usually by pressing F5 or the "Run" button). The application will start in your terminal.
