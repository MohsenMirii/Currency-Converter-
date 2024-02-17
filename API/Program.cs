#region

using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using API;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sentry;
using Share.DbContracts;
using Share.Exceptions;

#endregion

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


builder.Services.AddCurrencyServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AutoRegisterServices();
builder.Services.AutoRegisterMediatR();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMediatR(typeof(DbLoggerCategory.Database.Command).GetTypeInfo().Assembly);
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddSwaggerGen(option =>
{
    option.CustomSchemaIds(type => type.ToString());
    option.DescribeAllParametersInCamelCase();
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Demo API", Version = "v1"
    });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header, Description = "Please enter a valid token", Name = "Authorization", Type = SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddMemoryCache();
Extensions.RegisterEdmModel(typeof(CurrencyAbstraction).Assembly);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero, ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = "apiWithAuthBackend", ValidAudience = "apiWithAuthBackend", IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("!SomethingSecret!")
            )
        };
    });

builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

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

app.Run();