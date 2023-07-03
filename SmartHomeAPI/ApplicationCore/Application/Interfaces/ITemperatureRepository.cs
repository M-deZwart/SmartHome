﻿using SmartHomeAPI.Application.Entities;

namespace Application.Application.Interfaces
{
    public interface ITemperatureRepository
    {
        Task Create(Temperature temperature);
        Task<Temperature> GetById(Guid id);
        Task<List<Temperature>> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
