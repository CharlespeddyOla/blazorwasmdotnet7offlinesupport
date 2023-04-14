using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAcademics.Server.Interfaces.Academics.Exam
{
    public interface IACDResultsBroadSheetRepository
    {
        Task<bool> DeleteAsync(int id);
        Task<ACDBroadSheet> ExecuteScriptAsync(ACDBroadSheet model);
        Task<List<dynamic>> GetAllAsync();
        Task<List<string>> GetFieldNamesAsync();
        Task<ACDBroadSheet> UpdateAsync(ACDBroadSheet model);
    }
}
