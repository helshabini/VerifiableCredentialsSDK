using VerifiableCredentials.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Verifiable Credentials
builder.Services.AddVerifiableCredentials()
    .WithIssuanceRequests(config: builder.Configuration.GetSection("VCIssuanceOptions"));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();