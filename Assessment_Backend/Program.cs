internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        #region Log
        Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .WriteTo.Console()
           .WriteTo.File("data/backend.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();
        #endregion
        builder.Host.UseSerilog();

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
                    Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))

                };
            }
            );


        #endregion

        #region DbContext
        var connectionString = builder.Configuration.GetConnectionString("AssessmentConnection");
        builder.Services.AddDbContext<AssessmentDbContext>(options => options.UseSqlServer(connectionString));
        #endregion
        var options=new DbContextOptionsBuilder<AssessmentDbContext>()
            .UseSqlServer(connectionString).Options;
        using AssessmentDbContext context=new AssessmentDbContext(options);
        
        if (context.Database.GetPendingMigrations().Count()>0)
        {
            context.Database.Migrate();
        }

        #region IOC

        builder.Services.AddScoped<ITokenHelperService, TokenHelper>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddScoped<IGradeService, GradeService>();
        builder.Services.AddTransient<ICourseService, CourseService>();
        builder.Services.AddTransient<IAssessmentService, AssessmentService>();
        builder.Services.AddScoped<IStatisticsService , StatisticsService>();

        #endregion


        builder.Services.AddControllers();

        #region Cors
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
            corsbuilder =>
            {
                corsbuilder.WithOrigins("*")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowAnyOrigin();
            });
        });
        #endregion

        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        #region Confing Swagger
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(Path.Combine(Directory.GetCurrentDirectory(), @"bin\Debug\net8.0", "Assessment_Backend.xml"));

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
                        Reference=new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                             Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
            });
        }
        else
        {
            builder.Services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(Path.Combine(Directory.GetCurrentDirectory(), "Assessment_Backend.xml"));


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
                        Reference=new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                             Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
            });
        }
            
        #endregion
        var app = builder.Build();

        app.UseCors();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        #region Adding Security Middleware

        //app.Use(async (context, next) =>
        //{
        //    if (context.Request.Path.StartsWithSegments("/swagger") && context.Request.Method == "GET")
        //    {
        //        string authHeader = context.Request.Headers["Authorization"];

        //        if (authHeader == null || !authHeader.StartsWith("Basic "))
        //        {
        //            // Password not provided in the request
        //            context.Response.StatusCode = 401;
        //            context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"My API\"";
        //            await context.Response.WriteAsync("To access Swagger, you need to provide a password.");
        //            return;
        //        }
        //        else
        //        {
        //            // Check the password
        //            string credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Substring(6)));
        //            string[] parts = credentials.Split(':');
        //            string username = parts[0];
        //            string password = parts[1];

        //            if (username == "admin" && password == "123456789")
        //            {
        //                // Correct password, allow access to Swagger
        //                await next();
        //                return;
        //            }
        //            else
        //            {
        //                // Incorrect password
        //                context.Response.StatusCode = 401;
        //                await context.Response.WriteAsync("Incorrect password.");
        //                return;
        //            }
        //        }
        //    }

        //    await next();
        //});
        #endregion
        app.UseSwagger();
        app.UseSwaggerUI();
        app.Run();
    }
}