using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public LocationController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public async Task<ActionResult> GetLocations(double latitude, double longitude)
        {
            // Adjust these values with your Google Places API key and desired search parameters
            string apiKey = "AIzaSyDcbfpvwD1RR09CEGCy8OeYM8o3WXr2OhY";
            string types = "restaurant";
            int radius = 1000;

            string apiUrl = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={latitude},{longitude}&radius={radius}&types={types}&key={apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            // You may need to create a model class to deserialize the JSON response
            // Adjust the deserialization logic based on your response structure
            var locations = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            return Ok(locations);
        }
    }
}