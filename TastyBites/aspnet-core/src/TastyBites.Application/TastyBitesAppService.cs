using System;
using System.Collections.Generic;
using System.Text;
using TastyBites.Localization;
using Volo.Abp.Application.Services;

namespace TastyBites;

/* Inherit your application services from this class.
 */
public abstract class TastyBitesAppService : ApplicationService
{
    protected TastyBitesAppService()
    {
        LocalizationResource = typeof(TastyBitesResource);
    }
}
