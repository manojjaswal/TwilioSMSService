using HC.Model;
using HC.Patient.Data;
using HC.Patient.Repositories.IRepositories.APIKeyConfigurations;
using HC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HC.Patient.Repositories.Repositories.APIKeyConfigurations
{
    public class APIKeyConfigurationsRepository: RepositoryBase<Entity.APIKeyConfigurations>, IAPIKeyConfigurationsRepository
    {

        private HCOrganizationContext _context;

        public APIKeyConfigurationsRepository(HCOrganizationContext context) : base(context)
        {
            this._context = context;
        }


       public List<Entity.APIKeyConfigurations> GetAllApiKeys()
        {
            return _context.APIKeyConfigurations.Where(x => x.OrganizationId == 128).ToList();
        }

    }
}
