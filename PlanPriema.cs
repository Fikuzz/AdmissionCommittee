using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriyemnayaKomissiya
{
    class PlanPriema
    {
        public int Id { get; set; }
        /// <summary>
        /// Ид специальности
        /// </summary>
        public int IdSpec { get; set; }
        /// <summary>
        /// Ид формы обучения
        /// </summary>
        public int IdForm { get; set; }
        /// <summary>
        /// ИД финансирования
        /// </summary>
        public int IdFinance { get; set; }
        /// <summary>
        /// Количество мест
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Количество целевых мест
        /// </summary>
        public int CountCelevihMest { get; set; }
        /// <summary>
        /// Год
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// Код специальности
        /// </summary>
        public string CodeSpec { get; set; }
        /// <summary>
        /// Наименование специальности
        /// </summary>
        public string NameSpec { get; set; }
        /// <summary>
        /// Наименование формы обучения
        /// </summary>
        public string NameForm { get; set; }
        /// <summary>
        /// Наименование финансирование
        /// </summary>
        public string NameFinance { get; set; }
        /// <summary>
        /// Наименование образования
        /// </summary>
        public string NameObrazovaie { get; set; }
        /// <summary>
        /// Принимаются ли сертификаты цт
        /// </summary>
        public bool Ct { get; set; }
        /// <summary>
        /// Количество записей
        /// </summary>
        public int Writes { get; set; }

        public PlanPriema Clone()
        {
            return new PlanPriema
            {
                Id = this.Id,
                IdSpec = this.IdSpec,
                IdForm = this.IdForm,
                IdFinance = this.IdFinance,
                Count = this.Count,
                CountCelevihMest = this.CountCelevihMest,
                Year = this.Year,
                CodeSpec = this.CodeSpec,
                NameSpec = this.NameSpec,
                NameForm = this.NameForm,
                NameFinance = this.NameFinance,
                NameObrazovaie = this.NameObrazovaie,
                Ct = this.Ct,
                Writes = this.Writes
            };
        }
    }
}
