using Assessment_Backend.Filters;
using Assessment_Backend.Middleware;
using Serilog.Sinks.MSSqlServer;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region Log (Serilog)
        // لاگ درخواست و پاسخ هر HTTP Request در دو جدول مجزای دیتابیس:
        //   RequestLogs  -> پراپرتی LogType=Request
        //   ResponseLogs -> پراپرتی LogType=Response
        // (توسط RequestResponseLoggingMiddleware تولید و در اینجا به دیتابیس نوشته می‌شود)
        var logConnectionString = builder.Configuration.GetConnectionString("AssessmentConnection");

        // نمایش خطاهای خود Sink ها (مثل قطعی دیتابیس) روی stderr تا بی‌صدا گم نشوند
        Serilog.Debugging.SelfLog.Enable(msg => Console.Error.WriteLine($"[SERILOG SELFLOG] {msg}"));

        var requestSinkOptions = new MSSqlServerSinkOptions { TableName = "RequestLogs", AutoCreateSqlTable = true };
        var responseSinkOptions = new MSSqlServerSinkOptions { TableName = "ResponseLogs", AutoCreateSqlTable = true };

        Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .WriteTo.Console()
           .WriteTo.File("data/backend.txt", rollingInterval: RollingInterval.Day)
           // جدول RequestLogs: فقط رخدادهای LogType=Request
           .WriteTo.Logger(lc => lc
               .Filter.ByExcluding(e =>
                   !(e.Properties.TryGetValue("LogType", out var t) && t.ToString() == "\"Request\""))
               .WriteTo.MSSqlServer(
                   connectionString: logConnectionString,
                   sinkOptions: requestSinkOptions,
                   columnOptions: RequestResponseLogColumns.RequestOptions(),
                   restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information))
           // جدول ResponseLogs: فقط رخدادهای LogType=Response
           .WriteTo.Logger(lc => lc
               .Filter.ByExcluding(e =>
                   !(e.Properties.TryGetValue("LogType", out var t) && t.ToString() == "\"Response\""))
               .WriteTo.MSSqlServer(
                   connectionString: logConnectionString,
                   sinkOptions: responseSinkOptions,
                   columnOptions: RequestResponseLogColumns.ResponseOptions(),
                   restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information))
           .CreateLogger();

        builder.Host.UseSerilog();
        #endregion

        #region Authentication
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Authentication:Issuer"],
                    ValidAudience = builder.Configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]
                        ?? throw new InvalidOperationException("Authentication:SecretForKey is not configured.")))
                };
            });
        #endregion

        #region DbContext
        var connectionString = builder.Configuration.GetConnectionString("AssessmentConnection");
        builder.Services.AddDbContext<AssessmentDbContext>(options => options.UseSqlServer(connectionString));
        #endregion

        // اعمال Migration های در انتظار در زمان استارتاپ
        var options = new DbContextOptionsBuilder<AssessmentDbContext>()
            .UseSqlServer(connectionString).Options;
        using (AssessmentDbContext context = new(options))
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }

        #region IOC
        builder.Services.AddScoped<ITokenHelperService, TokenHelper>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IGradeService, GradeService>();
        builder.Services.AddScoped<ICourseService, CourseService>();
        builder.Services.AddScoped<IAssessmentService, AssessmentService>();
        builder.Services.AddScoped<IStatisticsService, StatisticsService>();
        #endregion

        #region Controllers + Filters
        builder.Services.AddControllers(options =>
        {
            // تبدیل خودکار نتیجه سرویس‌ها به HTTP Status واقعی
            options.Filters.Add<ServiceResultFilter>();
            // پاسخ استاندارد برای خطاهای ModelState
            options.Filters.Add<ModelValidationFilter>();
        });
        #endregion

        #region Cors
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(corsbuilder =>
            {
                corsbuilder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
            });
        });
        #endregion

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddEndpointsApiExplorer();

        #region Swagger
        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
            var xmlPath = builder.Environment.IsDevelopment()
                ? Path.Combine(AppContext.BaseDirectory, xmlFile)
                : Path.Combine(Directory.GetCurrentDirectory(), xmlFile);
            if (File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath);

            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Plese insert token",
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        #endregion

        var app = builder.Build();

        #region Middleware Pipeline
        // اولین Middleware: ثبت Request و Response هر درخواست در دو جدول دیتابیس
        app.UseRequestResponseLogging();

        // بعد از آن: هر Exception در کل برنامه فقط اینجا مدیریت می‌شود
        app.UseGlobalExceptionHandling();

        app.UseSerilogRequestLogging();

        app.UseCors();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        #endregion

        app.UseSwagger();
        app.UseSwaggerUI();

        app.Run();
    }
}
