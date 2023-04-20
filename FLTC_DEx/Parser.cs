using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLTC_DEx
{
    /// <summary>
    /// Класс, реализующий разбор строки запроса
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Копия экземпляра входной строки
        /// </summary>
        private string InpStr = string.Empty;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="inputString"></param>
        public Parser(string inputString)
        {
            //Приведение строки к верхнему регистру
            InpStr = inputString.ToUpper();
        }

        /// <summary>
        /// Парсинг команды, принятой из SerialPort
        /// </summary>
        /// <returns></returns>
        public string Parse(ref UInt32 out_value)
        {
            if (InpStr.IndexOf("GETPORT") == 0)
            {
                return ParseGetPortCommand(ref out_value);
            }

            if (InpStr.IndexOf("GETADC") == 0)
            {
                return ParseGetADCCommand(ref out_value);
            }

            if (InpStr.IndexOf("SETPORT") == 0)
            {
                return ParseSetPortCommand(ref out_value);
            }

            if (InpStr.IndexOf("0X") == 0)
            {
                return ParseHexCommand(ref out_value);
            }

            if (InpStr.IndexOf("0B") == 0)
            {
                return ParseBinaryCommand(ref out_value);
            }

            return "Команда не реализована";
        }

        /// <summary>
        /// Парсинг команды - GetPort
        /// </summary>
        /// <param name="out_value"></param>
        /// <returns></returns>
        string ParseGetPortCommand(ref UInt32 out_value)
        {
            //Разделение запроса на аргументы
            var args = InpStr.Split(',');

            //Если недостаточно аргументов
            if(args.Length < 2)
            {
                return $"Команда: {InpStr} - недостаточно аргументов";
            }

            //Преобразование строки в байт
            byte port_num = 0;
            var result_conversion = Byte.TryParse(args[1], out port_num);

            //Вывод ошибки
            if (result_conversion == false)
            {
                return "Строка запроса имеет неверный формат";
            }

            //Приведение запроса к типу UInt32
            byte[] bts = new byte[]
            {
                port_num, 0x00, 0x00, 0x10
            };

            //Присвоение значения
            out_value = BitConverter.ToUInt32(bts, 0);

            //Для отладки
            var x = out_value.ToString("X4");

            //Ответ об успешном выполнении команды
            return "Done";
        }

        /// <summary>
        /// Парсинг команды -GetADC
        /// </summary>
        /// <param name="out_value"></param>
        /// <returns></returns>
        string ParseGetADCCommand(ref UInt32 out_value)
        {
            //Разделение запроса на аргументы
            var args = InpStr.Split(',');

            //Если недостаточно аргументов
            if (args.Length < 2)
            {
                return $"Команда: {InpStr} - недостаточно аргументов";
            }

            //Преобразование строки в байт
            byte ch_num = 0;
            var result_conversion = Byte.TryParse(args[1], out ch_num);

            //Вывод ошибки
            if (result_conversion == false)
            {
                return "Строка запроса имеет неверный формат";
            }

            //Приведение запроса к типу UInt32
            byte[] bts = new byte[]
            {
                ch_num, 0x00, 0x00, 0x20
            };

            //Присвоение значения
            out_value = BitConverter.ToUInt32(bts, 0);

            //Для отладки
            var x = out_value.ToString("X4");

            //Ответ об успешном выполнении команды
            return "Done";
        }

        /// <summary>
        /// Парсинг команды -SetPort
        /// </summary>
        /// <param name="out_value"></param>
        /// <returns></returns>
        string ParseSetPortCommand(ref UInt32 out_value)
        {
            //Разделение запроса на аргументы
            var args = InpStr.Split(',');

            //Если недостаточно аргументов
            if (args.Length < 3)
            {
                return $"Команда: {InpStr} - недостаточно аргументов";
            }

            //Объявление локальных переменных
            byte port_num = 0, pin_num = 0;

            //Преобразование строки в байт
            var result_conversion = Byte.TryParse(args[1], out port_num);
                result_conversion &= Byte.TryParse(args[2], out pin_num);

            //Вывод ошибки
            if (result_conversion == false)
            {
                return "Строка запроса имеет неверный формат";
            }

            //Приведение запроса к типу UInt32
            byte[] bts = new byte[]
            {
                pin_num, port_num, 0x00, 0x30
            };

            //Присвоение значения
            out_value = BitConverter.ToUInt32(bts, 0);

            //Для отладки
            var x = out_value.ToString("X4");

            //Ответ об успешном выполнении команды
            return "Done";
        }

        /// <summary>
        /// Парсинг команды -отправить HEX
        /// </summary>
        /// <param name="out_value"></param>
        /// <returns></returns>
        string ParseHexCommand(ref UInt32 out_value)
        {
            //Разделение запроса на аргументы
            var args = InpStr.Split('X');

            //Если недостаточно аргументов
            if (args.Length < 2)
            {
                return $"Команда: {InpStr} - недостаточно аргументов";
            }

            //Удаление из строки всех пробелов
            var hexstr = args[1].Replace(" ", "");

            //Приведение запроса к типу UInt32
            try
            {
                out_value = uint.Parse(hexstr, System.Globalization.NumberStyles.HexNumber);
                
                //Для отладки
                var x = out_value.ToString("X4");
            }
            catch(Exception ex)
            {
                return "Строка запроса имеет неверный формат";
            }

            //Ответ об успешном выполнении команды
            return "Done";
        }

        /// <summary>
        /// Парсинг команды -отправить BIN
        /// </summary>
        /// <param name="out_value"></param>
        /// <returns></returns>
        string ParseBinaryCommand(ref UInt32 out_value)
        {
            //Разделение запроса на аргументы
            var args = InpStr.Split('B');

            //Если недостаточно аргументов
            if (args.Length < 2)
            {
                return $"Команда: {InpStr} - недостаточно аргументов";
            }

            //Удаление из строки всех пробелов
            var binstr = args[1].Replace(" ", "");

            //Приведение запроса к типу UInt32
            try
            {
                out_value = Convert.ToUInt32(binstr, 2);
                var x = out_value.ToString("X4");
            }
            catch(Exception ex)
            {
                return "Строка запроса имеет неверный формат";
            }

            //Ответ об успешном выполнении команды
            return "Done";
        }
    }
}
