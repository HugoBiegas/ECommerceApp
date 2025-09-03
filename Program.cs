using ECommerceApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuration des sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // Session expires after 2 hours of inactivity
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Make the session cookie essential
    options.Cookie.Name = "BookStore.Session";
});

// Enregistrement des services personnalisés
builder.Services.AddHttpContextAccessor();

// Services d'authentification et utilisateurs
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Services métier
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ILibrarianRequestService, LibrarianRequestService>();

// Service de données
builder.Services.AddScoped<IDataService, DataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ajouter le support des sessions AVANT UseAuthorization
app.UseSession();

// Middleware personnalisé pour initialiser les données au démarrage
app.Use(async (context, next) =>
{
    var dataService = context.RequestServices.GetRequiredService<IDataService>();
    if (!await dataService.IsDataInitializedAsync())
    {
        await dataService.InitializeDataAsync();
    }
    await next();
});

app.UseAuthorization();

// Routes personnalisées pour les différents contrôleurs
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Index}",
    defaults: new { controller = "Admin" });

app.MapControllerRoute(
    name: "account",
    pattern: "Account/{action=Login}",
    defaults: new { controller = "Account" });

app.MapControllerRoute(
    name: "books",
    pattern: "Books/{action=Index}/{id?}",
    defaults: new { controller = "Books" });

app.MapControllerRoute(
    name: "cart",
    pattern: "Cart/{action=Index}",
    defaults: new { controller = "Cart" });

app.MapControllerRoute(
    name: "orders",
    pattern: "Orders/{action=Index}",
    defaults: new { controller = "Orders" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();