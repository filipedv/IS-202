using System.Collections.Generic;
using System.Threading.Tasks;
using OBLIG1.Models;

namespace OBLIG1.Repository
{

    public interface IDataRepository
    {
        Task<DataDto> AddData(DataDto dataDto);
        Task<DataDto> GetElementById(int id);
        Task<IEnumerable<DataDto>> GetAllData();
        Task<DataDto> DeleteById(int id);
        
        Task<DataDto> UpdateData(DataDto datadto);
        
    }
}