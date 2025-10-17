using OBLIG1.Models;
using OBLIG1.ObstacleData;

namespace OBLIG1.Repository
{

    public class DataRepository : IDataRepository
    {
        private readonly ApplicationContext _context;

        public DataRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<ObstacleData.DataDto> AddData(ObstacleData.DataDto dataDto) //få cpu til å ikke vente på en oppgave
        {
            await _context.Datas.AddAsync(dataDto); //Trenger en application context 
            await _context.SaveChangesAsync();
            return dataDto;
            
        }

        public async Task<ObstacleData.DataDto> GetElementById(int id) 
        {
            var findById = await _context.Datas.Where(x => x.DataId == id)
                .FirstOrDefaultAsync(); //Sjekker om id vi får her er samme som primærnøkkel i database
            if (findById != null)
            {
                return findById;
            }
            else
            {
                return null;
            }
        }

        public async Task<ObstacleData.DataDto> DeleteById(int id)
        {
            var elementById =
                await _context.Datas.FindAsync(id); //lurt å skrive await hver gang man snakker med databasen
            if(elementById != null)
            {
                _context.Datas.Remove(elementById);
                await _context.SaveChangeAsync(); //lagre endringer
                return elementById;
            }
            else
            {
                return null; //finner vi ikke noe i databasen
            }
        }
    }
    public async Task<ObstacleData.DataDto> UpdateData(ObstacleData.DataDto dataDto)
    {
        _context.Datas.Update(dataDto);
        await _context.SaveChangesAsync();
        return dataDto;
    }
    
    //få alt som er lagt til i databasen
    public async Task<IEnumerable<DataDto>> GetAllData(DataDto dataDto)
    {
        var getAllData = await _context.Datas.Take(50).ToListAsync();
        return getAllData;
    }
    //når vi kaller kontrolleren noe, hvis vi vil vise det, må vi lage samme navn i view
    //hvis man vil ha frontent til dette må det legges mappe til i view som heter Data
    //en controller skal ikke gjøre hele jobben, ha flere forskjellige
    //controller returnerer en html side/et view
    
}
