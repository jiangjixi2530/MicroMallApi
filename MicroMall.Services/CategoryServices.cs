using MicroMall.ISerivces;
using MicroMall.Model;
using MicroMall.Model.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroMall.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly IRepository repository;

        public CategoryServices(IRepository repository)
        {
            this.repository = repository;
        }

        public List<mall_category> GetCategories(int groupId)
        {
            return repository.GetList<mall_category>(x => x.GroupId == groupId);
        }
    }
}
