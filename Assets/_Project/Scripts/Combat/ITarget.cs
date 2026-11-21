using Isometric2DGame.Combat.Healths;
using Isometric2DGame.Infrastructure;

namespace Isometric2DGame.Combat
{
    public interface ITarget : IPositionHolder, IDamageable, IHealthOwner
    {
    }
}
