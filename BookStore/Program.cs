using BookStore.Extensions;
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
builder.AddMongo();

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
    root.MapPut("{id}", Update);
    root.MapDelete("{id}", Delete);
});

app.Run();

// Endpoints
async Task<IResult> GetAll(IMongoCollection<Book> books)
{
    var filter = Builders<Book>.Filter.Empty;

    var latest = await books
        .Find(filter)
        .SortByDescending(x => x.CreatedAt)
        .Limit(10)
        .ToListAsync();

    var totalCount = await books.CountDocumentsAsync(filter);

    return Results.Ok(new
    {
        results = latest ?? new(),
        total = totalCount
    });
}

async Task<IResult> GetOne(IMongoCollection<Book> books, string id)
{
    if (!ObjectId.TryParse(id, out _))
        return Results.BadRequest("Invalid object _id was received");

    var item = await books.Find(p => p.Id == id).FirstOrDefaultAsync();

    return item is { }
        ? Results.Ok(item)
        : Results.NotFound(string.Format("{0} with Id \"{1}\" not found", nameof(Book), id));
}

async Task<IResult> Create(IMongoCollection<Book> books, BookDto book)
{
    var newItem = new Book
    {
        Name = book.Name,
        Pages = book.Pages
    };
    await books.InsertOneAsync(newItem);
    return Results.Created(newItem.Id, newItem);
}

async Task<IResult> Update(IMongoCollection<Book> books, BookDto book, string? id)
{
    if (!ObjectId.TryParse(id, out _))
        return Results.BadRequest("Invalid object _id was received");

    var update = Builders<Book>.Update
        .Set(x => x.Name, book.Name)
        .Set(x => x.Pages, book.Pages);

    var filter = Builders<Book>.Filter.Eq(s => s.Id, id);

    var options = new FindOneAndUpdateOptions<Book>
    {
        ReturnDocument = ReturnDocument.After
    };

    var item = await books.FindOneAndUpdateAsync(filter, update, options);

    if (item is null)
        return Results.NotFound(string.Format("{0} with Id \"{1}\" not found", nameof(Book), id));

    return Results.Ok(item);
}

async Task<IResult> Delete(IMongoCollection<Book> books, string? id)
{
    if (!ObjectId.TryParse(id, out _))
        return Results.BadRequest("Invalid object _id was received");

    var item = await books.Find(p => p.Id == id).FirstOrDefaultAsync();

    if (item is null)
        return Results.NotFound(string.Format("{0} with Id \"{1}\" not found", nameof(Book), id));

    var filter = Builders<Book>.Filter.Eq(s => s.Id, id);

    await books.DeleteOneAsync(filter);
    return Results.Ok(item);
}