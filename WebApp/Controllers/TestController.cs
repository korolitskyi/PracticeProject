using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApp.Controllers
{
    public class TestModel
    {
        [JsonProperty("number")]
        public int Number { get; set; }
    }


    [Route("Test")]
    public class TestController : Controller
    {
        [HttpPost("post")]
        public IActionResult Test([FromBody] TestModel tmodel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok();
        }
    }
}
