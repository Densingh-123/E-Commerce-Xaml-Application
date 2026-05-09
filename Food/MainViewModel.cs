using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Food.Models;
using Food.Services;

namespace Food.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseService _db = new();

        [ObservableProperty]
        private string _currentPage = "Login";

        [ObservableProperty]
        private User? _currentUser;

        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        [ObservableProperty]
        private ObservableCollection<Product> _filteredProducts = new();

        [ObservableProperty]
        private string _searchText = "";

        [ObservableProperty]
        private ObservableCollection<Product> _cartItems = new();

        [ObservableProperty]
        private ObservableCollection<Order> _allOrders = new();

        [ObservableProperty]
        private ObservableCollection<Order> _userOrders = new();

        [ObservableProperty]
        private Product? _selectedProduct;

        [ObservableProperty]
        private Order? _selectedOrder;

        [ObservableProperty]
        private double _cartTotal;

        [ObservableProperty]
        private string _toastMessage = "";

        [ObservableProperty]
        private bool _isToastVisible;

        [ObservableProperty]
        private bool _isChangePasswordPopupOpen;

        [ObservableProperty]
        private bool _isPrivacyPopupOpen;

        [RelayCommand]
        public void OpenChangePassword() => IsChangePasswordPopupOpen = true;

        [RelayCommand]
        public void CloseChangePassword() => IsChangePasswordPopupOpen = false;

        [RelayCommand]
        public void OpenPrivacyPolicy() => IsPrivacyPopupOpen = true;

        [RelayCommand]
        public void ClosePrivacyPolicy() => IsPrivacyPopupOpen = false;

        [RelayCommand]
        public void SavePassword(object parameter)
        {
            var values = (object[])parameter;
            string oldPass = (string)values[0];
            string newPass = (string)values[1];
            
            if (oldPass != CurrentUser.Password)
            {
                ShowToast("Incorrect old password!");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(newPass) || newPass.Length < 8)
            {
                ShowToast("New password must be at least 8 characters!");
                return;
            }
            
            CurrentUser.Password = newPass;
            _db.UpdateUser(CurrentUser);
            ShowToast("Password changed successfully!");
            IsChangePasswordPopupOpen = false;
        }

        [ObservableProperty]
        private bool _isCartPopupOpen;

        [ObservableProperty]
        private bool _isProfilePopupOpen;

        [ObservableProperty]
        private string _selectedCategory = "All";

        [ObservableProperty]
        private ObservableCollection<string> _categories = new();

        [ObservableProperty]
        private ObservableCollection<Offer> _offers = new();

        [ObservableProperty]
        private ObservableCollection<Advertisement> _advertisements = new();

        [ObservableProperty]
        private ObservableCollection<CarouselImage> _adminCarouselImages = new();

        [ObservableProperty]
        private User? _adminSelectedUser;

        [ObservableProperty]
        private ObservableCollection<Order> _adminUserOrders = new();

        [ObservableProperty]
        private Offer _newOffer = new();

        [ObservableProperty]
        private CarouselImage _newCarouselImage = new();

        [ObservableProperty]
        private Advertisement _newAdvertisement = new();

        [ObservableProperty]
        private Product _newProduct = new();

        [ObservableProperty]
        private string _adImage = "Assets/website/ad-banner.png";

        [ObservableProperty]
        private string _adText = "Welcome to D-Mart! Freshness Guaranteed.";

        [RelayCommand]
        public void UploadProductImage() => NewProduct.ImagePath = OpenFile();

        [RelayCommand]
        public void UploadCarouselImage() => NewCarouselImage.ImagePath = OpenFile();

        [RelayCommand]
        public void UploadOfferImage() => NewOffer.ImagePath = OpenFile();

        [RelayCommand]
        public void UploadAdImage() => NewAdvertisement.ImagePath = OpenFile();

        private string OpenFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };
            return dialog.ShowDialog() == true ? dialog.FileName : "";
        }

        [ObservableProperty]
        private ObservableCollection<string> _carouselImages = new();

        [ObservableProperty]
        private string _currentCarouselImage = "Assets/hero-premium.png";

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private ObservableCollection<User> _allUsers = new();

        private int _carouselIndex = 0;
        private System.Windows.Threading.DispatcherTimer _carouselTimer = new();

        public MainViewModel()
        {
            InitializeData();
            StartCarousel();
        }

        private void InitializeData()
        {
            LoadProducts();
            LoadCategories();
            LoadOffers();
            LoadCarouselImages();
            LoadAdvertisements();
        }

        public void LoadCategories()
        {
            Categories = new ObservableCollection<string>(_db.GetCategories());
        }

        public void LoadOffers()
        {
            Offers = new ObservableCollection<Offer>(_db.GetOffers());
        }

        public void LoadAdvertisements()
        {
            Advertisements = new ObservableCollection<Advertisement>(_db.GetAdvertisements());
        }

        public void LoadCarouselImages()
        {
            var imgs = _db.GetCarouselImages();
            AdminCarouselImages = new ObservableCollection<CarouselImage>(imgs);
            
            var paths = imgs.Select(i => i.ImagePath).ToList();
            if (!paths.Any()) paths.Add("Assets/hero-premium.png");
            
            CarouselImages = new ObservableCollection<string>(paths);
            if (CarouselImages.Any()) CurrentCarouselImage = CarouselImages[0];
        }

        private void StartCarousel()
        {
            _carouselTimer.Interval = TimeSpan.FromSeconds(3);
            _carouselTimer.Tick += (s, e) =>
            {
                if (CarouselImages != null && CarouselImages.Count > 0)
                {
                    _carouselIndex = (_carouselIndex + 1) % CarouselImages.Count;
                    CurrentCarouselImage = CarouselImages[_carouselIndex];
                }
            };
            _carouselTimer.Start();
        }

        [RelayCommand]
        public void Logout()
        {
            CurrentUser = null;
            CartItems.Clear();
            UpdateCartTotal();
            _ = Navigate("Login");
            ShowToast("Logged out successfully.");
        }

        [RelayCommand]
        public void AddProduct()
        {
            if (string.IsNullOrEmpty(NewProduct.Name) || NewProduct.Price <= 0) 
            {
                ShowToast("Please enter valid product details.");
                return;
            }

            if (NewProduct.Id == 0)
                _db.AddProduct(NewProduct);
            else
                _db.UpdateProduct(NewProduct);

            ShowToast(NewProduct.Id == 0 ? "Product Added!" : "Product Updated!");
            NewProduct = new Product();
            LoadProducts();
            LoadCategories();
        }

        [RelayCommand]
        public void EditProduct(Product p)
        {
            NewProduct = new Product
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImagePath = p.ImagePath,
                Category = p.Category
            };
        }

        [RelayCommand]
        public void RemoveProduct(Product p)
        {
            _db.RemoveProduct(p.Id);
            ShowToast("Product Removed!");
            LoadProducts();
            LoadCategories();
        }

        [RelayCommand]
        public void AddOffer()
        {
            if (string.IsNullOrEmpty(NewOffer.Title)) return;
            _db.AddOffer(NewOffer);
            ShowToast("Offer Added!");
            NewOffer = new Offer();
            LoadOffers();
        }

        [RelayCommand]
        public void RemoveOffer(Offer o)
        {
            _db.RemoveOffer(o.Id);
            ShowToast("Offer Removed!");
            LoadOffers();
        }

        [RelayCommand]
        public void AddAdvertisement()
        {
            if (string.IsNullOrEmpty(NewAdvertisement.ImagePath)) return;
            _db.AddAdvertisement(NewAdvertisement);
            ShowToast("Ad Added!");
            NewAdvertisement = new Advertisement();
            LoadAdvertisements();
        }

        [RelayCommand]
        public void RemoveAdvertisement(Advertisement ad)
        {
            _db.RemoveAdvertisement(ad.Id);
            ShowToast("Ad Removed!");
            LoadAdvertisements();
        }

        [RelayCommand]
        public void AddCarouselImage()
        {
            if (string.IsNullOrEmpty(NewCarouselImage.ImagePath)) return;
            _db.AddCarouselImage(NewCarouselImage);
            ShowToast("Banner Added!");
            NewCarouselImage = new CarouselImage();
            LoadCarouselImages();
        }

        [RelayCommand]
        public void RemoveCarouselImage(CarouselImage c)
        {
            _db.RemoveCarouselImage(c.Id);
            ShowToast("Banner Removed!");
            LoadCarouselImages();
        }

        [ObservableProperty]
        private int _adminTabIndex;

        [RelayCommand]
        public void SelectUser(User user)
        {
            AdminSelectedUser = user;
            AdminUserOrders = new ObservableCollection<Order>(_db.GetAllOrders().Where(o => o.UserId == user.Id));
            AdminTabIndex = 3;
        }

        [RelayCommand]
        public void SelectOrder(Order order)
        {
            SelectedOrder = order;
            _ = Navigate("OrderDetails");
        }

        [RelayCommand]
        public void UpdateOrderStatus(Order order)
        {
            if (order != null)
            {
                _db.UpdateOrderStatus(order.Id, order.Status);
                ShowToast($"Order #{order.Id} updated to {order.Status}");
                LoadOrders();
            }
        }

        public void LoadOrders()
        {
            AllOrders = new ObservableCollection<Order>(_db.GetAllOrders());
        }

        [RelayCommand]
        public async System.Threading.Tasks.Task Navigate(string page)
        {
            // Payment Logic: If going to payment from cart but COD is selected, skip to checkout
            if (CurrentPage == "Cart" && page == "Payment")
            {
                // This logic is better handled by a dedicated command, but we can do it here too
                // We'll see how the UI calls this.
            }

            IsLoading = true;
            await System.Threading.Tasks.Task.Delay(500);

            CurrentPage = page;
            if (page == "Dashboard") { LoadProducts(); LoadAdvertisements(); }
            if (page == "Offers") LoadOffers();
            if (page == "Profile" && CurrentUser != null)
            {
                UserOrders = new ObservableCollection<Order>(_db.GetAllOrders().Where(o => o.UserId == CurrentUser.Id));
            }
            if (page == "Admin" && CurrentUser?.IsAdmin == true)
            {
                LoadOrders();
                AllUsers = new ObservableCollection<User>(_db.GetAllUsers());
                LoadProducts();
                LoadOffers();
                LoadCarouselImages();
                LoadAdvertisements();
            }

            IsLoading = false;
        }

        public async void LoadProducts()
        {
            IsLoading = true;
            await System.Threading.Tasks.Task.Delay(1000);

            var list = _db.GetProducts(CurrentUser?.Id ?? 0);
            Products = new ObservableCollection<Product>(list);
            
            LoadCategories(); 
            ApplyFilter();
            
            if (CurrentUser != null)
            {
                UserOrders = new ObservableCollection<Order>(_db.GetAllOrders().Where(o => o.UserId == CurrentUser.Id).ToList());
            }
            IsLoading = false;
        }

        partial void OnSelectedCategoryChanged(string value) => ApplyFilter();

        partial void OnSearchTextChanged(string value) => ApplyFilter();

        private void ApplyFilter()
        {
            var filtered = Products.AsEnumerable();

            if (SelectedCategory != "All")
            {
                filtered = filtered.Where(p => p.Category == SelectedCategory);
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(p => 
                    p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                    p.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            FilteredProducts = new ObservableCollection<Product>(filtered);
        }

        public async void ShowToast(string message)
        {
            ToastMessage = message;
            IsToastVisible = true;
            await System.Threading.Tasks.Task.Delay(4000);
            IsToastVisible = false;
        }

        [RelayCommand]
        public void SetCategory(string category)
        {
            SelectedCategory = category;
        }

        [RelayCommand]
        public void Login(object parameter)
        {
            var values = (object[])parameter;
            string email = (string)values[0];
            string password = (string)values[1];

            var user = _db.Login(email, password);
            if (user != null)
            {
                CurrentUser = user;
                if (user.IsAdmin)
                    _ = Navigate("Admin");
                else
                    _ = Navigate("Dashboard");
                
                ShowToast($"Welcome to D-Mart, {user.Name}!");
            }
            else
            {
                ShowToast("Invalid email or password.");
            }
        }

        [RelayCommand]
        public void Register(object parameter)
        {
            var values = (object[])parameter;
            var user = new User
            {
                Name = (string)values[0],
                Email = (string)values[1],
                Password = (string)values[2],
                Phone = (string)values[3],
                Address = (string)values[4]
            };

            if (_db.Register(user))
            {
                ShowToast("Registration successful! Please login.");
                _ = Navigate("Login");
            }
            else
            {
                ShowToast("Registration failed. Email may already exist.");
            }
        }

        [RelayCommand]
        public void ViewDetails(Product product)
        {
            SelectedProduct = product;
            _ = Navigate("Details");
        }

        [RelayCommand]
        public void AddToCart(Product product)
        {
            CartItems.Add(product);
            UpdateCartTotal();
            ShowToast($"{product.Name} added to cart!");
        }

        [RelayCommand]
        public void RemoveFromCart(Product product)
        {
            CartItems.Remove(product);
            UpdateCartTotal();
        }

        private void UpdateCartTotal()
        {
            CartTotal = CartItems.Sum(i => i.Price);
        }

        [RelayCommand]
        public void Checkout(object parameter)
        {
            if (CurrentUser == null || !CartItems.Any()) return;
            var values = (object[])parameter;
            
            string addr = (string)values[0];
            string phone = (string)values[1];
            string method = (string)values[2];

            if (string.IsNullOrWhiteSpace(addr) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(CurrentUser.Email))
            {
                ShowToast("Email, Phone number, and Address are required to place an order.");
                return;
            }

            // Update user profile if changed
            if (addr != CurrentUser.Address || phone != CurrentUser.Phone)
            {
                CurrentUser.Address = addr;
                CurrentUser.Phone = phone;
                _db.UpdateUser(CurrentUser);
            }

            if (method.Contains("Pay Online"))
            {
                _ = Navigate("Payment");
                return;
            }

            // For COD, place order immediately
            PlaceOrder(method, addr, phone);
        }

        [RelayCommand]
        public void CompletePayment()
        {
            if (CurrentUser == null || !CartItems.Any()) return;
            PlaceOrder("Online", CurrentUser.Address, CurrentUser.Phone);
        }

        private void PlaceOrder(string method, string addr, string phone)
        {
            var order = new Order
            {
                UserId = CurrentUser!.Id,
                UserName = CurrentUser.Name,
                TotalAmount = CartTotal,
                Address = addr,
                Phone = phone,
                PaymentMethod = method,
                Items = CartItems.Select(i => new OrderItem { ProductId = i.Id, ProductName = i.Name, Quantity = 1, Price = i.Price }).ToList()
            };

            _db.PlaceOrder(order);
            CartItems.Clear();
            UpdateCartTotal();
            ShowToast("Order placed successfully!");
            _ = Navigate("Dashboard");
        }

        [RelayCommand]
        public void UpdateProfile(object parameter)
        {
            if (CurrentUser == null) return;
            var values = (object[])parameter;
            CurrentUser.Name = (string)values[0];
            CurrentUser.Phone = (string)values[1];
            CurrentUser.Address = (string)values[2];
            _db.UpdateUser(CurrentUser);
            ShowToast("Profile updated successfully!");
        }
    }
}
