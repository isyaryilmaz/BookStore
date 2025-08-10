using BookStoreMvc.Data;
using BookStoreMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsý
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (Kullanýcý ve Roller) ve Identity UI
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// MVC + Session
builder.Services.AddControllersWithViews();

// Session için gerekli servisler
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware’ler
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();  // Session middleware mutlaka routing sonrasý

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Identity UI Razor Pages için gerekli
app.MapRazorPages();

// Admin rolü ve kullanýcý otomatik oluþturma + seed verisi ekleme
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    string[] roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        var roleExists = await roleManager.RoleExistsAsync(role);
        if (!roleExists)
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    string adminEmail = "admin@bookstore.com";
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail
        };
        var createResult = await userManager.CreateAsync(newAdmin, adminPassword);

        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }

    // Seed Kategoriler
    if (!context.Categories.Any())
    {
        var categories = new List<Category>
        {
            new Category { Name = "Bilim" },
            new Category { Name = "Edebiyat" },
            new Category { Name = "Teknoloji" },
            new Category { Name = "Tarih" },
            new Category { Name = "Felsefe" }
        };
        context.Categories.AddRange(categories);
        context.SaveChanges();
    }

    // Kategori Id’lerini al
    var catBilim = context.Categories.FirstOrDefault(c => c.Name == "Bilim")?.Id ?? 0;
    var catEdebiyat = context.Categories.FirstOrDefault(c => c.Name == "Edebiyat")?.Id ?? 0;
    var catTeknoloji = context.Categories.FirstOrDefault(c => c.Name == "Teknoloji")?.Id ?? 0;
    var catTarih = context.Categories.FirstOrDefault(c => c.Name == "Tarih")?.Id ?? 0;
    var catFelsefe = context.Categories.FirstOrDefault(c => c.Name == "Felsefe")?.Id ?? 0;

    // Seed Kitaplar
    if (!context.Books.Any())
    {
        var books = new List<Book>
        {
            new Book { Title = "Kürk Mantolu Madonna", Price = 75.00m, CategoryId = catEdebiyat, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789750701918" },
            new Book { Title = "Saatleri Ayarlama Enstitüsü", Price = 80.00m, CategoryId = catEdebiyat, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789750517530" },
            new Book { Title = "Tutunamayanlar", Price = 90.00m, CategoryId = catEdebiyat, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789750509834" },
            new Book { Title = "Ýnce Memed", Price = 85.00m, CategoryId = catEdebiyat, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789754700056" },
            new Book { Title = "Masumiyet Müzesi", Price = 70.00m, CategoryId = catEdebiyat, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789755106446" },

            new Book { Title = "Türklerin Tarihi", Price = 110.00m, CategoryId = catTarih, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789750502607" },
            new Book { Title = "Osmanlý Tarihi", Price = 100.00m, CategoryId = catTarih, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9786058644497" },
            new Book { Title = "Atatürk: Bir Milletin Yeniden Doðuþu", Price = 95.00m, CategoryId = catTarih, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789751508934" },
            new Book { Title = "Nutuk", Price = 85.00m, CategoryId = catTarih, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789758709601" },
            new Book { Title = "Türkiye'nin Yakýn Tarihi", Price = 90.00m, CategoryId = catTarih, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9786057762857" },

            new Book { Title = "Felsefe Tarihi", Price = 70.00m, CategoryId = catFelsefe, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789755337566" },
            new Book { Title = "Varoluþ ve Özgürlük", Price = 65.00m, CategoryId = catFelsefe, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9786055413347" },
            new Book { Title = "Sufizm ve Tasavvuf", Price = 75.00m, CategoryId = catFelsefe, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9786050823839" },
            new Book { Title = "Ýslam Felsefesi", Price = 80.00m, CategoryId = catFelsefe, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789753553494" },
            new Book { Title = "Düþünce Tarihi", Price = 85.00m, CategoryId = catFelsefe, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789750505042" },

            new Book { Title = "Bilim Tarihi Üzerine Denemeler", Price = 90.00m, CategoryId = catBilim, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789750521531" },
            new Book { Title = "Türkiye'de Bilim ve Teknoloji", Price = 95.00m, CategoryId = catBilim, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9786050901364" },
            new Book { Title = "Bilim Felsefesi", Price = 80.00m, CategoryId = catBilim, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789753053677" },
            new Book { Title = "Teknoloji ve Toplum", Price = 75.00m, CategoryId = catTeknoloji, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789752105822" },
            new Book { Title = "Yapay Zeka", Price = 85.00m, CategoryId = catTeknoloji, ImageUrl = "https://cdn.kitapyurdu.com/v1/getImage/fn:9789750206900" }
        };
        context.Books.AddRange(books);
        context.SaveChanges();
    }
}

app.Run();