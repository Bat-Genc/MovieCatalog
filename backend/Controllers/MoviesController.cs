using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MoviesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies()
    {
        return await _context.Movies
            .Include(m => m.Actors)
            .Select(m => new MovieDto(
                m.Id, m.Title, m.ReleaseYear,
                m.Actors.Select(a => new ActorDto(a.Id, a.Name, a.Bio)).ToList()))
            .ToListAsync();
    }

    [HttpGet("actors")]
    public async Task<ActionResult<IEnumerable<ActorDto>>> GetActors()
    {
        return await _context.Actors
            .Select(a => new ActorDto(a.Id, a.Name, a.Bio))
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<MovieDto>> CreateMovie(CreateMovieDto dto)
    {
        var movie = new Movie { Title = dto.Title, ReleaseYear = dto.ReleaseYear };

        if (dto.ActorIds?.Any() == true)
            movie.Actors = await _context.Actors
                .Where(a => dto.ActorIds.Contains(a.Id)).ToListAsync();

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        var result = new MovieDto(movie.Id, movie.Title, movie.ReleaseYear,
            movie.Actors.Select(a => new ActorDto(a.Id, a.Name, a.Bio)).ToList());

        return CreatedAtAction(nameof(GetMovies), null, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMovie(int id, CreateMovieDto dto)
    {
        var movie = await _context.Movies.Include(m => m.Actors)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null) return NotFound();

        movie.Title = dto.Title;
        movie.ReleaseYear = dto.ReleaseYear;

        if (dto.ActorIds != null)
            movie.Actors = await _context.Actors
                .Where(a => dto.ActorIds.Contains(a.Id)).ToListAsync();

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return NotFound();

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
