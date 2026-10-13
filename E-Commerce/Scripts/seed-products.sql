-- Inserts a random number (50-100) of sample products into "Products".
-- Run: psql -h localhost -p 5433 -U postgres -d Ecommerce -f Scripts/seed-products.sql
INSERT INTO "Products" ("Name", "Description", "Price", "Stock", "CreatedAt")
SELECT 'Product ' || n,
       'Sample description for product ' || n,
       round((random() * 990 + 10):: numeric, 2), -- 10.00 - 1000.00
       floor(random() * 500)::int,                 -- 0 - 499 now() - (random() * interval '90 days')
FROM generate_series(1, 50 + floor(random() * 51)::int) AS n; -- 50 to 100 rows
