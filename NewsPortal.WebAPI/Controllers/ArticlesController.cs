using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Data.DTO;
using NewsPortal.Data.Entity;

namespace NewsPortal.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArticlesController : ControllerBase
    {
        private readonly NewsPortalContext _context;
        private readonly UserManager<User> _userManager;

        public ArticlesController(NewsPortalContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyArticles()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                return Ok(_context.Article
                        .Where(x => x.UserId == user.Id)
                            .OrderByDescending(x => x.Date)
                                .ToList()
                                    .Select(ArticleDTO.ConvertArticleToDTO));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }

        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                if (user == null)
                {
                    return BadRequest();
                }

                if ((await _context.Article.FindAsync(id)).UserId != user.Id)
                {
                    return BadRequest();
                }

                return Ok(_context.Article.Where(x => x.Id == id)
                            .AsEnumerable()
                                .Select(ArticleDTO.ConvertArticleToDTO)
                                    .Single());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }

        // PUT: api/Articles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle([FromRoute] int id, [FromBody] ArticleDTO articleDTO)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != articleDTO.Id)
                {
                    return BadRequest();
                }

                Article article = _context.Article.FirstOrDefault(x => x.Id == id);

                if (article == null)
                    return NotFound();


                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                if (user == null)
                {
                    return BadRequest();
                }

                if (article.UserId != user.Id)
                {
                    return BadRequest();
                }

                article.Id = articleDTO.Id;
                article.Title = articleDTO.Title;
                article.Summary = articleDTO.Summary;
                article.Text = articleDTO.Text;
                article.IsFeatured = articleDTO.IsFeatured;
                article.Date = articleDTO.Date;
                article.UserId = articleDTO.UserId;


                // will update by userId
                //article.Author = articleDTO.Author;

                _context.Entry(article).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                // Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/Articles
        [HttpPost]
        public async Task<IActionResult> PostArticle([FromBody] ArticleDTO articleDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var addedArticle = _context.Article.Add(new Article
                {
                    Title = articleDTO.Title,
                    Summary = articleDTO.Summary,
                    Text = articleDTO.Text,
                    IsFeatured = articleDTO.IsFeatured,
                    Date = articleDTO.Date,
                    UserId = articleDTO.UserId,

                    // will update by userId
                    Author = null

                    // images will be uploaded with picturecontroller
                });

                await _context.SaveChangesAsync();

                articleDTO.Id = addedArticle.Entity.Id;

                return CreatedAtAction("GetArticle", new { id = addedArticle.Entity.Id }, articleDTO);

            }
            catch
            {
                // Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var article = await _context.Article.FindAsync(id);

                if (article == null)
                {
                    return NotFound();
                }

                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                if (user == null)
                {
                    return BadRequest();
                }

                if (article.UserId != user.Id)
                {
                    return BadRequest();
                }

                _context.Article.Remove(article);
                await _context.SaveChangesAsync();

                return Ok(ArticleDTO.ConvertArticleToDTO(article));
            }
            catch
            {
                // Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }

    }
}