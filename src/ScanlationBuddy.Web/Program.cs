var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
	   .AddCardboardHttp()
	   .AddDatabase()
	   .AddCore()
	   .AddOAuth(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/error");
}
else
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors(c =>
{
	c.AllowAnyHeader()
	 .AllowAnyMethod()
	 .AllowAnyOrigin()
	 .WithExposedHeaders("Content-Disposition");
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(c =>
{
	c.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
	c.MapFallbackToFile("/index.html");
});
app.Run();
