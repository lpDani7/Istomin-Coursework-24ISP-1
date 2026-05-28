using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Курсовая_работа___Игра_Жизнь_Дж.Конвея
{
    public partial class Form1 : Form
    {
        bool[,] today; //Массив значений популяции в данном поколении
        bool[,] nextDay; //Массив значений популяции в следующем поколении
        int generalCountIndivid = 0; //Общее число особей за весь период
        int averageCountIndivid = 0; //Среднее количество особей за весь период
        int numGen = 0; //Номер поколения
        int countIndivid = 0; //Количество особей
        int x = 0; //Значении ширины мира по умолчанию
        int y = 0; //Значении высоты мира по умолчанию
        private List<PointF> points; // Список точек для графика
        private int pointCounter = 0; // Счетчик добавленных точек

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "0"; //Значение по умолчанию для поля "Ширина"
            textBox2.Text = "0"; //Значение по умолчанию для поля "Высота"
            textBox4.Text = "0"; //Значение по умолчанию для поля "Количество живых клеток"
            textBox3.Text = "0"; //Значение по умолчанию для поля "Количество поколений"
            textBox7.Text = "0"; //Значение по умолчанию для поля "Поколение N№"
            points = new List<PointF>(); // Инициализация списка точек
        }

        //Кнопка "Создать мир"
        private void button4_Click(object sender, EventArgs e)
        {
            x = int.Parse(textBox1.Text); //Значении ширины мира(берётся из поля "Ширина")
            y = int.Parse(textBox2.Text); //Значении высоты мира(берётся из поля "Высота")
            int size = 13; //Размер одной клетки мира по умолчанию
            //Адаптивное изменение размера клеток мира исходя из его размеров
            if (x <= 10 && y <= 10) size = 67;
            else if (x <= 20 && y <= 20) size = 33;
            else if (x <= 30 && y <= 30) size = 22;
            else if (x <= 40 && y <= 40) size = 16;
            else if (x <= 50 && y <= 50) size = 13;
            //Предупреждение о превышении максимального размера мира и установка размеров мира на максимально допустимое значение
            else
            {
                MessageBox.Show("Ограничение мира 50х50!");
                x = 50;
                y = 50;
                textBox1.Text = "50";
                textBox2.Text = "50";
            }
            if (x==0)
            {
                MessageBox.Show("Минимальная ширина мира 1!");
                x = 1;
                textBox1.Text = "1";
            }
            if (y == 0)
            {
                MessageBox.Show("Минимальная высота мира 1!");
                y = 1;
                textBox2.Text = "1";
            }
            dataGridView1.ColumnCount = x; //Установка кол-ва столбцов таблицы(мира)
            dataGridView1.RowCount = y; //Установка кол-ва строк таблицы(мира)
            today = new bool[x, y]; //Массив значений популяции в данном поколении размерностью X*Y
            nextDay = new bool[x, y]; //Массив значений популяции в следующем поколении размерностью X*Y
            //Цикл создания мира
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    nextDay[i, j] = false; // начальное состояние — «пустое»
                    dataGridView1[i,j].Style.BackColor = Color.White; //Перекрашивает клетки в белый
                    countIndivid = 0; //Обнуляет кол-во особей
                    textBox4.Text = countIndivid.ToString(); //Обновляет значение поля "Количество осбей"
                    dataGridView1.Columns[i].Width = size; //Задание размеров столбца
                    dataGridView1.Rows[j].Height = size; //Задание размеров строки
                }
            }
        }

        //Действия, происходящие при нажатии мышью на клетку мира
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Если клетка не зелёная, то она красится в зелёный и её значение в массиве становится живым(True), а также прибавляется кол-во живых клеток
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor != Color.Green)
            {
                countIndivid = int.Parse(textBox4.Text);
                countIndivid++;
                textBox4.Text = countIndivid.ToString();
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Green;
                today[e.RowIndex, e.ColumnIndex] = true;
            }
            //Иначе клетка красится в белый и её значение в массиве становится мёртвым(False), а также обовляется кол-во живых клеток
            else
            {
                countIndivid = int.Parse(textBox4.Text);
                countIndivid--;
                textBox4.Text = countIndivid.ToString();
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                today[e.RowIndex, e.ColumnIndex] = false;
            }
             //Функция чтобы выделенная клетка не подсвечивалась синим
             dataGridView1.ClearSelection();
        }

        //Кнопка "Очистить мир"
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop(); //Останавливает таймер
            int x = int.Parse(textBox1.Text); //Считывает значение ширины с пользовательского поля
            int y = int.Parse(textBox2.Text); //Считывает значение высоты с пользовательского поля
            //Цикл, убивающий живые клетки и перекрашивающий их в белый
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (today[i, j]) today[i, j] = false;
                    if (dataGridView1 [i, j].Style.BackColor == Color.Green) dataGridView1[i,j].Style.BackColor = Color.White;
                }
            }
            textBox4.Text = "0"; //Обнуляет значение для поля "Количество живых клеток"
            textBox7.Text = "0"; //Обнуляет значение для поля "Поколение N№"
            numGen = 0; //Обнуляет номер поколения
            textBox5.Text = "0"; //Обнуляет значение для поля "Общее число особей за весь период"
            generalCountIndivid = 0; //Обнуляет общее число особей за весь период
            textBox6.Text = "0"; //Обнуляет значение для поля "Среднее количество особей за весь период"
            averageCountIndivid = 0; //Обнуляет среднее количество особей за весь период
        }

        //Кнопка "Расставить автоматически"
        private void button1_Click(object sender, EventArgs e)
        {
            int x = int.Parse(textBox1.Text);
            int y = int.Parse(textBox2.Text);
            int i, j;
            int countIndivid = int.Parse(textBox4.Text); //Берёт кол-во особей из пользовательского поля
            Random rnd = new Random();
            //Цикл, убивающий живые клетки и перекрашивающий их в белый
            for ( i = 0; i < x; i++)
            {
                for ( j = 0; j < y; j++)
                {
                    if (today[i, j])
                    {
                        today[i, j] = false;
                        dataGridView1[i, j].Style.BackColor = Color.White;
                    }
                }
            }
            //Цикл, заполняющий поле живыми клетками в случайных местах
            for ( i = 0; i < countIndivid; i++)
            {
                int Rx = rnd.Next(0, x); //Генерация случейного значения по X
                int Ry = rnd.Next(0, y); //Генерация случейного значения по Y
                today[Rx, Ry] = true;
                dataGridView1[Rx, Ry].Style.BackColor = Color.Green;
            }
            //textBox4.Text = countIndivid.ToString(); //Обновление данных о кол-ве особей
        }

        //Кнопка "Старт"
        private void button3_Click(object sender, EventArgs e)
        {
            int countGen = int.Parse(textBox3.Text); //Считывается кол-во поколений из поля "Количество поколений"
            //Предупреждение о превышении максимального числа поколений и установка числа поколений на максимально допустимое значение
            if (countGen > 100)
            {
                MessageBox.Show("Ограничение числа поколений 100!");
                countGen = 100;
                textBox3.Text = "100";
            }
            timer1.Start(); //Запускает таймер
        }

        //Действия, происходящие при каждом тике таймера
        private void timer1_Tick(object sender, EventArgs e)
        {   
            int countGen = int.Parse(textBox3.Text); //Считывается кол-во поколений из поля "Количество поколений"
            //Если номер поколения меньше числа поколений, то:
            if (numGen < countGen)
            {
                x = int.Parse(textBox1.Text);
                y = int.Parse(textBox2.Text);
                int countIndivid = 0; //Обнуляется число особей
                for (int i = 1; i < x - 1; i++)
                {
                    for (int j = 1; j < y - 1; j++)
                    {
                        bool isAlive = today[i, j]; //Запоминается значение текущей клетки для проверки на то, что она жива
                        int numNeigbours = 0; //Кол-во живых соседей
                        if (today[i - 1, j - 1]) numNeigbours++; //Если сосед Слева-Сверху жив, то +1 сосед
                        if (today[i - 1, j]) numNeigbours++; //Сверху
                        if (today[i - 1, j + 1]) numNeigbours++; //Справа_Сверху
                        if (today[i, j - 1]) numNeigbours++; //Слева
                        if (today[i, j + 1]) numNeigbours++; //Справа
                        if (today[i + 1, j - 1]) numNeigbours++; //Слева-Снизу
                        if (today[i + 1, j]) numNeigbours++; //Снизу
                        if (today[i + 1, j + 1]) numNeigbours++; //Справа-Снизу
                        bool keepAlive = isAlive && (numNeigbours == 2 || numNeigbours == 3); //Проверка на то, что живая клетка останется живой
                        bool makeNewLive = !isAlive && numNeigbours == 3; //Проверка на то, что мёртвая клетка станет живой
                        nextDay[i, j] = keepAlive || makeNewLive; //Запись значения клетки в массив следующего поколения
                        //Перекрашивание клеток в зависимости от её состояния в новом поколении
                        if (nextDay[i, j])
                        {
                            dataGridView1[i, j].Style.BackColor = Color.Green;

                        }
                        else
                        {
                            dataGridView1[i, j].Style.BackColor = Color.White;

                        }
                    }
                }
                //Значения массива следующего поколения записываются в нынешнее поколение(смена поколений)
                for (int i = 1; i < x - 1; i++)
                {
                    for (int j = 1; j < y - 1; j++)
                    {
                        today[i, j] = nextDay[i, j];
                    }
                }
                //Пересчитывается количество особей по результатам смены поколений
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        if (today[i, j]) countIndivid++;
                    }
                }
                textBox4.Text = countIndivid.ToString(); //В поле "Количество особей" записывается актуальное значение
                generalCountIndivid += countIndivid; //Обновляется общее количество особей за весь период
                textBox5.Text = generalCountIndivid.ToString(); //В поле "Общее количество особей за весь период" записывается актуальное значение
                numGen++; //Прибавляется 1 к номеру поколения
                textBox7.Text = numGen.ToString(); //В поле "Поколение N№" записывается актуальное значение
                averageCountIndivid = generalCountIndivid / numGen; //Высчитывается среднее количество особей за весь период
                textBox6.Text = averageCountIndivid.ToString(); //В поле "Среднее количество особей за весь период" записывается актуальное значение
                int count = countGen+1;
                double[] CartX = new double[count];
                double[] ChartYPop = new double[count];
                double[] ChartYBrate = new double[count];
                double[] ChartYMrate = new double[count];
                for (int i = 0; i < count; i++)
                {

                    CartX[i] = 0 + 1 * i;
                    ChartYPop[i] = countIndivid;
                }
                //chart1.Series[0].Points.Add(new PointF(pointCounter, rnd.Next(100)));
                //pointCounter++;
                //chart1.Series.Points.AddY(rnd.Next(100));
                
                chart1.ChartAreas[0].AxisX.Minimum = 0;
                chart1.ChartAreas[0].AxisX.Maximum = countGen;
                chart1.ChartAreas[0].AxisX.MajorGrid.Interval = 1;
                chart1.Series[0].Points.DataBindXY(CartX, ChartYPop);
                chart1.Series[0].Points.Add(countGen, countIndivid);
                Invalidate();// Перерисовка графика
            }
            //Иначе останавливает таймер
            else timer1.Stop();
        }
        //Кнопка "Стоп"
        private void button6_Click(object sender, EventArgs e)
        {
            timer1.Stop();//Останавливает таймер
        }

        //Поле "Ширина"
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || (int)e.KeyChar == 8 ))
                e.KeyChar = (char)0;//Зпрет на ввод любых символов кроме чисел
        }

        //Поле "Высота"
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || (int)e.KeyChar == 8))
                e.KeyChar = (char)0;//Зпрет на ввод любых символов кроме чисел
        }

        //Поле "Количество живых клеток"
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || (int)e.KeyChar == 8))
                e.KeyChar = (char)0;//Зпрет на ввод любых символов кроме чисел
        }

        //Поле "Количество поколений"
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || (int)e.KeyChar == 8))
                e.KeyChar = (char)0;//Зпрет на ввод любых символов кроме чисел
        }

        //Поле "Среднее количество особей за весь период"
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = (char)0;//Зпрет на ввод любых символов
        }

        //Поле "Общее количество особей за весь период"
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = (char)0;//Зпрет на ввод любых символов
        }

        //Поле "Поколение N№"
        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = (char)0;//Зпрет на ввод любых символов
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            
        }
        
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox3_Click(object sender, EventArgs e)
        {
            
        }

        
    }
}
