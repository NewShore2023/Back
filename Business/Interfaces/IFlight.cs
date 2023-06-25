using DataAccess.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
	public interface IFlight
	{
		Task<Journey> GetFlight(string Origin, string Destination);
	}
}
