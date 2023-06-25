using Business.Interfaces;
using DataAccess.model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System.Text;


namespace Business.Domain
{
	public class FlightServices : IFlight
	{
		private static string _baseUrl = "";

		private readonly IConfiguration _configuration;

		private readonly HttpClient _httpClient;

		public FlightServices(IConfiguration configuration)
		{

			_configuration = configuration;
			_httpClient = new HttpClient();
			_baseUrl = _configuration.GetSection("ApiSetting")["baseUrl"];
			 Log.ForContext<Flight>(); 

		}
		public async Task<Journey> GetFlight(string origin, string destination)
		{

			try
			{
				var datos = new
				{
					Origin = origin,
					Destination = destination
				};

				var json = System.Text.Json.JsonSerializer.Serialize(datos);
				var contenido = new StringContent(json, Encoding.UTF8, "application/json");
				Journey journey = new Journey();

				var request = new HttpRequestMessage
				{
					Method = HttpMethod.Get,
					RequestUri = new Uri(_baseUrl),
					Content = contenido
				};

				HttpResponseMessage response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					string contenidoRespuesta = await response.Content.ReadAsStringAsync();

					List<Response> vuelos = JsonConvert.DeserializeObject<List<Response>>(contenidoRespuesta);

					List<Flight> flightsData = new List<Flight>();
					List<Flight> flightsDataF = new List<Flight>();
					List<Flight> flightsDataFinal = new List<Flight>();


					for (int i = 0; i < vuelos.Count; i++)
					{
						Response vuelo = vuelos[i];

						Flight flightData = new Flight
						{
							Origin = vuelo.DepartureStation,
							Destination = vuelo.ArrivalStation,
							Price = vuelo.Price,
							Transport = new Transport
							{
								FlightCarrier = vuelo.FlightCarrier,
								FlightNumber = vuelo.FlightNumber
							}
						};

						if (vuelo.DepartureStation == origin || vuelo.ArrivalStation == destination || ((vuelo.DepartureStation == origin) && (vuelo.ArrivalStation == destination)))
						{

							flightsData.Add(flightData);

						}

					}
					for (int i = 0; i < flightsData.Count; i++)
					{
						Flight flight = flightsData[i];

						for (int j = 1; j < flightsData.Count - 1; j++)
						{
							Flight flightNext = flightsData[j];
							if (flight.Origin == flightNext.Destination || flight.Destination == flightNext.Origin ||
								((flight.Origin == origin) && (flight.Destination == destination)))
							{

								Log.Information("coincidencias de origin y destino");
								flightsDataFinal.Add(flight);
								flightsDataFinal.Add(flightNext);
								journey = new()
								{
									Origin = origin,
									Destination = destination,
									Price = flight.Price + flightNext.Price,
									Flights = flightsDataFinal
								};
								return journey;
							}
							else
							{

								journey = new()
								{
									Origin = origin,
									Destination = destination,
									Price = 0,
									Flights = null
								};

							}
						}

					}

					return journey;
				}
				else
				{

					Log.Error("Error:::::" + response.ToString());

					throw new Exception(response.Content.ReadAsStringAsync().Result);
				}
			}
			catch (Exception ex)
			{
				Log.Error("Error::.." + ex.ToString());
				throw ex;
			}
		}
	}

}
