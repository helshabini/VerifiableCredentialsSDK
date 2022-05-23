using Microsoft.AspNetCore.Server.Kestrel.Core;
using VerifiableCredentials.Web;

var builder = WebApplication.CreateBuilder(args);

// Add Verifiable Credentials
builder.Services.AddVerifiableCredentials()
    .WithIssuanceRequests(config: builder.Configuration.GetSection("VCIssuanceOptions"));

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", policy =>
{
    policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    // .WithOrigins("http://example.com","http://www.contoso.com");
}));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);//You can set Time   
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = _ => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();  // use iHttpFactory as best practice, should be easy to use extra retry and hold off policies in the future

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseVerifiableCredentials();//.OnCallBackReceivied(() =>
//{
    //Put an item into a stack or cache and it should automatically reflect in the UI.
//});

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("MyPolicy");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

app.Run();