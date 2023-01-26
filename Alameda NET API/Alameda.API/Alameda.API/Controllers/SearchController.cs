using Microsoft.AspNetCore.Mvc;
using Alameda.API.Model;
using System.Text;

namespace Alameda.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> GetSearchResult([FromBody] QueryInput entry)
        {
            SearchClass searchLogic = new SearchClass();
            string[] output;
            string input = entry.Input;
            string query = entry.Query;

            input = input.Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' '); //Remove any new line tags and replace them with spaces

            if (entry.MatchCase == false)
                query = query.ToLower();

            if (entry.IgnorePunctuation == true)
            {
                Tuple<string, string> editted = searchLogic.removePunctuation(input, query);
                input = editted.Item1;
                query = editted.Item2;
            }

            List<Tuple<string, TokenType>> queryTokens = searchLogic.parseQuery(query);

            if (queryTokens.Count == 1 && queryTokens[0].Item2 == TokenType.String && queryTokens[0].Item1.Length == 1 && (Char.IsPunctuation(queryTokens[0].Item1[0]) || queryTokens[0].Item1 == " ")) // If the query is only a space or punctuation
            {
                int numFound = input.Split(queryTokens[0].Item1[0]).Length - 1;
                output = new string[numFound];
                Array.Fill(output, queryTokens[0].Item1);
            }
            else
                output = searchLogic.searchString(input, queryTokens, entry.MatchWhole, entry.MatchCase);

            return Ok(output);
        }


    } 
}