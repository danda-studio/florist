using FloristAI.Application.Language.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Language
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
        public Task<IEnumerable<LanguageModel>> GetLanguageList();
    }
}
