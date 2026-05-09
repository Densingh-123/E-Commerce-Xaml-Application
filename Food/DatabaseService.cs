using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Food.Models;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace Food.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "food.db");
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Phone TEXT,
                    Address TEXT,
                    IsAdmin INTEGER DEFAULT 0
                );";

            string createProductsTable = @"
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Price REAL NOT NULL,
                    ImagePath TEXT,
                    Category TEXT
                );";

            string createOrdersTable = @"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    UserName TEXT,
                    OrderDate TEXT,
                    TotalAmount REAL,
                    Status TEXT,
                    Address TEXT,
                    Phone TEXT,
                    PaymentMethod TEXT,
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );";

            string createOrderItemsTable = @"
                CREATE TABLE IF NOT EXISTS OrderItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderId INTEGER NOT NULL,
                    ProductId INTEGER NOT NULL,
                    ProductName TEXT,
                    Quantity INTEGER,
                    Price REAL,
                    FOREIGN KEY(OrderId) REFERENCES Orders(Id)
                );";

            string createLikedProductsTable = @"
                CREATE TABLE IF NOT EXISTS LikedProducts (
                    UserId INTEGER NOT NULL,
                    ProductId INTEGER NOT NULL,
                    PRIMARY KEY(UserId, ProductId)
                );";
            string createOffersTable = @"
                CREATE TABLE IF NOT EXISTS Offers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT,
                    Description TEXT,
                    ImagePath TEXT
                );";

            string createCarouselImagesTable = @"
                CREATE TABLE IF NOT EXISTS CarouselImages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ImagePath TEXT
                );";

            string createAdsTable = @"
                CREATE TABLE IF NOT EXISTS Advertisements (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT,
                    ImagePath TEXT
                );";

            using var command = connection.CreateCommand();
            command.CommandText = createUsersTable + createProductsTable + createOrdersTable + createOrderItemsTable + createLikedProductsTable + createOffersTable + createCarouselImagesTable + createAdsTable;
            command.ExecuteNonQuery();

            // Seed Admin
            SeedAdmin(connection);
        }

        private void SeedAdmin(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT OR IGNORE INTO Users (Name, Email, Password, IsAdmin) VALUES ('Admin Den', 'den@gmail.com', 'admin123', 1);";
            command.ExecuteNonQuery();
        }
        // --- User Methods ---
        public User? Login(string email, string password)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Email = @email AND Password = @password";
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@password", password);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    Phone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Address = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    IsAdmin = reader.GetInt32(6) == 1
                };
            }
            return null;
        }

        public bool Register(User user)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Users (Name, Email, Password, Phone, Address, IsAdmin) VALUES (@name, @email, @pass, @phone, @addr, @admin)";
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@pass", user.Password);
                command.Parameters.AddWithValue("@phone", user.Phone);
                command.Parameters.AddWithValue("@addr", user.Address);
                command.Parameters.AddWithValue("@admin", user.IsAdmin ? 1 : 0);
                command.ExecuteNonQuery();
                return true;
            }
            catch { return false; }
        }

        public void UpdateUser(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET Name = @name, Phone = @phone, Address = @address WHERE Id = @id";
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@phone", user.Phone);
            command.Parameters.AddWithValue("@address", user.Address);
            command.Parameters.AddWithValue("@id", user.Id);
            command.ExecuteNonQuery();
        }

        // --- Product Methods ---
        public void AddProduct(Product p)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Products (Name, Description, Price, ImagePath, Category) VALUES (@name, @desc, @price, @path, @cat)";
            command.Parameters.AddWithValue("@name", p.Name);
            command.Parameters.AddWithValue("@desc", p.Description);
            command.Parameters.AddWithValue("@price", p.Price);
            command.Parameters.AddWithValue("@path", p.ImagePath);
            command.Parameters.AddWithValue("@cat", p.Category);
            command.ExecuteNonQuery();
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users";
            using var reader = command.ExecuteReader();
            while(reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Phone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Address = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    IsAdmin = reader.GetInt32(6) == 1
                });
            }
            return users;
        }

        // --- Product Methods (continued) ---
        public void UpdateProduct(Product p)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Products SET Name=@name, Description=@desc, Price=@price, ImagePath=@path, Category=@cat WHERE Id=@id";
            command.Parameters.AddWithValue("@name", p.Name);
            command.Parameters.AddWithValue("@desc", p.Description);
            command.Parameters.AddWithValue("@price", p.Price);
            command.Parameters.AddWithValue("@path", p.ImagePath);
            command.Parameters.AddWithValue("@cat", p.Category);
            command.Parameters.AddWithValue("@id", p.Id);
            command.ExecuteNonQuery();
        }

        public void RemoveProduct(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Products WHERE Id=@id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public List<string> GetCategories()
        {
            var categories = new List<string> { "All" };
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT DISTINCT Category FROM Products WHERE Category IS NOT NULL AND Category != ''";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(reader.GetString(0));
            }
            return categories;
        }

        public List<Product> GetProducts(int userId)
        {
            var products = new List<Product>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            
            // Get Liked Products for this user
            var likedIds = new HashSet<int>();
            using var likedCmd = connection.CreateCommand();
            likedCmd.CommandText = "SELECT ProductId FROM LikedProducts WHERE UserId = @uid";
            likedCmd.Parameters.AddWithValue("@uid", userId);
            using (var reader = likedCmd.ExecuteReader())
            {
                while (reader.Read()) likedIds.Add(reader.GetInt32(0));
            }

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Products";
            using var reader2 = command.ExecuteReader();
            while (reader2.Read())
            {
                int id = reader2.GetInt32(0);
                products.Add(new Product
                {
                    Id = id,
                    Name = reader2.GetString(1),
                    Description = reader2.GetString(2),
                    Price = reader2.GetDouble(3),
                    ImagePath = reader2.GetString(4),
                    Category = reader2.GetString(5),
                    IsLiked = likedIds.Contains(id)
                });
            }
            return products;
        }

        public void ToggleLike(int userId, int productId, bool isLiked)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            if (isLiked)
                command.CommandText = "INSERT OR IGNORE INTO LikedProducts (UserId, ProductId) VALUES (@uid, @pid)";
            else
                command.CommandText = "DELETE FROM LikedProducts WHERE UserId = @uid AND ProductId = @pid";
            
            command.Parameters.AddWithValue("@uid", userId);
            command.Parameters.AddWithValue("@pid", productId);
            command.ExecuteNonQuery();
        }

        // --- Order Methods ---
        public void PlaceOrder(Order order)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO Orders (UserId, UserName, OrderDate, TotalAmount, Status, Address, Phone, PaymentMethod) 
                    VALUES (@uid, @uname, @date, @total, @status, @addr, @phone, @pay);
                    SELECT last_insert_rowid();";
                
                command.Parameters.AddWithValue("@uid", order.UserId);
                command.Parameters.AddWithValue("@uname", order.UserName);
                command.Parameters.AddWithValue("@date", DateTime.Now.ToString("o"));
                command.Parameters.AddWithValue("@total", order.TotalAmount);
                command.Parameters.AddWithValue("@status", "Pending");
                command.Parameters.AddWithValue("@addr", order.Address);
                command.Parameters.AddWithValue("@phone", order.Phone);
                command.Parameters.AddWithValue("@pay", order.PaymentMethod);

                long orderId = (long)command.ExecuteScalar()!;

                foreach (var item in order.Items)
                {
                    using var itemCmd = connection.CreateCommand();
                    itemCmd.Transaction = transaction;
                    itemCmd.CommandText = "INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price) VALUES (@oid, @pid, @pname, @qty, @price)";
                    itemCmd.Parameters.AddWithValue("@oid", orderId);
                    itemCmd.Parameters.AddWithValue("@pid", item.ProductId);
                    itemCmd.Parameters.AddWithValue("@pname", item.ProductName);
                    itemCmd.Parameters.AddWithValue("@qty", item.Quantity);
                    itemCmd.Parameters.AddWithValue("@price", item.Price);
                    itemCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<Order> GetAllOrders()
        {
            var orders = new List<Order>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Orders ORDER BY OrderDate DESC";
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var order = new Order
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    UserName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    OrderDate = DateTime.Parse(reader.GetString(3)),
                    TotalAmount = reader.GetDouble(4),
                    Status = reader.GetString(5),
                    Address = reader.GetString(6),
                    Phone = reader.GetString(7),
                    PaymentMethod = reader.GetString(8)
                };
                orders.Add(order);
            }

            foreach(var order in orders)
            {
                using var itemCmd = connection.CreateCommand();
                itemCmd.CommandText = "SELECT * FROM OrderItems WHERE OrderId = @oid";
                itemCmd.Parameters.AddWithValue("@oid", order.Id);
                using var itemReader = itemCmd.ExecuteReader();
                while(itemReader.Read())
                {
                    order.Items.Add(new OrderItem
                    {
                        Id = itemReader.GetInt32(0),
                        OrderId = itemReader.GetInt32(1),
                        ProductId = itemReader.GetInt32(2),
                        ProductName = itemReader.GetString(3),
                        Quantity = itemReader.GetInt32(4),
                        Price = itemReader.GetDouble(5)
                    });
                }
            }

            return orders;
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Orders SET Status = @status WHERE Id = @id";
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", orderId);
            command.ExecuteNonQuery();
        }

        // --- Offers & Carousel Methods ---
        public List<Offer> GetOffers()
        {
            var offers = new List<Offer>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Offers";
            using var reader = command.ExecuteReader();
            while(reader.Read())
            {
                offers.Add(new Offer {
                    Id = reader.GetInt32(0),
                    Title = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    ImagePath = reader.IsDBNull(3) ? "" : reader.GetString(3)
                });
            }
            return offers;
        }

        public void AddOffer(Offer o)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Offers (Title, Description, ImagePath) VALUES (@t, @d, @i)";
            command.Parameters.AddWithValue("@t", o.Title);
            command.Parameters.AddWithValue("@d", o.Description);
            command.Parameters.AddWithValue("@i", o.ImagePath);
            command.ExecuteNonQuery();
        }

        public void RemoveOffer(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Offers WHERE Id=@id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public List<CarouselImage> GetCarouselImages()
        {
            var imgs = new List<CarouselImage>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM CarouselImages";
            using var reader = command.ExecuteReader();
            while(reader.Read())
            {
                imgs.Add(new CarouselImage {
                    Id = reader.GetInt32(0),
                    ImagePath = reader.IsDBNull(1) ? "" : reader.GetString(1)
                });
            }
            return imgs;
        }

        public void AddCarouselImage(CarouselImage c)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO CarouselImages (ImagePath) VALUES (@i)";
            command.Parameters.AddWithValue("@i", c.ImagePath);
            command.ExecuteNonQuery();
        }

        public void RemoveCarouselImage(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM CarouselImages WHERE Id=@id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public List<Advertisement> GetAdvertisements()
        {
            var ads = new List<Advertisement>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Advertisements";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Advertisement
                {
                    Id = reader.GetInt32(0),
                    Title = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    ImagePath = reader.IsDBNull(2) ? "" : reader.GetString(2)
                });
            }
            return ads;
        }

        public void AddAdvertisement(Advertisement ad)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Advertisements (Title, ImagePath) VALUES (@t, @i)";
            command.Parameters.AddWithValue("@t", ad.Title);
            command.Parameters.AddWithValue("@i", ad.ImagePath);
            command.ExecuteNonQuery();
        }

        public void RemoveAdvertisement(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Advertisements WHERE Id=@id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }
    }
}
