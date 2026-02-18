using DocumentToPdfConverter;
using DocumentToPdfConverter.LibreOffice;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDocumentToPdfConverter(opt =>
{
    opt.ProcessTimeout = TimeSpan.FromSeconds(120);
    opt.MaxConcurrency = 2;
});
builder.Services.AddHealthChecks();
var app = builder.Build();

app.MapHealthChecks("/health");
app.MapPost("/convert", async (IFormFile file, IDocumentConverter converter, CancellationToken ct) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest("No file uploaded.");
    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
    var type = ext switch
    {
        ".docx" => DocumentType.Docx,
        ".doc" => DocumentType.Doc,
        ".xlsx" => DocumentType.Xlsx,
        ".xls" => DocumentType.Xls,
        ".odt" => DocumentType.Odt,
        ".ods" => DocumentType.Ods,
        ".pptx" => DocumentType.Pptx,
        ".ppt" => DocumentType.Ppt,
        _ => (DocumentType?)null
    };
    if (type == null)
        return Results.BadRequest("Unsupported format.");
    await using var input = file.OpenReadStream();
    using var output = new MemoryStream();
    var result = await converter.ConvertAsync(input, output, type.Value, null, ct);
    if (!result.Success)
        return Results.BadRequest(result.ErrorMessage);
    output.Position = 0;
    var name = Path.GetFileNameWithoutExtension(file.FileName) + ".pdf";
    return Results.File(output, "application/pdf", name);
});

app.Run();
