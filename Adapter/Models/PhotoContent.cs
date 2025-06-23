using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adapter.Models
{
    /// <summary>
    /// Модель фото
    /// </summary>
    public class PhotoContent
    {
        /// <summary>
        /// Ссылка на фото или file_id если TG
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Описание фото
        /// </summary>
        public string? Description { get; set; }
    }
}
