using API;
using Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sentry;
using Share.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry(setup =>
{
    setup.Dsn = builder.Configuration["Logging:Sentry:Dsn"];

    setup.TracesSampleRate = 1.0;

    setup.AddExceptionFilterForType<BadRequest400Exception>();
    setup.AddExceptionFilterForType<Forbidden403Exception>();
    setup.AddExceptionFilterForType<NotFound404Exception>();
    setup.AddExceptionFilterForType<Unauthorized401Exception>();
    setup.AddExceptionFilterForType<OperationCanceledException>();
    setup.AddExceptionFilterForType<TaskCanceledException>();
});

builder.Services.AddControllers();
builder.Services.AutoRegisterServices();
builder.Services.AutoRegisterMediatR();


builder.Services.AddSwaggerGen(option =>
{
    option.CustomSchemaIds(type => type.ToString());
    option.DescribeAllParametersInCamelCase();
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Demo API", Version = "v1"
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseExceptionHandler(ErrorHandlerHelpers.ConfigureExceptionHandler);
app.UseAuthorization();

app.MapControllers();

// to create db for the first time or update db with new migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<CurrencyDb>();
    await context?.Database.MigrateAsync()!;
}

app.Run();