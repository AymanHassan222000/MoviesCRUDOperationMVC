using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MoviesCRUDOperationMVC.Models.Data;
using MoviesCRUDOperationMVC.Models.Entities;
using MoviesCRUDOperationMVC.ViewModels;
using NToastNotify;

namespace MoviesCRUDOperationMVC.Controllers;

public class MoviesController : Controller
{
    private readonly ApplictionDbContext _context;
    private readonly IToastNotification _toastNotification;
    private List<string> _allowedExtenstions = new List<string> { ".jpg", ".png" };
    private long _maxAllowedPosterSize = 1048576;
    public MoviesController(ApplictionDbContext context,IToastNotification toastNotification)
    {
        _context = context;
        _toastNotification = toastNotification;
    }

    public async Task<IActionResult> Index() 
    {
        var movies = await _context.Movies.OrderByDescending(m => m.Rate).ToListAsync();

        return View(movies);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new MovieFormeViewModel 
        {
            Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync(),
        };

        return View("MovieForm", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovieFormeViewModel model)
    {

        if (!ModelState.IsValid) 
        {
            model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
            return View("MovieForm", model);
        }

        var files = Request.Form.Files;
        if (!files.Any()) 
        {
            model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
            ModelState.AddModelError("Poster","Please Select Move Poster.");
            return View("MovieForm", model);
        }

        var poster = files.FirstOrDefault();
        

        if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower())) 
        {
            model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
            ModelState.AddModelError("Poster", "Only .JPG, .PNG images are allowed");
            return View("MovieForm", model);
        }

        if (poster.Length > _maxAllowedPosterSize) 
        {
            model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
            ModelState.AddModelError("Poster", "Poster can not be more than 1MB");
            return View("MovieForm", model);
        }

        using MemoryStream memoryStream = new MemoryStream();
        await poster.CopyToAsync(memoryStream);

        Movie movie = new Movie
        {
            Title = model.Title,
            StoryLine = model.StoryLine,
            Year = model.Year,
            Rate = model.Rate,
            GenreId = model.GenreId,
            Poster = memoryStream.ToArray()
        };

        await _context.Movies.AddAsync(movie);
        _context.SaveChanges();

        _toastNotification.AddSuccessToastMessage("Movie Created Successfuly.");

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id) 
    {
        if (id is null)
            return BadRequest();

        var movie = await _context.Movies.FindAsync(id);

        if (movie is null)
            return NotFound();

        var viewModel = new MovieFormeViewModel
        {
            Id= movie.Id,
            Title = movie.Title,
            GenreId = movie.GenreId,
            Rate = movie.Rate,
            Year = movie.Year,
            StoryLine = movie.StoryLine,
            Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync(),
        };

        ViewBag.Poster = movie.Poster;

        return View("MovieForm",viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MovieFormeViewModel model) 
    {
        if (!ModelState.IsValid) 
        {
            model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
            return View("MovieForm",model);
        }

        var movie = await _context.Movies.FindAsync(model.Id);

        if (movie is null)
            return NotFound();

        var files = Request.Form.Files;

        if (files.Any())
        {
            var poster = files.FirstOrDefault();
            using MemoryStream memoryStream = new MemoryStream();
            await poster.CopyToAsync(memoryStream);

            ViewBag.Poster = memoryStream.ToArray();
            
            if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower())) 
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster","Only .PNG,.JPG images are allowed!");
                model.Poster = null;
                return View("MovieForm",model);
            }

            if (poster.Length > _maxAllowedPosterSize) 
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster","Poster cannot be more than 1MB!");
                model.Poster = null;
                return View("MovieForm",model);
            }

            movie.Poster = memoryStream.ToArray();
        }


        movie.Title = model.Title;
        movie.GenreId = model.GenreId;
        movie.StoryLine = model.StoryLine;
        movie.Year = model.Year;
        movie.Rate = model.Rate;

        _context.SaveChanges();

        _toastNotification.AddSuccessToastMessage("Movie Updated Successfuly.");

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id) 
    {
        if (id is null)
            return BadRequest();

        var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);

        if (movie is null)
            return NotFound();

        return View(movie);
    }

    public async Task<IActionResult> Delete(int? id) 
    {
        if (id is null)
            return BadRequest();

        var movie = await _context.Movies.FindAsync(id);

        if (movie is null)
            return NotFound();

        _context.Remove(movie);
        _context.SaveChanges();

        return Ok();
    }

}
    