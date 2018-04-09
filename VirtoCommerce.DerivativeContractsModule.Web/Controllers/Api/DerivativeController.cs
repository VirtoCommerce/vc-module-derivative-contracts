using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.DerivativeContractsModule.Core;
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
        private readonly IDerivativeContractInfoEvaluator _derivativeContractInfoEvaluator;

        public DerivativeContractController()
        {
        }

        public DerivativeContractController(IDerivativeContractService derivativeContractService, IDerivativeContractSearchService derivativeContractSearchService, IDerivativeContractInfoEvaluator derivativeContractInfoEvaluator)
        {
            _derivativeContractService = derivativeContractService;
            _derivativeContractSearchService = derivativeContractSearchService;
            _derivativeContractInfoEvaluator = derivativeContractInfoEvaluator;
        }

        /// <summary>
        /// Get derivative contracts by IDs
        /// </summary>
        /// <param name="ids">Derivative contracts IDs</param>
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
        /// Get derivative contract items by IDs
        /// </summary>
        /// <param name="ids">Derivative contract items IDs</param>
        /// <returns></returns>
        [HttpGet]
        [Route("items")]
        [ResponseType(typeof(DerivativeContractItem[]))]
        public IHttpActionResult GetItemsByIds([FromUri] string[] ids)
        {
            var retVal = _derivativeContractService.GetDerivativeContractItemsByIds(ids);
            return Ok(retVal.ToArray());
        }

        /// <summary>
        /// Evaluate derivative contract info
        /// </summary>
        /// <param name="context">Derivative contract info evaluation context</param>
        [HttpPost]
        [ResponseType(typeof(DerivativeContractInfo[]))]
        [Route("infos/evaluate")]
        public IHttpActionResult EvaluatePromotions(DerivativeContractInfoEvaluationContext context)
        {
            var retVal = _derivativeContractInfoEvaluator.EvaluateDerivativeInfos(context);
            return Ok(retVal);
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
        ///  Create new or update existing derivative contract item
        /// </summary>
        /// <param name="derivativeContractItems">Derivative contract items</param>
        /// <returns></returns>
        [HttpPost]
        [Route("items")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.DerivativeContractUpdate)]
        public IHttpActionResult Update(DerivativeContractItem[] derivativeContractItems)
        {
            _derivativeContractService.SaveDerivativeContractItems(derivativeContractItems);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Delete derivative contracts by IDs
        /// </summary>
        /// <param name="ids">Derivative contracts IDs</param>
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
        /// Delete derivative contracts by IDs
        /// </summary>
        /// <param name="ids">Derivative contracts IDs</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("items")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.DerivativeContractDelete)]
        public IHttpActionResult DeleteItems([FromUri] string[] ids)
        {
            _derivativeContractService.DeleteDerivativeContractItems(ids);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Search for derivative contracts
        /// </summary>
        /// <param name="criteria">Derivative contracts search criteria</param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [ResponseType(typeof(GenericSearchResult<DerivativeContract>))]
        public IHttpActionResult Search(DerivativeContractSearchCriteria criteria)
        {
            var result = _derivativeContractSearchService.SearchDerivativeContracts(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Search for derivative contract items
        /// </summary>
        /// <param name="criteria">Derivative contract items search criteria</param>
        /// <returns></returns>
        [HttpPost]
        [Route("items/search")]
        [ResponseType(typeof(GenericSearchResult<DerivativeContractItem>))]
        public IHttpActionResult SearchItems(DerivativeContractItemSearchCriteria criteria)
        {
            var result = _derivativeContractSearchService.SearchDerivativeContractItems(criteria);
            return Ok(result);
        }
    }
}
