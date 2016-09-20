using System.Threading.Tasks;
using Dropcraft.Common;

namespace Dropcraft.Runtime
{
    public interface IRuntimeEngine
    {
        RuntimeContext RuntimeContext { get; }

        Task Start();
        void Stop();
    }
}