using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindows.Externals;
using Tindows.Externals.Tinder_Objects;

namespace Tindows.Models
{
    /// <summary>
    /// Singleton class for maintaining state for the entire application :)
    /// Copies singleton logic from Template10 convention
    /// </summary>
    class TinderState
    {
        // Singleton
        public static TinderState Instance { get; }

        private Authentication profileInfo;
        public TinderAPI api { get; }

        static TinderState()
        {
            // implement singleton pattern
            Instance = Instance ?? new TinderState();
        }

        private TinderState()
        {
            api = new TinderAPI();
        }
    }



}

