using Sys = Cosmos.System;

namespace Pandora.Applets
{
    class PowerManager //very simple - just allows access to the shutdown and reboot functions.
    {
        public void Reboot(string[] argv) { Sys.Power.Reboot(); }
        public void Shutdown(string[] argv) { Sys.Power.Shutdown(); }
    }
}
