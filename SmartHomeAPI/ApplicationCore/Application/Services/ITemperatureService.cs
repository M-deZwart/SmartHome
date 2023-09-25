using Application.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Application.Services;

public interface ITemperatureService
{
    Task<TemperatureDTO> GetCurrentTemperature(string sensorTitle);
    Task<List<TemperatureDTO>> GetTemperatureByDateRange(
        DateTime startDate,
        DateTime endDate,
        string sensorTitle
    );
    Task SetTemperature(double celsius, string sensorTitle);
}
