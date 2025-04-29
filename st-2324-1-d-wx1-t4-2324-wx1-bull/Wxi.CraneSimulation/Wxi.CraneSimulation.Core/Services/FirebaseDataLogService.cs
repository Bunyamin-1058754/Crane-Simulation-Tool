using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Wxi.CraneSimulation.Core.Entities;

namespace Wxi.CraneSimulation.Core.Services
{
    public class FirebaseDataLogService
    {
        private readonly FirebaseService _firebaseService;

        public FirebaseDataLogService()
        {
            _firebaseService = new FirebaseService();
        }

        public async Task<List<DataLog>> GetAllAsync()
        {
            return (await _firebaseService.client.Child(nameof(DataLog)).OnceAsync<DataLog>()).Select(l => new DataLog
            {
                Id = l.Object.Id,
                X = l.Object.X,
                Y = l.Object.Y,
                Date = l.Object.Date,
                IsError = l.Object.IsError,
                Name = l.Object.Name,

            }).ToList();
        }

        public async Task<bool> CreateAsync(DataLog dataLog)
        {
            var jsonResult = JsonConvert.SerializeObject(dataLog);
            var response = await _firebaseService.client.Child(nameof(DataLog)).PostAsync(jsonResult);
            if(response.Key != null)
            {
                return true;
            }
            return false;
        }

    }
}
