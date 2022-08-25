using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mongo.Services.Identity;
using Mongo.Services.Identity.DbContexts;
using Mongo.Services.Identity.Models;
using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Reflection;
using Mongo.Services.Identity.Initializer;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.AspNetIdentity;
using Mongo.Services.Identity.Services;

var builder = WebApplication.CreateBuilder(args);

var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContext");

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    // see https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/
                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(SD.IdentityResources)
                .AddInMemoryApiScopes(SD.ApiScopes)
                .AddInMemoryClients(SD.Clients)
                .AddAspNetIdentity<ApplicationUser>();
                // this adds the config data from DB (clients, resources, CORS)
                //.AddConfigurationStore(options =>
                //{
                //    options.ConfigureDbContext = b =>
                //        b.UseSqlServer(connectionString, dbOpts => dbOpts.MigrationsAssembly(typeof(Program).Assembly.FullName));
                //})
                //// this is something you will want in production to reduce load on and requests to the DB//.AddConfigurationStoreCache()//// this adds the operational data from DB (codes, tokens, consents)
                //.AddOperationalStore(options =>
                //{
                //    options.ConfigureDbContext = b =>
                //        b.UseSqlServer(connectionString, dbOpts => dbOpts.MigrationsAssembly(typeof(Program).Assembly.FullName));

                //    // this enables automatic token cleanup. this is optional.
                //    options.EnableTokenCleanup = true;
                //    options.RemoveConsumedTokens = true;
                //});

builder.Services.AddAuthentication();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

//builder.Services.AddAuthorization(options =>
//    options.AddPolicy("admin",
//        policy => policy.RequireClaim("sub", "1"))
//);

//builder.Services.Configure<RazorPagesOptions>(options =>
//    options.Conventions.AuthorizeFolder("/Admin", "admin"));

//builder.Services.AddTransient<ClientRepository>();
//builder.Services.AddTransient<IdentityScopeRepository>();
//builder.Services.AddTransient<ApiScopeRepository>();



//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
//    builder.Configuration.GetConnectionString("ApplicationDbContext")
//    ));


//builder.Services.AddIdentityServer(options =>
//{
//    options.Events.RaiseErrorEvents = true;
//    options.Events.RaiseInformationEvents = true;
//    options.Events.RaiseFailureEvents = true;
//    options.Events.RaiseSuccessEvents = true;
//    options.EmitStaticAudienceClaim = true;
//}).AddConfigurationStore(options =>
//{
//    options.ConfigureDbContext = b => b.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContext"),
//        sql => sql.MigrationsAssembly(migrationsAssembly));
//})
//    .AddInMemoryIdentityResources(SD.IdentityResources)
//           .AddInMemoryApiScopes(SD.ApiScopes)
//           .AddInMemoryClients(SD.Clients)
//           .AddAspNetIdentity<ApplicationUser>().AddDeveloperSigningCredential();


//builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


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
app.UseIdentityServer();
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    // use dbInitializer
    dbInitializer.Initialize();
}
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();

app.Run();
