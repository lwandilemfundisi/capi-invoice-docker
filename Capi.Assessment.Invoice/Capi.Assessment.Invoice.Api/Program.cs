using Capi.Assessment.Invoice.Api.Extensions;
using Capi.Assessment.Invoice.Api.Mappers.Abstract;
using Capi.Assessment.Invoice.Api.Models.ResponseModels.Abstract;
using Capi.Assessment.Invoice.Domain;
using Capi.Assessment.Invoice.Persistence.Extensions;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.

builder
    .Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddSingleton<IMappingRegistry>(sp => new MappingRegistry(typeof(ResponseModel<>), typeof(Program).Assembly));

builder.Services
    .ConfigureApplicationInfrastructure();

builder.Services
    .ConfigureCapiAssessmentInvoiceDomain()
    .ConfigureCapiAssessmentInvoicePersistence();

builder.Services.AddLogging(l => l.AddConsole());
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Title = "An unexpected error occurred.";
    };
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventCatalogService.Api", Version = "v1" });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
