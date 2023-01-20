using CsvHelper;
using MongoDB.Driver;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace sReportsV2.Initializer.OrganizationCSV
{
    public class ImportOrganization
    {
        public IOrganizationDAL organizationDAL;

        public ImportOrganization()
        {
        }

        public ImportOrganization(IOrganizationDAL organizationDAL)
        {
            this.organizationDAL = organizationDAL;
        }

        public List<CsvOrganization> LoadOrganizationsFromCsv(string csvPath) 
        {
            List<CsvOrganization> csvOrganizations = new List<CsvOrganization>();
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvOrganizations = csv.GetRecords<CsvOrganization>().ToList();
            }

            return csvOrganizations;
        }

        public void InsertOrganization(string csvPath, int? countryId)
        {
            if (organizationDAL.GetAllCount() == 0) 
            {
                List<CsvOrganization> csvOrganizations = LoadOrganizationsFromCsv(csvPath);
                List<IGrouping<string, CsvOrganization>> groupedOrganizations = csvOrganizations.GroupBy(x => x.InstitutionName).ToList();

                for (int j = 0; j < groupedOrganizations.Count; j++)
                {
                    var partOf = InsertHeadOrganization(groupedOrganizations, j, countryId);

                    if (groupedOrganizations.ElementAt(j).Count() > 1)
                        InsertSubOrganizations(groupedOrganizations, j, partOf, countryId);
                }
            }
        }

        private int? InsertHeadOrganization(List<IGrouping<string, CsvOrganization>> groupedOrganizations, int j, int? countryId) 
        {
            Organization organizationEntry = new Organization
            {
                Name = $"{groupedOrganizations.ElementAt(j).Select(x => x.InstitutionName).FirstOrDefault()}, {groupedOrganizations.ElementAt(j).Select(x => x.Standort).FirstOrDefault()}",
                Address = new Address
                {
                    Street = groupedOrganizations.ElementAt(j).Select(x => x.StreetName).FirstOrDefault(),
                    City = groupedOrganizations.ElementAt(j).Select(x => x.PostalNumberAndMunicipality).FirstOrDefault().Split(new[] { ' ' }, 2)[1],
                    CountryId = countryId,
                    State = groupedOrganizations.ElementAt(j).Select(x => x.LocationCanton).FirstOrDefault(),
                    PostalCode = groupedOrganizations.ElementAt(j).Select(x => x.PostalNumberAndMunicipality).FirstOrDefault().Split(' ').First(),
                }
            };

            organizationDAL.InsertOrUpdate(organizationEntry);
            return organizationEntry.OrganizationId;
        }

        private void InsertSubOrganizations(List<IGrouping<string, CsvOrganization>> groupedOrganizations, int j, int? partOf, int? countryId)
        {
            //TO DO IMEPLEMENT 

            for (int i = 1; i < groupedOrganizations.ElementAt(j).Count(); i++)
            {
                Organization organizationEntry = new Organization
                {
                    Name = $"{groupedOrganizations.ElementAt(j).Select(x => x.InstitutionName).ElementAt(i)}, {groupedOrganizations.ElementAt(j).Select(x => x.Standort).ElementAt(i)}",
                    Address = new Address
                    {
                        Street = groupedOrganizations.ElementAt(j).Select(x => x.StreetName).ElementAt(i),
                        City = groupedOrganizations.ElementAt(j).Select(x => x.PostalNumberAndMunicipality).ElementAt(i).Split(new[] { ' ' }, 2)[1],
                        CountryId = countryId,
                        State = groupedOrganizations.ElementAt(j).Select(x => x.LocationCanton).ElementAt(i),
                        PostalCode = groupedOrganizations.ElementAt(j).Select(x => x.PostalNumberAndMunicipality).ElementAt(i).Split(' ').First(),
                    }
                    //,
                    //OrganizationRelation = new OrganizationRelation()
                    //{
                    //    ParentId = partOf.Value,
                    //}
                    // ,ParentId = partOf,
                    //Ancestors = new List<int>() { partOf }
                };

                organizationDAL.InsertOrUpdate(organizationEntry);
                OrganizationRelation relation = new OrganizationRelation()
                {
                    ParentId = partOf.Value,
                    ChildId = organizationEntry.OrganizationId
                };
                organizationDAL.InsertOrganizationRelation(relation);
                organizationEntry.OrganizationRelationId = relation.OrganizationRelationId;
                organizationDAL.InsertOrUpdate(organizationEntry);
            }
        }

    }
}
