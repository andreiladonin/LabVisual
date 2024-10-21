using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabVisual
{
    static class MBVReader
    {
        public static Bitmap ReadMBVFile(string filePath)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    // Чтение ширины и высоты изображения (по 2 байта)
                    ushort width = reader.ReadUInt16();
                    ushort height = reader.ReadUInt16();

                    //Console.WriteLine($"Ширина: {width}, Высота: {height}");

                    // Создание объекта Bitmap
                    Bitmap bitmap = new Bitmap(width, height);

                    // Чтение пикселей и заполнение Bitmap
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Чтение 2 байт для каждого пикселя
                            ushort pixelData = reader.ReadUInt16();

                            // Извлечение яркости (младшие 10 бит)
                            ushort brightness = (ushort)(pixelData & 1023); // Маска для 10 младших бит

                            brightness >>= 0;
                            // Преобразование яркости к диапазону 0-255 (поскольку 10 бит дает значение от 0 до 1023)
                            byte grayValue = (byte)(brightness);

                            // Устанавливаем цвет пикселя (оттенок серого)
                            Color pixelColor = Color.FromArgb(grayValue, grayValue, grayValue);

                            // Устанавливаем пиксель в Bitmap
                            bitmap.SetPixel(x, y, pixelColor);
                        }
                    }

                    return bitmap; // Возвращаем созданное изображение
                }
            }
            catch (Exception ex)
            {
               //  Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                return null;
            }
        }
    }
}
