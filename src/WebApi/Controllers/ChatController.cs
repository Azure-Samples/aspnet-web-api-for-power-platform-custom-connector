using System.Net.Http.Headers;

using Microsoft.AspNetCore.Mvc;

using WebApi.References.AzureOpenAI;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        // GET: api/<ChatController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ChatController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ChatController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            var accessToken = this.Request.Headers["x-aoai-access-token"];
            var deploymetId = "model-gpt432k";
            var apiVersion = "2023-03-15-preview";
            Body3 body = new Body3();
            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            ChatCompletionsClient client = new ChatCompletionsClient(http);
            Response3 response = await client.CreateAsync(deployment_id: deploymetId, api_version: apiVersion, body: body);

            var result = response.Choices.First().Message.Content;

            return new OkObjectResult(result);
        }

        // PUT api/<ChatController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ChatController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
