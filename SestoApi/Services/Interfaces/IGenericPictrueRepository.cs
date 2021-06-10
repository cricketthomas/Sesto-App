using System;
using System.Threading.Tasks;
using PexelsDotNetSDK.Models;
using sesto.api.Static;

namespace sesto.api.Services.Interfaces
{
    public interface IGenericPictrueRepository
    {
        Emoji GetEmoji(string name, string placeType);
        Task<string> GetFirstPexelImage(string[] searchTypes);
    }
}
