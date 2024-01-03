using BookStore.Models;
using BookStore.Models.Dto;
using Branchy;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var options = builder.Configuration.GetSection(nameof(ApiSettings)).Get<ApiSettings>();

builder.Services.AddScoped(_ => new MongoClient(options.ConnectionString));

builder.Services.AddScoped(svc =>
{
    var client = svc.GetRequiredService<MongoClient>();
    var database = client.GetDatabase(options.DatabaseName);
    return database;
});

// register collections
builder.Services.AddScoped(svc =>
{
    var db = svc.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<Book>(options.BooksCollectionName ?? Book.Collection);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Path("/api/books", root =>
{
    root.MapGet(GetAll);
    root.MapGet("{id}", GetOne);
    root.MapPost(Create);
});

app.Run();

// Endpoints
async Task<IResult> GetAll(IMongoCollection<Book> library)
{
    var filter = Builders<Book>.Filter.Empty;

    var latest = await library
        .Find(filter)
        .SortByDescending(x => x.CreatedAt)
        .Limit(10)
        .ToListAsync();

    var totalCount = await library.CountDocumentsAsync(filter);

    return Results.Ok(new
    {
        results = latest ?? new(),
        total = totalCount
    });
}

async Task<IResult> GetOne(IMongoCollection<Book> book, string id)
{
    if (!ObjectId.TryParse(id, out _))
        return Results.BadRequest("Invalid object _id was received");

    var person = await book.Find(p => p.Id == id).FirstOrDefaultAsync();

    return person is { }
        ? Results.Ok(person)
        : Results.NotFound(string.Format("{0} with Id \"{1}\" not found", nameof(Book), id));
}

async Task<IResult> Create(IMongoCollection<Book> library, BookDto book)
{
    var newItem = new Book
    {
        Name = book.Name,
        Pages = book.Pages
    };
    await library.InsertOneAsync(newItem);
    return Results.Created(newItem.Id, newItem);
}