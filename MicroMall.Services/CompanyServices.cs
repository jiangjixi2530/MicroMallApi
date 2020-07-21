using MicroMall.Common;
using MicroMall.ISerivces;
using MicroMall.Model;
using MicroMall.Model.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroMall.Services
{
    public class CompanyServices : ICompanyServices
    {
        private readonly IRepository repository;

        public CompanyServices(IRepository repository)
        {
            this.repository = repository;
        }

        public mall_company GetCompanyByPhone(string phone)
        {
            return repository.Single<mall_company>(x => x.Phone == phone);
        }

        public mall_company GetCompanyByUserName(string userName)
        {
            return repository.Single<mall_company>(x => x.UserName == userName);
        }

        public mall_company GetCompayById(int id)
        {
            return repository.GetById<mall_company>(id);
        }

        public int Register(string name, string userName, string pwd, string phone, int areaId)
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
                int parentId = repository.AddReturnId(companyParent);
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
                var id = repository.AddReturnId(companyParent);
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
