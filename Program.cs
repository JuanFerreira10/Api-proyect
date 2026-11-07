var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuración del entorno de desarrollo (Swagger UI)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserManagementAPI v1");
    });
}

app.UseHttpsRedirection();

// ---------------------------------------------------------
// Middleware Personalizado de Logging de Peticiones HTTP
// ---------------------------------------------------------
app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var method = context.Request.Method;
    var path = context.Request.Path;
    
    logger.LogInformation("[REQUEST] Método: {Method} | Ruta: {Path}", method, path);

    await next.Invoke();

    var statusCode = context.Response.StatusCode;
    logger.LogInformation("[RESPONSE] Método: {Method} | Ruta: {Path} | Código Estado: {StatusCode}", method, path, statusCode);
});

app.UseAuthorization();

app.MapControllers();

app.Run();