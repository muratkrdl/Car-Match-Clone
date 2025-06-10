using Runtime.Objects;

namespace Runtime.Systems.ObjectPool
{
    public class CarObjectObjPool : BaseObjectPool<CarObject,CarObjectObjPool>
    {
        protected override void OnRelease(CarObject obj)
        {
            base.OnRelease(obj);
            obj.transform.SetParent(transform);
        }
    }
}