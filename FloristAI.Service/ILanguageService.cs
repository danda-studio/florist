using FloristAI.Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application
{
    /// <summary>
    /// Интерфейс сервиса языков
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>
        /// Метод для получения списка языков
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Language>> GetLanguageList();
    }
}
