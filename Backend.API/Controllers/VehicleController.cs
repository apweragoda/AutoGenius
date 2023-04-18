using Backend.API.Contracts;
using Backend.API.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly IVehicleRepository _vehicleRepo;
        public VehicleController(IVehicleRepository vehicleRepo, IConfiguration configuration)
        {
            _vehicleRepo = vehicleRepo;
            _configuration = configuration;
        }


        // GET: api/<VehicleController>
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            try
            {
                var vehicles = await _vehicleRepo.GetAllVehicles();
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<VehicleController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleByUserId(int id)
        {
            try
            {
                var vehicle = await _vehicleRepo.GetVehicleByUserId(id);
                if (vehicle == null)
                    return NotFound();

                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<VehicleController>
        [HttpPost]
        public async Task<IActionResult> AddVehicle(VehicleModel vehicle)
        {
            try
            {
                if (vehicle.Vin != null && vehicle.Model != null && vehicle.Make != null)
                {
                    var vehicleDb = await _vehicleRepo.AddVehicle(vehicle);

                    return Ok(vehicleDb);
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        // PUT api/<VehicleController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(VehicleModel vehicle)
        {
            try
            {
                if (vehicle != null)
                {
                    var vehicleDb = await _vehicleRepo.UpdateVehicle(vehicle);

                    return Ok(vehicleDb);
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        // DELETE api/<VehicleController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(string vin)
        {
            try
            {
                if (vin != null)
                {
                    var userDb = await _vehicleRepo.DeleteVehicle(vin);

                    return Ok(userDb);
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }
    }
}
