using System;
using Microsoft.Extensions.Configuration;
using sesto.api.Infastructure;
using sesto.api.Services.Interfaces;

namespace sesto.api.Services.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly SestoDbContext _dbContext;
        private IConfiguration _config;
        public DashboardRepository(SestoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    