using Runtime.Objects;
using Runtime.Utilities;

namespace Runtime.Systems.ObjectPool
{
    public class CarObjectObjPool : BaseObjectPool<CarObject,CarObjectObjPool>
    {
        protected override void OnGet(CarObject obj)
        {
            base.OnGet(obj);
            obj.transform.localScale = ConstantsUtilities.One3;
        }

        protected override void OnRelease(CarObject obj)
        {
            base.OnRelease(obj);
            obj.transform.SetParent(transform);
        }
    }
}