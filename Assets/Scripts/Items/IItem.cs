using Entities;
using Entities.Player;

namespace Items
{
    public interface IItem
    {
        public void Apply(Player player);
    }
}