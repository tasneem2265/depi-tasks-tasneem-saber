The Entity Relationship Diagram (ERD) represents the database design for a Bookstore Management System. It consists of six main entities: Authors, Books, Categories, Customers, Orders, and OrderItems.
Authors stores information about book authors, including their names and biographies.
Books contains details about each book such as title, price, stock quantity, and references to its author and category.
Categories organizes books into different categories.
Customers stores customer information including name, email, city, and registration date.
Orders records customer purchases and links each order to a specific customer.
OrderItems acts as a junction table between Orders and Books, storing the books included in each order along with their quantity and sale price.
Relationships
One Author can write many Books (1:M).
One Category can contain many Books (1:M).
One Customer can place many Orders (1:M).
One Order can contain many OrderItems (1:M).
One Book can appear in many OrderItems (1:M).
The database enforces referential integrity using foreign keys. It also includes constraints such as UNIQUE on the customer's email, price > 0, and stock ≥ 0 to ensure data accuracy and consistency.

the erd is in the attached photo in repo