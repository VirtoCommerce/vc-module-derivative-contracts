using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.DerivativesModule.Core.Model;
using VirtoCommerce.DerivativesModule.Core.Services;

namespace VirtoCommerce.DerivativesModule.Web.Controllers.Api
{
    [RoutePrefix("api/derivatives")]
    public class DerivativeController : ApiController
    {
        private readonly IDerivativeService _derivativeService;
        private readonly IDerivativeSearchService _derivativeSearchService;

        public DerivativeController(IDerivativeService derivativeService, IDerivativeSearchService derivativeSearchService)
        {
            _derivativeService = derivativeService;
            _derivativeSearchService = derivativeSearchService;
        }

        /// <summary>
        /// Search for FulfillmentCenterMapping by AssetEntrySearchCriteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [ResponseType(typeof(DerivativeSearchResult))]
        // [CheckPermission(Permission = PredefinedPermissions.AssetAccess)]
        public IHttpActionResult Search(DerivativeSearchCriteria criteria)
        {
            var result = _derivativeSearchService.SearchDerivatives(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Get Derivatives by IDs
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{ids}")]
        [ResponseType(typeof(Derivative[]))]
        // [CheckPermission(Permission = PredefinedPermissions...)]
        public IHttpActionResult Get(string[] ids)
        {
            var retVal = _derivativeService.GetDerivativesByIds(ids);
            return Ok(retVal.ToArray());
        }

        /// <summary>
        ///  Create new or update existing Derivative
        /// </summary>
        /// <param name="items">Derivatives</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(void))]
        //[CheckPermission(Permission = PredefinedPermissions...)]
        public IHttpActionResult Update(Derivative[] items)
        {
            _derivativeService.SaveDerivatives(items);
            return Ok();
        }

        /// <summary>
        /// Delete Derivatives by IDs
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ResponseType(typeof(void))]
        //[CheckPermission(Permission = PredefinedPermissions...)]
        public IHttpActionResult Delete([FromUri] string[] ids)
        {
            _derivativeService.DeleteDerivatives(ids);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
