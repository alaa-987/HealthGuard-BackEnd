using HealthGuard.Core.Entities.Identity;
using HealthGuard.Core.Repository.contract;
using HealthGurad.Repository.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGurad.Repository
{
    public class NurseRepository : INurseRepository
    {
        private readonly AppIDentityDbContext _dbContext;

        public NurseRepository(AppIDentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Add(AppNurse item)
        {
            await _dbContext.AppNurses.AddAsync(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            try
            {
                await _dbContext.Appointments.AddAsync(appointment);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error adding appointment: {ex}");
                throw;
            }
        }

    

        public async Task ClearAllAppointmentsAsync()
        {
            _dbContext.Appointments.RemoveRange(_dbContext.Appointments);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(AppNurse item)
        {
            _dbContext.AppNurses.Remove(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Appointment>> GetAllAppointmentsAsync()
        {
            return await _dbContext.Appointments
                       .Include(a => a.AppNurse)
                       .Include(a => a.User)
                       .ToListAsync();
        }

        public async Task<IReadOnlyCollection<AppNurse>> GetAllAsync()
        {
            return await _dbContext.AppNurses.ToListAsync();
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _dbContext.Appointments
                       .Include(a => a.AppNurse)
                       .Include(a => a.User)
                      .FirstOrDefaultAsync(a => a.Id == appointmentId);

        }

        public async Task<Appointment> GetAppointmentsAsync(string appNurseId, DateTime startTime, DateTime endTime)
        {
            return await _dbContext.Appointments
                                .FirstOrDefaultAsync(a => a.AppNurseId == appNurseId &&
                                a.StartTime < endTime &&
                                a.EndTime > startTime);
        }

        public async Task<IReadOnlyCollection<Appointment>> GetAppointmentsForUserAsync(string userId)
        {
            return await _dbContext.Appointments
                 .Where(a => a.AppUserId == userId)
                 .ToListAsync();
        }

        public async Task<AppNurse?> GetAsync(string id)
        {
            return await _dbContext.AppNurses.FindAsync(id);
        }

        public async Task RemoveAppointmentAsync(int appointmentId)
        {
            var appointment = await _dbContext.Appointments.FindAsync(appointmentId);
            if (appointment != null)
            {
                _dbContext.Appointments.Remove(appointment);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async void Update(AppNurse item)
        {
            _dbContext.AppNurses.Update(item);
            _dbContext.SaveChanges();
        }

    }
}
