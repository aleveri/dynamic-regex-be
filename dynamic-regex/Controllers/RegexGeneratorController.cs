using dynamic_regex.Models;
using Microsoft.AspNetCore.Mvc;

namespace dynamic_regex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegexGeneratorController : ControllerBase
    {
        private readonly DynamicRegexGenerator _dynamicRegexGenerator;

        public RegexGeneratorController(DynamicRegexGenerator dynamicRegexGenerator)
        {
            _dynamicRegexGenerator = dynamicRegexGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> Generate([FromBody] GenerateRegexRequest request)
        {
            var result = _dynamicRegexGenerator.GenerateRegex(request.AllowNumeric, request.AllowAlphaNumeric, request.MinLength, request.MaxLength, request.AllowedSpecialChars, request.NotAllowedSpecialChars);
            return Ok(new { Expression = result });
        }

        [HttpGet]
        public async Task<IActionResult> Validate(string input, string regex)
            => Ok(new { IsValid = _dynamicRegexGenerator.ValidateString(input, regex) });
    }
}
