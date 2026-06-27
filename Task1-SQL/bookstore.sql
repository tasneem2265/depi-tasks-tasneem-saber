CREATE DATABASE IF NOT EXISTS BookStore;
USE BookStore;

CREATE TABLE Authors (
    author_id   INT AUTO_INCREMENT PRIMARY KEY,
    first_name  VARCHAR(100) NOT NULL,
    last_name   VARCHAR(100) NOT NULL,
    bio         TEXT
);

CREATE TABLE Categories (
    category_id   INT AUTO_INCREMENT PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Books (
    book_id      INT AUTO_INCREMENT PRIMARY KEY,
    title        VARCHAR(255) NOT NULL,
    author_id    INT          NOT NULL,
    category_id  INT          NOT NULL,
    price        DECIMAL(10,2) NOT NULL,
    stock        INT           NOT NULL DEFAULT 0,
    CONSTRAINT fk_book_author   FOREIGN KEY (author_id)   REFERENCES Authors(author_id),
    CONSTRAINT fk_book_category FOREIGN KEY (category_id) REFERENCES Categories(category_id),
    CONSTRAINT chk_price  CHECK (price  > 0),
    CONSTRAINT chk_stock  CHECK (stock >= 0)
);

CREATE TABLE Customers (
    customer_id INT AUTO_INCREMENT PRIMARY KEY,
    first_name  VARCHAR(100) NOT NULL,
    last_name   VARCHAR(100) NOT NULL,
    email       VARCHAR(255) NOT NULL UNIQUE,
    city        VARCHAR(100),
    joined_at   DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Orders (
    order_id    INT AUTO_INCREMENT PRIMARY KEY,
    customer_id INT      NOT NULL,
    order_date  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_order_customer FOREIGN KEY (customer_id) REFERENCES Customers(customer_id)
);

CREATE TABLE OrderItems (
    item_id        INT AUTO_INCREMENT PRIMARY KEY,
    order_id       INT           NOT NULL,
    book_id        INT           NOT NULL,
    quantity       INT           NOT NULL DEFAULT 1,
    price_at_sale  DECIMAL(10,2) NOT NULL,
    CONSTRAINT fk_item_order FOREIGN KEY (order_id) REFERENCES Orders(order_id),
    CONSTRAINT fk_item_book  FOREIGN KEY (book_id)  REFERENCES Books(book_id)
        ON DELETE RESTRICT,
    CONSTRAINT chk_qty       CHECK (quantity >= 1),
    CONSTRAINT chk_sale_price CHECK (price_at_sale > 0)
);

INSERT INTO Authors (first_name, last_name, bio) VALUES
('Robert',   'Martin',   'Software engineer and author of Clean Code.'),
('Martin',   'Fowler',   'Author and speaker on software design.'),
('Andrew',   'Hunt',     'Co-author of The Pragmatic Programmer.'),
('David',    'Thomas',   'Co-author of The Pragmatic Programmer.'),
('Eric',     'Evans',    'Author of Domain-Driven Design.'),
('Erich',    'Gamma',    'Co-author of Design Patterns (GoF).'),
('Yuval',    'Harari',   'Historian and author of Sapiens.'),
('George',   'Orwell',   'Author of 1984 and Animal Farm.'),
('Frank',    'Herbert',  'Author of Dune.'),
('J.K.',     'Rowling',  'Author of the Harry Potter series.'),
('Agatha',   'Christie', 'Queen of crime fiction.'),
('Stephen',  'Hawking',  'Theoretical physicist and author.'),
('Carl',     'Sagan',    'Astronomer and science communicator.'),
('Malcolm',  'Gladwell', 'Journalist and bestselling author.'),
('Nassim',   'Taleb',    'Author of The Black Swan.');

INSERT INTO Categories (category_name) VALUES
('Software Engineering'),
('Architecture & Design'),
('History'),
('Science Fiction'),
('Mystery'),
('Science & Physics'),
('Psychology & Behavior'),
('Fantasy');

INSERT INTO Books (title, author_id, category_id, price, stock) VALUES
('Clean Code',                          1,  1,  39.99, 50),
('The Clean Coder',                     1,  1,  34.99, 30),
('Refactoring',                         2,  2,  44.99, 25),
('Patterns of Enterprise Application',  2,  2,  49.99, 20),
('The Pragmatic Programmer',            3,  1,  42.99, 40),
('Domain-Driven Design',                5,  2,  47.99, 15),
('Design Patterns',                     6,  2,  52.99, 10),
('Sapiens',                             7,  3,  18.99, 60),
('Homo Deus',                           7,  3,  17.99, 45),
('21 Lessons for the 21st Century',     7,  3,  16.99, 35),
('1984',                                8,  4,  12.99, 80),
('Animal Farm',                         8,  4,   9.99, 70),
('Dune',                                9,  4,  14.99, 55),
("Dune Messiah",                        9,  4,  13.99, 40),
("Harry Potter and the Sorcerer's Stone",10,8,  15.99, 90),
('Harry Potter and the Chamber of Secrets',10,8,15.99, 85),
('Murder on the Orient Express',        11,  5,  11.99, 50),
('And Then There Were None',            11,  5,  10.99, 60),
('Death on the Nile',                   11,  5,  11.49, 45),
('A Brief History of Time',             12,  6,  13.99, 30),
('The Universe in a Nutshell',          12,  6,  16.99, 20),
('Cosmos',                              13,  6,  14.99, 25),
('The Pale Blue Dot',                   13,  6,  15.49, 18),
('The Tipping Point',                   14,  7,  13.99, 35),
('Outliers',                            14,  7,  12.99, 40),
('Blink',                               14,  7,  11.99, 30),
('The Black Swan',                      15,  7,  17.99, 22),
('Antifragile',                         15,  7,  19.99, 18),
('Skin in the Game',                    15,  7,  18.49, 14),
('Fooled by Randomness',                15,  7,  16.99, 12);

INSERT INTO Customers (first_name, last_name, email, city) VALUES
('Ahmed',   'Hassan',   'ahmed.hassan@email.com',   'Cairo'),
('Sara',    'Mohamed',  'sara.mohamed@email.com',   'Alexandria'),
('Omar',    'Ali',      'omar.ali@email.com',       'Cairo'),
('Nour',    'Ibrahim',  'nour.ibrahim@email.com',   'Giza'),
('Karim',   'Mahmoud',  'karim.mahmoud@email.com',  'Cairo'),
('Layla',   'Youssef',  'layla.youssef@email.com',  'Alexandria'),
('Tarek',   'Samir',    'tarek.samir@email.com',    'Luxor'),
('Hana',    'Farouk',   'hana.farouk@email.com',    'Aswan'),
('Youssef', 'Nabil',    'youssef.nabil@email.com',  'Cairo'),
('Dina',    'Khaled',   'dina.khaled@email.com',    'Giza'),
('Mostafa', 'Ragab',    'mostafa.ragab@email.com',  'Alexandria'),
('Rania',   'Adel',     'rania.adel@email.com',     'Cairo'),

('Ziad',    'Fathi',    'ziad.fathi@email.com',     'Luxor'),
('Mona',    'Sherif',   'mona.sherif@email.com',    'Aswan');

INSERT INTO Orders (customer_id, order_date) VALUES
(1,  '2025-01-05 10:00:00'),
(1,  '2025-02-14 11:30:00'),
(2,  '2025-01-10 09:15:00'),
(3,  '2025-01-20 14:00:00'),
(3,  '2025-03-05 16:00:00'),
(4,  '2025-02-01 08:45:00'),
(5,  '2025-01-15 12:00:00'),
(5,  '2025-03-20 13:30:00'),
(6,  '2025-02-18 10:00:00'),
(7,  '2025-01-25 15:00:00'),
(8,  '2025-03-10 09:00:00'),
(9,  '2025-01-30 17:00:00'),
(10, '2025-02-22 11:00:00'),
(11, '2025-03-01 14:00:00'),
(12, '2025-04-05 10:30:00'),
(1,  '2025-04-12 09:00:00'),
(2,  '2025-04-18 11:00:00'),
(3,  '2025-05-02 15:00:00'),
(4,  '2025-05-10 10:00:00'),
(5,  '2025-05-15 13:00:00');

INSERT INTO OrderItems (order_id, book_id, quantity, price_at_sale) VALUES

(1,  1,  2, 39.99),
(1,  8,  1, 18.99),

(2,  11, 1, 12.99),
(2,  13, 1, 14.99),
(2,  20, 1, 13.99),

(3,  15, 2, 15.99),
(3,  16, 1, 15.99),

(4,  3,  1, 44.99),
(4,  6,  1, 47.99),

(5,  25, 1, 12.99),
(5,  26, 1, 11.99),

(6,  7,  1, 52.99),
(6,  4,  1, 49.99),

(7,  9,  1, 17.99),
(7,  10, 1, 16.99),
(7,  12, 1,  9.99),

(8,  22, 1, 14.99),
(8,  24, 1, 13.99),

(9,  17, 2, 11.99),
(9,  18, 2, 10.99),

(10, 5,  1, 42.99),
(10, 1,  1, 39.99),

(11, 27, 1, 17.99),
(11, 28, 1, 19.99),

(12, 14, 1, 13.99),
(12, 13, 1, 14.99),

(13, 21, 1, 16.99),
(13, 23, 1, 15.49),

(14, 2,  1, 34.99),
(14, 5,  1, 42.99),

(15, 15, 3, 15.99),
(15, 11, 2, 12.99),

(16, 29, 1, 18.49),
(16, 30, 1, 16.99),

(17, 19, 2, 11.49),
(17, 17, 1, 11.99),

(18, 8,  1, 18.99),
(18, 9,  1, 17.99),

(19, 24, 1, 13.99),
(19, 26, 1, 11.99),

(20, 1,  2, 39.99),
(20, 3,  1, 44.99);

SELECT
    title,
    price
FROM Books
ORDER BY price DESC;

SELECT
    UPPER(b.title)                                         AS book_title,
    LOWER(CONCAT(a.first_name, ' ', a.last_name))         AS author_name
FROM Books b
JOIN Authors a ON b.author_id = a.author_id;

SELECT
    b.title,
    c.category_name AS category,
    CONCAT(a.first_name, ' ', a.last_name) AS author
FROM Books b
JOIN Categories c ON b.category_id = c.category_id
JOIN Authors    a ON b.author_id   = a.author_id
ORDER BY b.title;

SELECT
    CONCAT(c.first_name, ' ', c.last_name) AS customer,
    COUNT(o.order_id)                       AS total_orders
FROM Customers c
LEFT JOIN Orders o ON c.customer_id = o.customer_id
GROUP BY c.customer_id, c.first_name, c.last_name
ORDER BY total_orders DESC;

SELECT
    b.title,
    SUM(oi.quantity) AS total_sold
FROM Books b
JOIN OrderItems oi ON b.book_id = oi.book_id
GROUP BY b.book_id, b.title
ORDER BY total_sold DESC
LIMIT 5;

SELECT
    city,
    COUNT(*) AS customer_count
FROM Customers
WHERE city IS NOT NULL
GROUP BY city
ORDER BY customer_count DESC
LIMIT 1;

SELECT
    c.category_name,
    COUNT(b.book_id) AS book_count
FROM Categories c
JOIN Books b ON c.category_id = b.category_id
GROUP BY c.category_id, c.category_name
HAVING book_count > 5
ORDER BY book_count DESC;

SELECT
    title,
    price
FROM Books
WHERE price > (SELECT AVG(price) FROM Books)
ORDER BY price DESC;

SELECT
    CONCAT(c.first_name, ' ', c.last_name) AS customer,
    c.email
FROM Customers c
LEFT JOIN Orders o ON c.customer_id = o.customer_id
WHERE o.order_id IS NULL;

SELECT
    DATE_FORMAT(o.order_date, '%Y-%m') AS month,
    SUM(oi.quantity * oi.price_at_sale) AS total_revenue
FROM Orders o
JOIN OrderItems oi ON o.order_id = oi.order_id
GROUP BY month
ORDER BY month;

CREATE OR REPLACE VIEW vw_BookCatalog AS
SELECT
    b.book_id,
    b.title,
    c.category_name                                AS category,
    CONCAT(a.first_name, ' ', a.last_name)        AS author,
    b.price
FROM Books b
JOIN Categories c ON b.category_id = c.category_id
JOIN Authors    a ON b.author_id   = a.author_id;

SELECT * FROM vw_BookCatalog ORDER BY title;

DELIMITER $$

CREATE PROCEDURE GetCustomerPurchases(IN p_customer_id INT)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM Customers WHERE customer_id = p_customer_id) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Customer not found.';
    END IF;


    SELECT
        o.order_id,
        o.order_date,
        b.title                         AS book_title,
        oi.quantity,
        oi.price_at_sale,
        (oi.quantity * oi.price_at_sale) AS line_total
    FROM Orders o
    JOIN OrderItems oi ON o.order_id  = oi.order_id
    JOIN Books      b  ON oi.book_id  = b.book_id
    WHERE o.customer_id = p_customer_id
    ORDER BY o.order_date, o.order_id;


    SELECT
        o.order_id,
        o.order_date,
        SUM(oi.quantity * oi.price_at_sale) AS order_total
    FROM Orders o
    JOIN OrderItems oi ON o.order_id = oi.order_id
    WHERE o.customer_id = p_customer_id
    GROUP BY o.order_id, o.order_date
    ORDER BY o.order_date;


    SELECT
        SUM(oi.quantity * oi.price_at_sale) AS grand_total
    FROM Orders o
    JOIN OrderItems oi ON o.order_id = oi.order_id
    WHERE o.customer_id = p_customer_id;
END$$

DELIMITER ;

CALL GetCustomerPurchases(1);
