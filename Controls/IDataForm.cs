using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PriyemnayaKomissiya.Controls
{
    interface IDataForm
    {
        /// <summary>
		/// Проверка корректности данных на форме
		/// </summary>
		/// <returns>true:если все данные корректны false: если хоть одно поле введено нерорректно</returns>
        bool Validate();
    }
}
