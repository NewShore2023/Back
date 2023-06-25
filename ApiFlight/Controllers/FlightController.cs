using Business.Interfaces;
using DataAccess.model;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiFlight.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FlightController : ControllerBase
	{
		IFlight _iflight;

		public FlightController(IFlight iflight)
		{
			_iflight = iflight;	
		}
		

		// Post 
		[HttpPost("/PostFlight")]
		public async Task<Journey> PostFlight([FromBody] FligthRequest request)
		{	
				return await _iflight.GetFlight(request.origin,request.destination);
			
		}
		// GET 
		[HttpGet("/GetFlight")]
		public async Task<Journey> GetFlight([FromQuery] FligthRequest request)
		{
			return await _iflight.GetFlight(request.origin, request.destination);

		}
		// GET 
		[HttpGet("/")]
		public string Get()
		{
			return "Get Api";
		}


	}
}
