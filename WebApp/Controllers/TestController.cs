using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApp.Controllers
{
    public class TestModel
    {
        [JsonProperty("dvalue")]
        public int DValue { get; set; }
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
