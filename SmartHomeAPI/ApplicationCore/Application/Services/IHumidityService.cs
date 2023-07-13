using Application.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Application.Services;
public interface IHumidityService
{
    Task<HumidityDTO> GetCurrentHumidity();
    Task<List<HumidityDTO>> GetHumidityByDateRange(DateTime startDate, DateTime endDate);
    Task SetHumidity(double percentage);
}
