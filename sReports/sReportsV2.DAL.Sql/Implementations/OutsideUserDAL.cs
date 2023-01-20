using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.OutsideUser;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.SqlDomain.Implementations
{
    public class OutsideUserDAL : IOutsideUserDAL
    {
        private SReportsContext context;
        public OutsideUserDAL(SReportsContext context)
        {
            this.context = context;
        }

        public void Delete(int id)
        {
            OutsideUser formDb = context.OutsideUsers.FirstOrDefault(x => x.OutsideUserId == id);
            if (formDb != null)
            {
                formDb.IsDeleted = true;
                formDb.SetLastUpdate();
                context.SaveChanges();
            }
        }


        public List<OutsideUser> GetAllByIds(List<int> ids)
        {
            return context.OutsideUsers.Include(x => x.Address).Where(x => ids.Contains(x.OutsideUserId)).ToList();
        }

        public OutsideUser GetById(int id)
        {
            return context.OutsideUsers.Include(x => x.Address).FirstOrDefault(x => x.OutsideUserId == id);
        }

        public int InsertOrUpdate(OutsideUser user)
        {
            if (user.OutsideUserId == 0)
            {
                context.OutsideUsers.Add(user);
            }
            else
            {
                OutsideUser dbUser = this.GetById(user.OutsideUserId);
                dbUser.FirstName = user.FirstName;
                dbUser.LastName = user.LastName;
                dbUser.Email = user.Email;
                dbUser.Institution = user.Institution;
                dbUser.InstitutionAddress = user.InstitutionAddress;
                dbUser.Address = UpdateAddress(dbUser.Address, user.Address);
                dbUser.SetLastUpdate();
            }
            context.SaveChanges();
            return user.OutsideUserId;

        }

        private Address UpdateAddress(Address dbAddress, Address newAddress) 
        {
            dbAddress.City = newAddress.City;
            dbAddress.State = newAddress.State;
            dbAddress.PostalCode = newAddress.PostalCode;
            dbAddress.Country = newAddress.Country;
            dbAddress.Street = newAddress.Street;
            dbAddress.StreetNumber = newAddress.StreetNumber;

            return dbAddress;
        }
    }
}
