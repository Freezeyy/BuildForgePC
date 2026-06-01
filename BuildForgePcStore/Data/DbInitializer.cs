using BuildForgePcStore.Models;
using BuildForgePcStore.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BuildForgePcStore.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.Migrate();

        if (context.Categories.Any())
            return;

        var hasher = new PasswordHasher<User>();

        var categories = new[]
        {
            "CPU", "GPU", "Motherboard", "RAM", "SSD", "Power Supply", "PC Case"
        };

        foreach (var name in categories)
            context.Categories.Add(new Category { CategoryName = name });

        context.SaveChanges();

        var cat = context.Categories.ToDictionary(c => c.CategoryName, c => c.CategoryId);

        var products = GetSeedProducts(cat);
        context.Products.AddRange(products);

        var admin = new User
        {
            Name = "Store Admin",
            Email = "admin@buildforge.my",
            Role = UserRoles.Admin
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

        var demo = new User
        {
            Name = "Demo Customer",
            Email = "demo@buildforge.my",
            Role = UserRoles.Customer
        };
        demo.PasswordHash = hasher.HashPassword(demo, "Customer@123");

        context.Users.AddRange(admin, demo);
        context.SaveChanges();
    }

    private static List<Product> GetSeedProducts(Dictionary<string, int> cat)
    {
        const string img = "/images/products/placeholder.svg";
        var list = new List<Product>();

        void Add(string category, string name, string desc, decimal price, int stock, string? socket = null)
        {
            list.Add(new Product
            {
                CategoryId = cat[category],
                ProductName = name,
                Description = desc,
                Price = price,
                Stock = stock,
                ImagePath = img,
                Socket = socket
            });
        }

        // CPU (6)
        Add("CPU", "AMD Ryzen 5 5600", "6-core, 12-thread AM4 processor for gaming and productivity.", 599m, 25, "AM4");
        Add("CPU", "AMD Ryzen 7 5800X3D", "8-core AM4 with 3D V-Cache for top gaming performance.", 1299m, 12, "AM4");
        Add("CPU", "AMD Ryzen 7 7800X3D", "8-core AM5 gaming CPU with 3D V-Cache.", 1899m, 15, "AM5");
        Add("CPU", "AMD Ryzen 9 7900X", "12-core AM5 processor for creators and enthusiasts.", 2099m, 10, "AM5");
        Add("CPU", "Intel Core i5-13400", "10-core hybrid CPU for LGA1700 builds.", 849m, 20, "LGA1700");
        Add("CPU", "Intel Core i7-14700K", "20-core unlocked LGA1700 flagship.", 1899m, 8, "LGA1700");

        // GPU (6)
        Add("GPU", "NVIDIA GeForce RTX 4060 8GB", "1080p/1440p entry ray tracing GPU.", 1299m, 18);
        Add("GPU", "NVIDIA GeForce RTX 4060 Ti 8GB", "Step-up 1080p high-refresh card.", 1599m, 14);
        Add("GPU", "NVIDIA GeForce RTX 4070 Super 12GB", "Strong 1440p ray tracing performance.", 2899m, 10);
        Add("GPU", "AMD Radeon RX 7600 8GB", "Efficient 1080p AMD option.", 1199m, 16);
        Add("GPU", "AMD Radeon RX 7800 XT 16GB", "1440p sweet-spot with 16GB VRAM.", 2399m, 9);
        Add("GPU", "NVIDIA GeForce RTX 4080 Super 16GB", "4K-capable high-end GPU.", 5299m, 5);

        // Motherboard (6)
        Add("Motherboard", "MSI B550-A PRO", "AM4 ATX board with PCIe 4.0 and M.2.", 499m, 14, "AM4");
        Add("Motherboard", "ASUS TUF B650-PLUS WIFI", "AM5 DDR5 board with Wi-Fi 6.", 899m, 11, "AM5");
        Add("Motherboard", "Gigabyte B650M DS3H", "Budget AM5 micro-ATX motherboard.", 649m, 15, "AM5");
        Add("Motherboard", "MSI PRO Z790-P WIFI", "Intel LGA1700 DDR5 ATX motherboard.", 1099m, 10, "LGA1700");
        Add("Motherboard", "ASRock B760M Pro RS", "LGA1700 micro-ATX DDR5 board.", 599m, 13, "LGA1700");
        Add("Motherboard", "ASUS ROG STRIX X670E-E", "Premium AM5 E-ATX for enthusiasts.", 1899m, 4, "AM5");

        // RAM (5)
        Add("RAM", "Kingston FURY 16GB DDR4 3200", "2x8GB kit for AM4/LGA1200 builds.", 199m, 40);
        Add("RAM", "Corsair Vengeance 32GB DDR5 6000", "2x16GB EXPO/XMP kit.", 499m, 28);
        Add("RAM", "G.Skill Trident Z5 32GB DDR5 6400", "2x16GB RGB DDR5 kit.", 599m, 22);
        Add("RAM", "TeamGroup T-Force 16GB DDR5 5600", "2x8GB budget DDR5.", 299m, 35);
        Add("RAM", "Crucial 64GB DDR5 5600", "2x32GB kit for workstations.", 899m, 8);

        // SSD (5)
        Add("SSD", "Kingston NV2 500GB NVMe", "PCIe 4.0 budget system drive.", 149m, 50);
        Add("SSD", "Samsung 990 PRO 1TB", "High-end PCIe 4.0 NVMe SSD.", 549m, 25);
        Add("SSD", "WD Black SN850X 2TB", "Fast 2TB gaming SSD.", 899m, 15);
        Add("SSD", "Crucial P3 Plus 1TB", "Value PCIe 4.0 NVMe.", 299m, 30);
        Add("SSD", "Samsung 870 EVO 1TB SATA", "Reliable 2.5\" SATA upgrade.", 349m, 20);

        // Power Supply (5)
        Add("Power Supply", "Cooler Master MWE 650W 80+ Bronze", "Reliable 650W non-modular PSU.", 249m, 30);
        Add("Power Supply", "Corsair RM750e 750W 80+ Gold", "Fully modular ATX 3.0 ready PSU.", 449m, 18);
        Add("Power Supply", "Seasonic Focus GX-850 80+ Gold", "850W modular with 10-year warranty.", 599m, 12);
        Add("Power Supply", "be quiet! Straight Power 11 1000W", "1000W Platinum for high-end GPUs.", 899m, 6);
        Add("Power Supply", "Thermaltake Smart 500W", "Budget 500W for office PCs.", 179m, 25);

        // PC Case (6) — total 39 products
        Add("PC Case", "Montech X1 Mesh", "Compact airflow case with mesh front.", 189m, 22);
        Add("PC Case", "NZXT H5 Flow", "Mid-tower airflow chassis.", 399m, 14);
        Add("PC Case", "Corsair 4000D Airflow", "Popular mid-tower with excellent airflow.", 449m, 16);
        Add("PC Case", "Lian Li O11 Dynamic EVO", "Premium dual-chamber showcase case.", 599m, 10);
        Add("PC Case", "Fractal Design Meshify 2 Compact", "Compact ATX mesh case.", 499m, 11);
        Add("PC Case", "Cooler Master MasterBox Q300L", "micro-ATX budget case.", 159m, 28);

        return list;
    }
}
