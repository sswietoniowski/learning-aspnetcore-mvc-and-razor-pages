using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(DbContext db) : base(db)
        {
        }

        public void Update(Company company)
        {
            var companyFromDb = dbSet.Find(company.Id);
            if (companyFromDb is not null)
            {
                // again :-), this could be simplified by using AutoMapper
                companyFromDb.Name = company.Name;
                companyFromDb.StreetAddress = company.StreetAddress;
                companyFromDb.City = company.City;
                companyFromDb.State = company.State;
                companyFromDb.PostalCode = company.PostalCode;
                companyFromDb.PhoneNumber = company.PhoneNumber;
                companyFromDb.IsAuthorizedCompany = company.IsAuthorizedCompany;
            }
        }
    }
}
