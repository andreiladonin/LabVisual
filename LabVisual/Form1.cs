using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LabVisual
{
    public partial class Form1 : Form
    {
        // Поле для хранения исходного изображения
        Bitmap imageOriginal = null;

        public Form1()
        {
            InitializeComponent();

            // Настройка PictureBox для автоматического размера изображения
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            // Включаем прокрутку панели, содержащей PictureBox
            panel1.AutoScroll = true;
            panel1.Controls.Add(pictureBox1);

            // Устанавливаем выбранный RadioButton по умолчанию
            radioButton1.Checked = true;

            // Подписываемся на события изменения состояния RadioButton
            radioButton1.CheckedChanged += RadioButton_CheckedChanged;
            radioButton2.CheckedChanged += RadioButton_CheckedChanged;
            radioButton3.CheckedChanged += RadioButton_CheckedChanged;

            // Подписываемся на событие изменения текста в TextBox для выбора строки изображения
            textBox1.TextChanged += TextBox1_TextChanged;

            // Подписываемся на событие движения мыши по PictureBox
            pictureBox1.MouseMove += PictureBox1_MouseMove;
        }

        // Обработчик события изменения состояния RadioButton
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ApplyShift();           // Применяем сдвиг яркости, выбранный пользователем
            ShowImageFragment();    // Обновляем видимый фрагмент изображения
        }

        // Обработчик события нажатия кнопки для загрузки изображения
        private void button1_Click(object sender, EventArgs e)
        {
            // Открываем диалоговое окно для выбора файла
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            // Получаем путь к выбранному файлу и отображаем его в textBox4
            string filename = openFileDialog1.FileName;
            textBox4.Text = filename;

            // Читаем и отображаем изображение из MBV файла
            ApplyShift();
            ShowImageFragment(); // Отображаем начальный фрагмент изображения
        }

        // Метод для применения сдвига яркости в зависимости от выбранного RadioButton
        private void ApplyShift()
        {
            // Определяем сдвиг по умолчанию
            ushort shift = 0;
            if (radioButton1.Checked) shift = 0;
            else if (radioButton2.Checked) shift = 1;
            else shift = 2;

            try
            {
                // Читаем изображение из файла с помощью BinaryReader
                using (BinaryReader reader = new BinaryReader(File.Open(textBox4.Text, FileMode.Open)))
                {
                    // Чтение ширины и высоты изображения
                    ushort width = reader.ReadUInt16();
                    ushort height = reader.ReadUInt16();

                    // Создаем Bitmap с указанными шириной и высотой
                    Bitmap bitmap = new Bitmap(width, height);

                    // Проходим по всем пикселям изображения
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Читаем данные пикселя (2 байта)
                            ushort pixelData = reader.ReadUInt16();

                            // Извлекаем яркость, используя маску на 10 младших бит
                            ushort brightness = (ushort)(pixelData & 1023);

                            // Применяем сдвиг яркости
                            brightness >>= shift;

                            // Конвертируем значение яркости в оттенок серого (0-255)
                            byte grayValue = (byte)(brightness);

                            // Создаем цвет пикселя
                            Color pixelColor = Color.FromArgb(grayValue, grayValue, grayValue);

                            // Устанавливаем цвет пикселя в Bitmap
                            bitmap.SetPixel(x, y, pixelColor);
                        }
                    }

                    // Сохраняем изображение в поле и отображаем его в PictureBox
                    imageOriginal = bitmap;
                    pictureBox1.Image = bitmap;
                }
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при проблемах с чтением файла
                MessageBox.Show("Ошибка чтения файла");
            }
        }

        // Метод для отображения фрагмента изображения, начиная с указанной строки
        private void ShowImageFragment()
        {
            // Проверяем, что в TextBox введено корректное число и что изображение загружено
            if (int.TryParse(textBox1.Text, out int rowNumber) && imageOriginal != null)
            {
                int imageHeight = imageOriginal.Height;

                // Проверяем, что введенный номер строки находится в пределах изображения
                if (rowNumber >= 0 && rowNumber < imageHeight)
                {
                    // Прокручиваем `panel1` так, чтобы начальная строка была видна сверху
                    panel1.AutoScrollPosition = new Point(0, rowNumber);
                }
                else
                {
                    // Если номер строки вне диапазона, выводим сообщение
                    MessageBox.Show("Номер строки вне диапазона изображения.");
                }
            }
            else
            {
                // Если введено не число, выводим сообщение
                MessageBox.Show("Введите корректный номер строки.");
            }
        }

        // Обработчик события изменения текста в TextBox
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            ShowImageFragment(); // Обновляем фрагмент изображения при изменении текста
        }

        // Обработчик события движения мыши по PictureBox
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Получаем координаты курсора внутри PictureBox
            int mouseX = e.X;
            int mouseY = e.Y;

            // Проверяем, что курсор находится в пределах изображения
            if (imageOriginal != null && mouseX >= 0 && mouseX < imageOriginal.Width && mouseY >= 0 && mouseY < imageOriginal.Height)
            {
                // Получаем цвет пикселя под курсором
                Color pixelColor = imageOriginal.GetPixel(mouseX, mouseY);

                // Вычисляем яркость пикселя как среднее значение каналов R, G и B
                int brightness = (int)((pixelColor.R / 3));

                // Отображаем координаты и яркость пикселя в текстовых полях
                textBoxX.Text = mouseX.ToString();
                textBoxY.Text = mouseY.ToString();
                textBoxBrightness.Text = brightness.ToString();
            }
        }
    }

}
