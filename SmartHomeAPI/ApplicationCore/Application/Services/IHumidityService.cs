using Application.Application.DTOs;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Application.Services;

public interface IHumidityService
{
    Task<HumidityDTO> GetCurrentHumidity(string sensorTitle);
    Task<List<HumidityDTO>> GetHumidityByDateRange(
        DateTime startDate,
        DateTime endDate,
        string sensorTitle
    );
    Task SetHumidity(double percentage, string sensorTitle);
}
