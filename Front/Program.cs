using Front;
using Front.Services;
using SistemaAcademico.Services;
using SistemaAcademico.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Razor Components con modo servidor interactivo
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Servicios de la Biblioteca Académica (Singleton — datos en memoria compartidos)
builder.Services.AddSingleton<EstudianteService>();
builder.Services.AddSingleton<IEstudianteService>(x => x.GetRequiredService<EstudianteService>());

builder.Services.AddSingleton<DocenteService>();
builder.Services.AddSingleton<IDocenteService>(x => x.GetRequiredService<DocenteService>());

builder.Services.AddSingleton<AsignaturaService>();

builder.Services.AddSingleton<EvaluacionService>();
builder.Services.AddSingleton<IEvaluacionService>(x => x.GetRequiredService<EvaluacionService>());

// Servicios Core - Ejercicio 2
builder.Services.AddSingleton<GestorEventosAcademicos>();
builder.Services.AddSingleton<IGestorEventos>(x => x.GetRequiredService<GestorEventosAcademicos>());

builder.Services.AddSingleton<PermanenciaService>();
builder.Services.AddSingleton<DirectivoService>();
builder.Services.AddSingleton<IAnalizadorEvaluaciones, AnalizadorEvaluacionService>();
builder.Services.AddSingleton<UniversidadService>();

// Servicio Facade — Singleton para mantener datos en memoria
builder.Services.AddSingleton<AcademicoFacadeService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

