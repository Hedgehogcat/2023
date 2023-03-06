using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using TokenSecurityDemo;
using TokenSecurityDemo.Authorization;
using TokenSecurityDemo.Entities;
using TokenSecurityDemo.Helpers;
using TokenSecurityDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//���jwt��֤��
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,//�Ƿ���֤Issuer
          ValidateAudience = true,//�Ƿ���֤Audience
          ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
          ClockSkew = TimeSpan.FromSeconds(30),
          ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
          ValidAudience = Const.Domain,//Audience
          ValidIssuer = Const.Domain,//Issuer���������ǰ��ǩ��jwt������һ��
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.SecurityKey))//�õ�SecurityKey
          
      };
  });

builder.Services.AddDbContext<DataContext>();
builder.Services.AddCors();


builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// configure DI for application services
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();
// add hardcoded test user to db on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    var testUser = new User
    {
        FirstName = "Test",
        LastName = "User",
        Username = "test",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
    };
    context.Users.Add(testUser);
    context.SaveChanges();
}
// Configure the HTTP request pipeline.
///���jwt��֤
app.UseAuthentication();

// global cors policy
app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
