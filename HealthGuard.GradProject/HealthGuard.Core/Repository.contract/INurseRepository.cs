using HealthGuard.Core.Entities.Identity;
using HealthGuard.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Repository.contract
{
    public interface INurseRepository
    {
        Task<AppNurse?> GetAsync(string id);
        Task<IReadOnlyCollection<AppNurse>> GetAllAsync();
        Task AddAppointmentAsync(Appointment appointment);
        Task<Appointment> GetAppointmentsAsync(string appNurseId, DateTime startTime, DateTime endTime);
        Task<IReadOnlyCollection<Appointment>> GetAllAppointmentsAsync();
        Task<IReadOnlyCollection<Appointment>> GetAppointmentsForUserAsync(string userId);
        public Task RemoveAppointmentAsync(int appointmentId);
        Task Add(AppNurse item);
        void Update(AppNurse item);
        Task Delete(AppNurse item);
        Task ClearAllAppointmentsAsync();
        Task<Appointment> GetAppointmentByIdAsync(int appointmentId);
    }
}
