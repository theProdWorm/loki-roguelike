using Entities.Stats;

namespace Entities.Player
{
    public partial class Player
    {
        private void Start()
        {
            InitializeMovement();
            
            PlayerBaseStats = (PlayerBaseStats) EntityBaseStats;
        }
    }
}