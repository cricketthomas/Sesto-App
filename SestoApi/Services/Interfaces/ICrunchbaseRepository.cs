using System;
using System.Threading.Tasks;

namespace sesto.api.Services.Interfaces
{
    public interface ICrunchbaseRepository
    {
        Task<string> GetDomain(string name);
    }
}
