using MicroMall.ISerivces;
using MicroMall.Model.DbModels;
using MicroMall.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroMall.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly IBaseRepository repository;

        public CategoryServices(IBaseRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<mall_category>> GetCategories(int groupId)
        {
            return await repository.GetList<mall_category>(x => x.GroupId == groupId);
        }
    }
}
