using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderCreator.Data;
using MassTransit;
using OrderCreator.Sagas;
using OrderCreator.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<OrderCreatorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderCreatorContext") ?? throw new InvalidOperationException("Connection string 'OrderCreatorContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(r =>
        {
            r.AddDbContext<DbContext, OrderCreatorContext>((provider, option) =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'OrderCreatorContext' not found."));
            });
        });
        

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
           h.Username("guest");
           h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Orders}/{action=Index}/{id?}");

app.Run();
