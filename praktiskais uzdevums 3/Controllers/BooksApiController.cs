using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using praktiskais_uzdevums_3.Models;

namespace praktiskais_uzdevums_3.Controllers;

[ApiController]
[Route("api/books")]
public class BooksApiController : ControllerBase
{
    private readonly LibraryDbContext _db;

    public BooksApiController(LibraryDbContext db)
    {
        _db = db;
    }

    [HttpPost("by-author")]
    public IActionResult ByAuthor([FromBody] TextRequest request)
    {
        var result = _db.Books
            .Where(b => b.Author.Name.Contains(request.Text))
            .Select(b => new BookDto(b.Id, b.Title, b.Year, b.Author.Name, b.Genre.Title))
            .ToList();
        return Ok(result);
    }

    [HttpPost("by-year")]
    public IActionResult ByYear([FromBody] YearRequest request)
    {
        var result = _db.Books
            .Where(b => b.Year == request.Year)
            .OrderBy(b => b.Year)
            .Select(b => new BookDto(b.Id, b.Title, b.Year, b.Author.Name, b.Genre.Title))
            .ToList();
        return Ok(result);
    }

    [HttpPost("by-genre")]
    public IActionResult ByGenre([FromBody] TextRequest request)
    {
        var result = _db.Books
            .Where(b => b.Genre.Title.Contains(request.Text))
            .Select(b => new BookDto(b.Id, b.Title, b.Year, b.Author.Name, b.Genre.Title))
            .ToList();
        return Ok(result);
    }

    [HttpGet("sorted-by-year")]
    public IActionResult SortedByYear()
    {
        var result = _db.Books
            .OrderBy(b => b.Year)
            .Select(b => new BookDto(b.Id, b.Title, b.Year, b.Author.Name, b.Genre.Title))
            .ToList();
        return Ok(result);
    }

    [HttpGet("author-counts")]
    public IActionResult AuthorCounts()
    {
        var result = _db.Authors
            .OrderByDescending(a => a.Books.Count())
            .Select(a => new AuthorCountDto(a.Name, a.Books.Count()))
            .ToList();
        return Ok(result);
    }

    [HttpGet("oldest-newest")]
    public IActionResult OldestNewest()
    {
        var oldest = _db.Books
            .OrderBy(b => b.Year)
            .Select(b => new BookDto(b.Id, b.Title, b.Year, b.Author.Name, b.Genre.Title))
            .FirstOrDefault();
        var newest = _db.Books
            .OrderByDescending(b => b.Year)
            .Select(b => new BookDto(b.Id, b.Title, b.Year, b.Author.Name, b.Genre.Title))
            .FirstOrDefault();

        if (oldest == null || newest == null)
            return NotFound("Nav grāmatu.");

        return Ok(new OldestNewestDto(oldest, newest));
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var book = _db.Books.Find(id);
        if (book == null)
            return NotFound($"Grāmata ar Id {id} nav atrasta.");

        _db.Books.Remove(book);
        _db.SaveChanges();
        return Ok($"Grāmata ar Id {id} dzēsta.");
    }
}

public record TextRequest(string Text);

public record YearRequest(int Year);

public record BookDto(int Id, string Title, int Year, string Author, string Genre);

public record AuthorCountDto(string Name, int Count);

public record OldestNewestDto(BookDto Oldest, BookDto Newest);