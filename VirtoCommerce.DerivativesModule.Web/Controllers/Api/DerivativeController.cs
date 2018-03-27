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

        public DerivativeController(IDerivativeService derivativeService)
        {
            _derivativeService = derivativeService;
        }

        /// <summary>
        /// Get Derivative by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(Derivative))]
        // [CheckPermission(Permission = PredefinedPermissions...)]
        public IHttpActionResult Get(string id)
        {
            var retVal = _derivativeService.GetDerivativesByIds(new[] { id });
            if (retVal?.Any() == true)
            {
                return Ok(retVal.Single());
            }

            return NotFound();
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
