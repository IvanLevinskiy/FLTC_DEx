using System;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace FLTC_DEx
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Экземпляр последовательного порта
        /// </summary>
        SerialPort _serialport = new SerialPort();

        /// <summary>
        /// Метод устанавливает исходное состояние
        /// элементам управления
        /// </summary>
        void InitializeUI()
        {
            InitializeComponent();

            cmbMacrosList.SelectedIndex = 0;

            //cmbAvailablesSerialPorts.Items.Clear();
            //cmbAvailablesSerialPorts.Items.AddRange(SerialPort.GetPortNames());

            //if (cmbAvailablesSerialPorts.Items.Count > 0)
            //{
               cmbAvailablesSerialPorts.SelectedIndex = 0;
            //}

            btnSend.Enabled = true;

            _serialport.DataReceived += _serialport_DataReceived;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Form1()
        {
            //Инициализация UI
            InitializeUI();
        }

        /// <summary>
        /// Событие, вызываемое при нажатии на кнопку Открыть - Закрыть Serial Port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSerialPortOpen_Click(object sender, EventArgs e)
        {
            try
            {
                //Если SerialPort открыт
                if (_serialport.IsOpen == true)
                {
                    _serialport.Close();
                    btnSend.Enabled = false;
                    btnSerialPortOpen.Text = "Открыть";
                    cmbAvailablesSerialPorts.Enabled = true;
                    return;
                }

                //Если SerialPort закрыт
                _serialport.PortName = cmbAvailablesSerialPorts.Text;
                _serialport.BaudRate = 115200;
                _serialport.Open();
                cmbAvailablesSerialPorts.Enabled = false;
                btnSend.Enabled = true;
                btnSerialPortOpen.Text = "Закрыть";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка операции с SerialPort", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        /// <summary>
        /// Передача запроса из макроса в строку запроса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMacrosAccept_Click(object sender, EventArgs e)
        {
            tbxQuery.Text = cmbMacrosList.Text;
        }

        /// <summary>
        /// Событие, генерируемое при нажатии на кнопку - ОТПРАВИТЬ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            //Парсинг команды
            UInt32 out_data = 0;
            var parser = new Parser(tbxQuery.Text);
            var msg = parser.Parse(ref out_data);

            //Цвет текста в консоли по умолчанию красный
            var color = Color.Red;

            //Если ответ от парсера - "Done" - отправляем данные 
            //в SerialPort
            if (msg == "Done")
            {
                if(_serialport.IsOpen == true)
                {
                    _serialport.Write(BitConverter.GetBytes(out_data), 0, 4);
                }

                //Если статус Done - цвет надписи синий
                color = Color.Blue;
            }

            //Определение длины текста в консоли
            int rtblenght = rtbConsole.Text.Length;

            //Добавление статуса в консоль
            rtbConsole.AppendText($">> {tbxQuery.Text}  - {msg}\n");

            //Задание цвету текста синего цвета
            rtbConsole.Select(rtblenght, rtbConsole.Text.Length - rtblenght);
            rtbConsole.SelectionColor = color;
        }

        /// <summary>
        /// Событие, вызываемое при приеме данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _serialport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Чтение данных
            Thread.Sleep(50);
            byte[] bytes = new byte[_serialport.BytesToRead];
            _serialport.Read(bytes, 0, bytes.Length);

            //Получение значения из SerialPort
            UInt32 value = BitConverter.ToUInt32(bytes, 0);

            Action action = () =>
            {
                //Определение длины консоли
                int rtblenght = rtbConsole.Text.Length;

                //Добавление статуса в консоль
                rtbConsole.AppendText($"<< {value.ToString("X4")}\n");

                //Задание цвету текста зеленого цвета
                rtbConsole.Select(rtblenght, rtbConsole.Text.Length - rtblenght);
                rtbConsole.SelectionColor = Color.Green;
            };
            this.Invoke(action);
        }

        /// <summary>
        /// Прокрутка rtbConsole в конец
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtbConsole_TextChanged(object sender, EventArgs e)
        {
            rtbConsole.SelectionStart = rtbConsole.Text.Length;
            rtbConsole.ScrollToCaret();
        }

        /// <summary>
        /// Метод для очистки конссоли
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbConsole.Clear();
        }
    }
}
