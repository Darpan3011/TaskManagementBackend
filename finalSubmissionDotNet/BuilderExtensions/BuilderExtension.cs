using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.ServiceContracts.IUserService;
using finalSubmission.Core.Services.UsersService;
using finalSubmission.Infrastructure.DbContexts;
using finalSubmission.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using ContactsManager.Core.Domain.IdentityEntities;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using finalSubmission.Core.ServiceContracts.ITaskService;
using finalSubmission.Core.Services.TaskService;
using System.Reflection;

namespace finalSubmissionDotNet.BuilderExtensions
{
    public static class BuilderExtension
    {
        public static IServiceCollection AddServiceCollection(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost4200", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Enter 'Bearer JWT_token' ",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
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
                        new string[] { }
                    }
                });
            });


            services.AddDbContext<TaskOrderDbContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]!);
            });

            // Adding Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<TaskOrderDbContext>().AddDefaultTokenProviders();

            // Configure JWT Authentication
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });


            // DI
            //services.AddScoped<IRoleSeeder, RoleSeeder>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGetAllTasks, GetAllTasks>();
            services.AddScoped<IChangeStatus, ChangeStatus>();
            services.AddScoped<ICreateTask, CreateTask>();
            services.AddScoped<IDeleteTask, DeleteTask>();
            services.AddScoped<IEditTask, EditTask>();
            services.AddScoped<IGetTaskByDueDate, GetTaskByDueDate>();
            services.AddScoped<IGetTaskByStatus, GetTaskByStatus>();
            services.AddScoped<IGetTaskByTitle, GetTaskByTitle>();
            services.AddScoped<IDeleteUser, DeleteUser>();
            services.AddScoped<ICreateUser, CreateUser>();
            services.AddScoped<IGetAllUsers, GetAllUsers>();
            services.AddScoped<IGetByUserID, GetByUserID>();
            services.AddScoped<IGetFilteredTasksByUser,  GetFilteredTasksByUser>();
            services.AddScoped<IUserExistsOrNot, UserExistsOrNot>();
            //services.AddScoped<IUserAndTaskSeeder, UserAndTaskSeeder>();

            // for app.UseHttpLogging();
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties |
                    Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });


            return services;
        }
    }
}
