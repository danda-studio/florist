using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Models.Response
{
    /// <summary>
    /// Модель язык интерфейса
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Код языка ("ru")
        /// </summary>
        public string Code { get; set; }
    }
}
