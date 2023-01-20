using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class AddressDAL : IAddressDAL
    {
        private readonly SReportsContext context;

        public AddressDAL(SReportsContext context)
        {
            this.context = context;
        }

        public void InsertOrUpdate(Address address)
        {
            if(address.AddressId == 0)
            {
                context.Address.Add(address);
            }

            context.Entry(address).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }
    }
}
