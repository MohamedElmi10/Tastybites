using TastyBites.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace TastyBites.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class TastyBitesController : AbpControllerBase
{
    protected TastyBitesController()
    {
        LocalizationResource = typeof(TastyBitesResource);
    }
}
