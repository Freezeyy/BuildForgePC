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
        var list = new List<Product>();

        void Add(string category, string name, string desc, decimal price, int stock, string imageFile, string? socket = null)
        {
            list.Add(new Product
            {
                CategoryId = cat[category],
                ProductName = name,
                Description = desc,
                Price = price,
                Stock = stock,
                ImagePath = $"/images/products/{imageFile}",
                Socket = socket
            });
        }

        // CPU (6)
        Add("CPU", "AMD Ryzen 5 5600", "6-core, 12-thread AM4 processor for gaming and productivity.", 599m, 25, "dfedbd6f50cb4bdabdfd24bf47fbd14b.jpg", "AM4");
        Add("CPU", "AMD Ryzen 7 5800X3D", "8-core AM4 with 3D V-Cache for top gaming performance.", 1299m, 12, "a2864e44e13644038bb9a5e3bf5facd5.jpg", "AM4");
        Add("CPU", "AMD Ryzen 7 7800X3D", "8-core AM5 gaming CPU with 3D V-Cache.", 1899m, 15, "91be2a85fc094b60a45e158d443cce6a.jpg", "AM5");
        Add("CPU", "AMD Ryzen 9 7900X", "12-core AM5 processor for creators and enthusiasts.", 2099m, 10, "94c3a797d996471f99410f3b4f3b3851.jpg", "AM5");
        Add("CPU", "Intel Core i5-13400", "10-core hybrid CPU for LGA1700 builds.", 849m, 20, "28c662da05f841989f1ef6ef35a89157.jpg", "LGA1700");
        Add("CPU", "Intel Core i7-14700K", "20-core unlocked LGA1700 flagship.", 1899m, 8, "cd3cbd660fdc4e0696e4e0534f0f1b14.jpg", "LGA1700");

        // GPU (6)
        Add("GPU", "NVIDIA GeForce RTX 4060 8GB", "1080p/1440p entry ray tracing GPU.", 1299m, 18, "bf1dbd385cd749e1968bdfc1c1664c0d.jpg");
        Add("GPU", "NVIDIA GeForce RTX 4060 Ti 8GB", "Step-up 1080p high-refresh card.", 1599m, 14, "1e1ad2bb19a24057aa1d6304f0d66532.jpg");
        Add("GPU", "NVIDIA GeForce RTX 4070 Super 12GB", "Strong 1440p ray tracing performance.", 2899m, 10, "4b741cd7061d4f6b84015e7a5a5e4bed.jpg");
        Add("GPU", "AMD Radeon RX 7600 8GB", "Efficient 1080p AMD option.", 1199m, 16, "862fc4beec294b3585dcd2740c545b41.jpg");
        Add("GPU", "AMD Radeon RX 7800 XT 16GB", "1440p sweet-spot with 16GB VRAM.", 2399m, 9, "4e923242401341df8f36706ee7ef5978.jpg");
        Add("GPU", "NVIDIA GeForce RTX 4080 Super 16GB", "4K-capable high-end GPU.", 5299m, 5, "c0facaf30afe498f86018e221ad1ec66.jpg");

        // Motherboard (6)
        Add("Motherboard", "MSI B550-A PRO", "AM4 ATX board with PCIe 4.0 and M.2.", 499m, 14, "6ff922c491f844d0a5cd359dd50082a2.png", "AM4");
        Add("Motherboard", "ASUS TUF B650-PLUS WIFI", "AM5 DDR5 board with Wi-Fi 6.", 899m, 11, "e1ff5ef6dea941c999981ece980d8eaf.jpg", "AM5");
        Add("Motherboard", "Gigabyte B650M DS3H", "Budget AM5 micro-ATX motherboard.", 649m, 15, "486c5e6775c3452c874c9cd848760cf6.jpg", "AM5");
        Add("Motherboard", "MSI PRO Z790-P WIFI", "Intel LGA1700 DDR5 ATX motherboard.", 1099m, 10, "ce3be65f0fc04d9083660666d1100e22.png", "LGA1700");
        Add("Motherboard", "ASRock B760M Pro RS", "LGA1700 micro-ATX DDR5 board.", 599m, 13, "89d6a3b55def4448816e2b7c0c547481.jpg", "LGA1700");
        Add("Motherboard", "ASUS ROG STRIX X670E-E", "Premium AM5 E-ATX for enthusiasts.", 1899m, 4, "44dfbddb3f1f4062a398f5ad0e76264b.jpg", "AM5");

        // RAM (5)
        Add("RAM", "Kingston FURY 16GB DDR4 3200", "2x8GB kit for AM4/LGA1200 builds.", 199m, 40, "913e2f9770494c1ab2fc0e92f22f8295.jpg");
        Add("RAM", "Corsair Vengeance 32GB DDR5 6000", "2x16GB EXPO/XMP kit.", 499m, 28, "3e890e45a5af4c0f86c8bcf102d3735a.jpg");
        Add("RAM", "G.Skill Trident Z5 32GB DDR5 6400", "2x16GB RGB DDR5 kit.", 599m, 22, "258b5ef4ce9d4f5c9a1274007488ce3e.png");
        Add("RAM", "TeamGroup T-Force 16GB DDR5 5600", "2x8GB budget DDR5.", 299m, 35, "f681b73d84ff41ab904a789dfc7820b5.jpg");
        Add("RAM", "Crucial 64GB DDR5 5600", "2x32GB kit for workstations.", 899m, 8, "02a2c42edf2049739e05a0804c1b098c.jpg");

        // SSD (5)
        Add("SSD", "Kingston NV2 500GB NVMe", "PCIe 4.0 budget system drive.", 149m, 50, "b0776a50dc32480ea880f4f8bdf7ab63.jpg");
        Add("SSD", "Samsung 990 PRO 1TB", "High-end PCIe 4.0 NVMe SSD.", 549m, 25, "283bf595e2bd4f208b5d5d73cb4d24d1.jpg");
        Add("SSD", "WD Black SN850X 2TB", "Fast 2TB gaming SSD.", 899m, 15, "53720ecf0e7f4daebfa446450cc407b8.jpg");
        Add("SSD", "Crucial P3 Plus 1TB", "Value PCIe 4.0 NVMe.", 299m, 30, "02a610db9cf844e1a01fadd936609952.jpg");
        Add("SSD", "Samsung 870 EVO 1TB SATA", "Reliable 2.5\" SATA upgrade.", 349m, 20, "c805da303d4248a6af50dff0e9b4024d.jpg");

        // Power Supply (5)
        Add("Power Supply", "Cooler Master MWE 650W 80+ Bronze", "Reliable 650W non-modular PSU.", 249m, 30, "296cc376a3544f58a7c90363db439502.jpg");
        Add("Power Supply", "Corsair RM750e 750W 80+ Gold", "Fully modular ATX 3.0 ready PSU.", 449m, 18, "399ca149c26a4bc4afe07799d418ec14.jpg");
        Add("Power Supply", "Seasonic Focus GX-850 80+ Gold", "850W modular with 10-year warranty.", 599m, 12, "e87b9ea0ad5c4e82826c59fe04b89164.jpg");
        Add("Power Supply", "be quiet! Straight Power 11 1000W", "1000W Platinum for high-end GPUs.", 899m, 6, "75f67e756bbf497c98fb82a6fa322bab.jpg");
        Add("Power Supply", "Thermaltake Smart 500W", "Budget 500W for office PCs.", 179m, 25, "67af070e5d01437e9a71791d27891a86.jpg");

        // PC Case (6) — total 39 products
        Add("PC Case", "Montech X1 Mesh", "Compact airflow case with mesh front.", 189m, 22, "d8dbea31cf2a4222a1674a9fb433006a.jpg");
        Add("PC Case", "NZXT H5 Flow", "Mid-tower airflow chassis.", 399m, 14, "7ff16d8cf902440796470eff6e5eeea1.jpg");
        Add("PC Case", "Corsair 4000D Airflow", "Popular mid-tower with excellent airflow.", 449m, 16, "be8fed6c3e2c41ef9bd8cad546518dcf.jpg");
        Add("PC Case", "Lian Li O11 Dynamic EVO", "Premium dual-chamber showcase case.", 599m, 10, "2d8d25afe026492ca2f97d7a43ec65df.jpg");
        Add("PC Case", "Fractal Design Meshify 2 Compact", "Compact ATX mesh case.", 499m, 11, "96604491d4764b0d91fc04dbeea593f2.jpg");
        Add("PC Case", "Cooler Master MasterBox Q300L", "micro-ATX budget case.", 159m, 28, "8b027c43efca4531a03275d3aac10a8b.jpg");

        return list;
    }
}
