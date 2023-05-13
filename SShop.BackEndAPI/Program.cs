using SShop.Domain.EF;
using SShop.Domain.Entities;
using FluentValidation;
using SShop.Repositories.Catalog.Categories;
using SShop.Repositories.Catalog.Discounts;
using SShop.Repositories.Catalog.OrderItems;
using SShop.Repositories.Catalog.Orders;
using SShop.Repositories.Catalog.ProductImages;
using SShop.Repositories.Catalog.Products;
using SShop.Repositories.Catalog.ReviewItems;
using SShop.Repositories.Catalog.WishItems;
using SShop.Repositories.System.Roles;
using SShop.Repositories.System.Users;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SShop.Repositories.Catalog.Brands;

using SShop.Repositories.Catalog.CartItems;
using SShop.Services.FileStorage;
using SShop.Services.MailJet;
using SShop.ViewModels.System.Users;
using SShop.Utilities.Constants.Systems;
using SShop.BackEndAPI.Middlewares;
using SShop.Repositories.System.Addresses;
using SShop.Repositories.Catalog.OrderState;
using SShop.Repositories.Catalog.PaymentMethod;
using SShop.Repositories.Catalog.DeliveryMethod;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
// Add services to the container.
services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("AppDbContext")));

services.AddIdentity<AppUser, IdentityRole>(opts =>
{
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequiredLength = 5;
    opts.Password.RequireDigit = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<IProductImageRepository, ProductImageRepository>();
services.AddScoped<ICategoryRepository, CategoryRepository>();
services.AddScoped<IBrandRepository, BrandRepository>();
services.AddScoped<IDiscountRepository, DiscountRepository>();
services.AddScoped<IReviewItemRepository, ReviewItemRepository>();
services.AddScoped<IOrderRepository, OrderRepository>();
services.AddScoped<IOrderItemRepository, OrderItemRepository>();
services.AddScoped<ICartItemRepository, CartItemRepository>();
services.AddScoped<IWishItemRepository, WishItemRepository>();
services.AddScoped<IAddressRepository, AddressRepository>();
services.AddScoped<IOrderStateRepository, OrderStateRepository>();
services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
services.AddScoped<IDeliveryMethodRepository, DeliveryMethodRepository>();

services.AddScoped<IFileStorageService, FileStorageService>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IRoleRepository, RoleRepository>();
services.AddScoped<IMailJetServices, MailJetServices>();

services.AddHttpContextAccessor();
services.AddHttpClient();
services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

services.AddSwaggerGen(s =>
{
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = @"JWT authorization header using the Bearer sheme. \r\n\r\n
                        Enter 'Bearer' [space] and then your token in the text input below.
                        \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    s.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
});
string issuer = configuration.GetValue<string>("Tokens:Issuer");
string signingKey = configuration.GetValue<string>("Tokens:Key");
byte[] signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);
services
    .AddAuthentication(opts =>
    {
        opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opts =>
    {
        opts.RequireHttpsMetadata = false;
        opts.SaveToken = true;
        opts.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = issuer,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
        };
    }).AddCookie(options =>
    {
        options.LoginPath = "/admin/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
    })
    .AddGoogle(opts =>
    {
        IConfigurationSection googleAuthNSection = configuration.GetSection("Authentication:Google");
        opts.ClientId = googleAuthNSection["ClientId"];
        opts.ClientSecret = googleAuthNSection["ClientSecret"];
        opts.SignInScheme = IdentityConstants.ExternalScheme;
    });
services.AddAuthorization();
services.AddSession();
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

IServiceProvider serviceProvider = services.BuildServiceProvider();
async Task CreateRoles(IServiceProvider serviceProvider)
{
    var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var roleName in SystemConstants.UserRoles.Roles)
    {
        var roleExist = await RoleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await RoleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
await CreateRoles(serviceProvider);

var app = builder.Build();
var env = app.Environment;
// Configure the HTTP request pipeline.
if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger(c =>
{
    c.SerializeAsV2 = true;
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API for FurnitureWebApp V1");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();