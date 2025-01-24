using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace MDProxyServer.Controllers;
[ApiController]
public class simpleSearchController : ControllerBase
{
    private readonly RateLimiter _ratelimiter;
    private readonly RecommendationService _recommendationService;
    public simpleSearchController( RateLimiter rateLimiter, RecommendationService recommendationService)
    {
        _ratelimiter = rateLimiter;
        _recommendationService = recommendationService;
    }
    [HttpGet("/TopManga")]
    public async Task<IActionResult> GetTopManga(string order = "latestUploadedChapter")
    {
        var permit = await _ratelimiter.AcquireAsync(1);
        if(!permit.IsAcquired)
        {
            return StatusCode(429, "Rate limit exceeded. Please try again later.");
        }
        try
        {
                    using(HttpClient client = new HttpClient())
        {
            
            var endpoint = new Uri($"https://api.mangadex.org/manga/?includes[]=cover_art&limit=10&order[{order}]=desc");
            var request = new HttpRequestMessage(HttpMethod.Get,endpoint);
            request.Headers.Add("user-agent","mangaMagnetPersonalApp");
            var response = await client.SendAsync(request);
            var responseMsg = response.Content.ReadAsStringAsync().Result;
            return Ok(responseMsg);
        }
        }
        finally
        {
            permit.Dispose();
        }

    }
    [HttpGet("/Manga")]
    public async Task<IActionResult> getMangaInfo(string mangaId)
    {
        var permit = await _ratelimiter.AcquireAsync(1);
        if(!permit.IsAcquired)
        {
            return StatusCode(429, "Rate limit exceeded. Please try again later.");
        }
        try
        {
            using(HttpClient client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Get,$"https://api.mangadex.org/manga/{mangaId}?includes[]=manga&includes[]=cover_art&includes[]=author&includes[]=artist&includes[]=tag&includes[]=creator");
            request.Headers.Add("user-agent","mangaMagnetPersonalApp");
            var response = await client.SendAsync(request);
            var responseMsg = response.Content.ReadAsStringAsync().Result;
            return Ok(responseMsg);
        }
        }
        finally
        {
            permit.Dispose();
        }

    }
    [HttpGet("/Authors")]
    public async Task<IActionResult> getAuthorList(string authorName)
    {
        var permit = await _ratelimiter.AcquireAsync(1);
        if(!permit.IsAcquired)
        {
            return StatusCode(429, "Rate limit exceeded. Please try again later.");
        }
        try
        {
                    using(HttpClient client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Get,$"https://api.mangadex.org/author?name={authorName}");
            request.Headers.Add("user-agent","mangaMagnetPersonalApp");
            var response = await client.SendAsync(request);
            var responseMsg = response.Content.ReadAsStringAsync().Result;
            return Ok(responseMsg);
        }
        }
        finally
        {
            permit.Dispose();
        }

    }
    [HttpGet("/Authors/{authorId}")]
    public async Task<IActionResult> getAuthor(string authorId)
    {
        var permit = await _ratelimiter.AcquireAsync(1);
        if(!permit.IsAcquired)
        {
            return StatusCode(429, "Rate limit exceeded. Please try again later.");
        }
        try{
                    using(HttpClient client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Get,$"https://api.mangadex.org/author/{authorId}");
            request.Headers.Add("user-agent","mangaMagnetPersonalApp");
            var response = await client.SendAsync(request);
            var responseMsg = response.Content.ReadAsStringAsync().Result;
            return Ok(responseMsg);

        }
        }
        finally
        {
            permit.Dispose();
        }
    }
    [HttpGet("/mangaCover")]
    public async Task<IActionResult> getMangaCover(string mangaId="", string imageId = "")
    {
        //if(System.IO.File.Exists($"wwwroot/MangaCovers/{mangaId}{imageId}"))
        //{
        //    return Ok("Already Downloaded");
        //}
        //using(WebClient client = new WebClient())
        //{
        //    if(mangaId == "" | imageId == "")
        //    {
        //        return NotFound();
         //   }
           // var endpoint = new Uri($"https://uploads.mangadex.org/covers/{mangaId}/{imageId}");
            //client.Headers["user-agent"] = "mangaMagnetPersonalApp";
            //await client.DownloadFileTaskAsync(endpoint, $"wwwroot/MangaCovers/{mangaId}{imageId}");
        //}
        var permit = await _ratelimiter.AcquireAsync(1);
        if(!permit.IsAcquired)
        {
            return StatusCode(429, "Rate limit exceeded. Please try again later.");
        }
        try
        {
            using(HttpClient client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Get,$"https://uploads.mangadex.org/covers/{mangaId}/{imageId}");
            request.Headers.Add("user-agent","mangaMagnetPersonalApp");
            var response = await client.SendAsync(request);
            var responseMsg = response.Content.ReadAsStreamAsync().Result;


            return Ok(responseMsg);
        }
        }
        finally
        {
            permit.Dispose();
        }
    }
    [HttpGet("/randomManga")]
    public async Task<IActionResult> getRandomCover()
    {
        var permit = await _ratelimiter.AcquireAsync(1);
        if(!permit.IsAcquired)
        {
            return StatusCode(429, "Rate limit exceeded. Please try again later.");
        }
        try
        {
             using(HttpClient client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Get,"https://api.mangadex.org/manga/random?includes[]=manga&includes[]=cover_art&includes[]=author&includes[]=artist&includes[]=tag&includes[]=creator");
            request.Headers.Add("user-agent","mangaMagnetPersonalApp");
            var response = await client.SendAsync(request);
            var responseMsg = response.Content.ReadAsStringAsync().Result;
            return Ok(responseMsg);
        }
        }
        finally
        {
            permit.Dispose();
        }

    }
    [HttpGet("/customSearch")]
    public async Task<IActionResult> customSearch()
    {
        var permit = await _ratelimiter.AcquireAsync(1);
        var queryString = Request.QueryString.Value;
        var thirdPartyApiBaseUrl = "https://api.mangadex.org/manga";
        var thirdPartyApiUrl = $"{thirdPartyApiBaseUrl}{queryString}";
        if(!permit.IsAcquired)
        {
            return StatusCode(429, "Rate limit exceeded. Please try again later.");
        }
        try
        {
            using(HttpClient client = new HttpClient())
        {
            
            var request = new HttpRequestMessage(HttpMethod.Get,thirdPartyApiUrl);
            request.Headers.Add("user-agent","mangaMagnetPersonalApp");
            var response = await client.SendAsync(request);
            var responseMsg = response.Content.ReadAsStringAsync().Result;
            return Ok(responseMsg);
        }
        }
        finally
        {
            permit.Dispose();
        }
    }
    [HttpGet("{mangaId}")]
    public IActionResult getRecommendations(string mangaId)
    {
        var recommendations = _recommendationService.GetRecommendations(mangaId);
        if(recommendations == null)
        {
            return NotFound();
        }
        return Ok(recommendations);
    }

}