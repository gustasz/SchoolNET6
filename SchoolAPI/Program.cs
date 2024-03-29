using Microsoft.EntityFrameworkCore;
using SchoolAPI;
using SchoolAPI.Data;
using SchoolAPI.Data.Interfaces;
using SchoolAPI.Data.Repositories;
using SchoolAPI.Profiles;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.

builder.Services.AddDbContext<SchoolContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
builder.Services.AddControllers();
/*builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();*/
builder.Services.Scan(scan =>
    scan.FromCallingAssembly()
        .AddClasses()
        .AsMatchingInterface()
        .WithScopedLifetime());
//builder.Services.AddScoped(typeof(IAsyncGenericRepository<>), typeof(GenericRepository<>));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddAutoMapper(typeof(SubjectProfile), typeof(StudentProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Application Starting Up");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}
