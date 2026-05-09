using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Food.Models
{
    public partial class User : ObservableObject
    {
        public int Id { get; set; }
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private string _phone = string.Empty;
        [ObservableProperty] private string _address = string.Empty;
        public bool IsAdmin { get; set; }
    }

    public partial class Product : ObservableObject
    {
        public int Id { get; set; }
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private double _price;
        [ObservableProperty] private string _imagePath = string.Empty;
        [ObservableProperty] private string _category = string.Empty;
        [ObservableProperty] private bool _isLiked;
    }

    public partial class Order : ObservableObject
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ObservableProperty] private string _userName = string.Empty;
        public DateTime OrderDate { get; set; }
        [ObservableProperty] private double _totalAmount;
        [ObservableProperty] private string _status = "Pending";
        [ObservableProperty] private string _address = string.Empty;
        [ObservableProperty] private string _phone = string.Empty;
        [ObservableProperty] private string _paymentMethod = "COD";
        public List<OrderItem> Items { get; set; } = new();
    }

    public partial class OrderItem : ObservableObject
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        [ObservableProperty] private string _productName = string.Empty;
        [ObservableProperty] private int _quantity;
        [ObservableProperty] private double _price;
    }
    public partial class Offer : ObservableObject
    {
        public int Id { get; set; }
        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private string _imagePath = string.Empty;
    }

    public partial class CarouselImage : ObservableObject
    {
        public int Id { get; set; }
        [ObservableProperty] private string _imagePath = string.Empty;
    }

    public partial class Advertisement : ObservableObject
    {
        public int Id { get; set; }
        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private string _imagePath = string.Empty;
    }
}
