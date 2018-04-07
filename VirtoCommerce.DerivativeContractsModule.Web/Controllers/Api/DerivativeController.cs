using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.DerivativeContractsModule.Core.Services;
using VirtoCommerce.DerivativeContractsModule.Web.Security;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Web.Security;

namespace VirtoCommerce.DerivativeContractsModule.Web.Controllers.Api
{
    [RoutePrefix("api/contracts/derivative")]
    [CheckPermission(Permission = PredefinedPermissions.DerivativeContractRead)]
    public class DerivativeContractController : ApiController
    {
        private readonly IDerivativeContractService _derivativeContractService;
        private readonly IDerivativeContractSearchService _derivativeContractSearchService;

        public DerivativeContractController()
        {
        }

        public DerivativeContractController(IDerivativeContractService DerivativeContractService, IDerivativeContractSearchService DerivativeContractSearchService)
        {
            _derivativeContractService = DerivativeContractService;
            _derivativeContractSearchService = DerivativeContractSearchService;
        }

        /// <summary>
        /// Get derivative contracts by IDs
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(DerivativeContract[]))]
        public IHttpActionResult GetByIds([FromUri] string[] ids)
        {
            var retVal = _derivativeContractService.GetDerivativeContractsByIds(ids);
            return Ok(retVal.ToArray());
        }

        /// <summary>
        ///  Create new or update existing derivative contract
        /// </summary>
        /// <param name="derivativeContracts">Derivative contracts</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.DerivativeContractUpdate)]
        public IHttpActionResult Update(DerivativeContract[] derivativeContracts)
        {
            _derivativeContractService.SaveDerivativeContracts(derivativeContracts);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Delete derivative contracts by IDs
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.DerivativeContractDelete)]
        public IHttpActionResult Delete([FromUri] string[] ids)
        {
            _derivativeContractService.DeleteDerivativeContracts(ids);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Search for FulfillmentCenterMapping by AssetEntrySearchCriteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [ResponseType(typeof(GenericSearchResult<DerivativeContract>))]
        public IHttpActionResult Search(DerivativeContractSearchCriteria criteria)
        {
            var result = _derivativeContractSearchService.SearchDerivativeContracts(criteria);
            return Ok(result);
        }
    }
}
