using Runtime.Objects;
using Runtime.Utilities;

namespace Runtime.Systems.ObjectPool
{
    public class ObstacleObjPool : BaseObjectPool<ObstacleObject, ObstacleObjPool>
    {
        protected override void OnGet(ObstacleObject obj)
        {
            base.OnGet(obj);
            obj.transform.localScale = ConstantsUtilities.One3;
        }

        protected override void OnRelease(ObstacleObject obj)
        {
            base.OnRelease(obj);
            obj.transform.SetParent(transform);
        }
    }
}