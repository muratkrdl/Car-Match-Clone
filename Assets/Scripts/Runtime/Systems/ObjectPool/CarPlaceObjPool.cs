using Runtime.Objects;
using Runtime.Utilities;

namespace Runtime.Systems.ObjectPool
{
    public class CarPlaceObjPool : BaseObjectPool<CarPlaceObject, CarPlaceObjPool>
    {
        protected override void OnGet(CarPlaceObject obj)
        {
            base.OnGet(obj);
            obj.transform.localScale = ConstantsUtilities.One3;
        }

        protected override void OnRelease(CarPlaceObject obj)
        {
            base.OnRelease(obj);
            obj.transform.SetParent(transform);
        }
    }
}