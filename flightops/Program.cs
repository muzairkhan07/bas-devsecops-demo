// BAS FlightOps - DEMO application (fictional, for DevSecOps demonstration only)
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => Results.Content(
    "<h1>BAS FlightOps (demo)</h1><p>Try <a href='/api/flights/GF001'>/api/flights/GF001</a> or <a href='/health'>/health</a></p>",
    "text/html"));

app.MapGet("/api/flights/{flightNo}", (string flightNo) =>
{
    // INSECURE: user input concatenated straight into SQL -> SQL injection risk (SAST finding)
    var sql = "SELECT FLIGHT_NO, STATUS FROM FLIGHTS WHERE FLIGHT_NO = '" + flightNo + "'";
    using var cmd = new OracleCommand(sql);

    var result = new { flight = flightNo, status = "On time", gate = "A12" };
    return Results.Content(JsonConvert.SerializeObject(result), "application/json");
});

app.MapGet("/health", (IConfiguration config) => Results.Ok(new
{
    status = "healthy",
    // Shows whether the DB secret was delivered at runtime - never prints the secret itself
    databaseSecretLoaded = !string.IsNullOrWhiteSpace(config.GetConnectionString("BasDb"))
}));

app.Run();
