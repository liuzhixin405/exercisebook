using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace ODataService.Web.Models
{
    public static class ModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<Person>("Person");
            return odataBuilder.GetEdmModel();
        }
    }
}
