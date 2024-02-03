using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Helpers;
using SearchService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchService.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery]SearchParams searchParams)
        {
            var query = DB.PagedSearch<Item, Item>();
            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
                query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();

            query = searchParams?.OrderBy?.ToLower() switch
            {
                "make" => query.Sort(x => x.Ascending(a => a.Make)),
                "new" => query.Sort(x => x.Ascending(a => a.CreatedAt)),
                _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
            };
            query = searchParams?.FilterBy?.ToLower() switch
            {
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                "endingsoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) 
                    && x.AuctionEnd > DateTime.UtcNow),
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(searchParams.Seller))
                query.Match(x => x.Seller == searchParams.Seller);

            query.PageNumber(searchParams.PageNumber).PageSize(searchParams.PageSize);
            var (Results, TotalCount, PageCount) = await query.ExecuteAsync();

            return Ok(new
            {
                results = Results,
                pageCount = PageCount,
                totalCount = TotalCount
            });
        }
    }
}
