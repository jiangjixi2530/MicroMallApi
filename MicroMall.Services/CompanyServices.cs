using MicroMall.Common;
using MicroMall.IRepository;
using MicroMall.ISerivces;
using MicroMall.Model;
using MicroMall.Model.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroMall.Services
{
    public class CompanyServices : ICompanyServices
    {
        private readonly IBaseRepository repository;

        public CompanyServices(IBaseRepository repository)
        {
            this.repository = repository;
        }

        public async Task<mall_company> GetCompanyByPhone(string phone)
        {
            return await repository.Single<mall_company>(x => x.Phone == phone);
        }

        public async Task<mall_company> GetCompanyByUserName(string userName)
        {
            return await repository.Single<mall_company>(x => x.UserName == userName);
        }

        public async Task<mall_company> GetCompayById(int id)
        {
            return await repository.GetById<mall_company>(id);
        }

        public async Task<int> Register(string name, string userName, string pwd, string phone, int areaId)
        {
            repository.BeginTran();
            try
            {
                mall_company companyParent = new mall_company
                {
                    CompanyName = name,
                    UserName = userName,
                    PassWord = pwd,
                    Phone = phone,
                    AreaId = areaId,
                    ParentId = 0,
                    CreateDate = AppSetting.NowTime,
                    ModifyDate = AppSetting.NowTime
                };
                int parentId = await repository.AddReturnId(companyParent);
                mall_company company = new mall_company
                {
                    CompanyName = name,
                    UserName = userName + "-1",
                    PassWord = pwd,
                    Phone = phone,
                    AreaId = areaId,
                    ParentId = parentId,
                    CreateDate = AppSetting.NowTime,
                    ModifyDate = AppSetting.NowTime
                };
                var id = await repository.AddReturnId(companyParent);
                repository.CommitTran();
                return id;
            }
            catch
            {
                repository.RollbackTran();
            }
            return default;
        }
    }
}
