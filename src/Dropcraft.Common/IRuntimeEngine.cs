using System.Threading.Tasks;

namespace Dropcraft.Common
{
    public interface IRuntimeEngine
    {
        RuntimeContext RuntimeContext { get; }

        Task Start();
        void Stop();
    }
}