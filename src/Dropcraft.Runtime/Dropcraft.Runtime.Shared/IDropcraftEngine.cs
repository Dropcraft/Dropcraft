using System.Threading.Tasks;
using Dropcraft.Contracts;

namespace Dropcraft.Runtime
{
    public interface IDropcraftEngine
    {
        RuntimeContext RuntimeContext { get; }

        Task Start();
        void Stop();
    }
}