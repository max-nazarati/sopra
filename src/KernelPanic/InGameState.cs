using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelPanic
{
    class InGameState
    {
        
        public Camera2D Camera { get; set; }
        public int SaveSlot { get; set; }
        //public SaveGame CurrentSaveGame { get; private set; }
        public InGameState(GameStateManager mngr)
        {

        }
    }
}
