using SShop.Repositories.Common.Interfaces;
using SShop.ViewModels.Catalog.DeliveryMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Repositories.Catalog.DeliveryMethod
{
    public interface IDeliveryMethodRepository : IModifyEntity<DeliveryMethodCreateRequest, DeliveryMethodUpdateRequest, int>,
        IRetrieveEntity<DeliveryMethodViewModel, DeliveryMethodGetPagingRequest, int>
    {
    }
}