using Application.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Application.Services;
public interface ITemperatureService
{
    Task<TemperatureDTO> GetCurrentTemperature(Guid id);
    Task<List<TemperatureDTO>> GetTemperatureByDateRange(DateTime startDate, DateTime endDate);
    Task SetTemperature(double celsius);
}
