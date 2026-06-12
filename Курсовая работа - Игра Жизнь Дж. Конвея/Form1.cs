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
        int numBirths = 0; //Количество родившихся особей
        int numDeaths = 0; //Количество вымерших особей
        // Для хранения истории
        List<int> liveCellsHistory = new List<int>();
        List<int> birthsCellsHistory = new List<int>();
        List<int> deathsCellsHistory = new List<int>();
        List<int> generationHistory = new List<int>();
        int maxHistoryPoints = 100; // Ограничиваем историю

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "0"; //Значение по умолчанию для поля "Ширина"
            textBox2.Text = "0"; //Значение по умолчанию для поля "Высота"
            textBox4.Text = "0"; //Значение по умолчанию для поля "Количество живых клеток"
            textBox3.Text = "0"; //Значение по умолчанию для поля "Количество поколений"
            textBox7.Text = "0"; //Значение по умолчанию для поля "Поколение N№"
            // Очищаем стандартные серии
            chart1.Series.Clear();
            chart2.Series.Clear();
            chart3.Series.Clear();
            // Создаём серию для живых клеток
            Series liveSeries = new Series("Живые клетки");
            liveSeries.ChartType = SeriesChartType.Line;
            liveSeries.Color = Color.Green;
            liveSeries.BorderWidth = 2;
            liveSeries.MarkerStyle = MarkerStyle.Circle;
            liveSeries.MarkerSize = 5;
            // Создаём серию для родившихся клеток клеток
            Series birthsSeries = new Series("Родившиеся клетки");
            birthsSeries.ChartType = SeriesChartType.Line;
            birthsSeries.Color = Color.Red;
            birthsSeries.BorderWidth = 2;
            birthsSeries.MarkerStyle = MarkerStyle.Circle;
            birthsSeries.MarkerSize = 5;
            // Создаём серию для вымерших клеток
            Series deathsSeries = new Series("Вымершие клетки");
            deathsSeries.ChartType = SeriesChartType.Line;
            deathsSeries.Color = Color.Black;
            deathsSeries.BorderWidth = 2;
            deathsSeries.MarkerStyle = MarkerStyle.Circle;
            deathsSeries.MarkerSize = 5;
            //Добавляем серии в коллекцию
            chart1.Series.Add(liveSeries);
            chart2.Series.Add(birthsSeries);
            chart3.Series.Add(deathsSeries);
            // Настройка осей для графика живых клеток
            chart1.ChartAreas[0].AxisX.Title = "Поколение";
            chart1.ChartAreas[0].AxisY.Title = "Кол-во живых клеток";
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            // Настройка осей для графика родившихся клеток
            chart2.ChartAreas[0].AxisX.Title = "Поколение";
            chart2.ChartAreas[0].AxisY.Title = "Кол-во родившихся клеток";
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisY.Minimum = 0;
            // Настройка осей для графика вымерших клеток
            chart3.ChartAreas[0].AxisX.Title = "Поколение";
            chart3.ChartAreas[0].AxisY.Title = "Кол-во вымерших клеток";
            chart3.ChartAreas[0].AxisX.Minimum = 0;
            chart3.ChartAreas[0].AxisY.Minimum = 0;
            //chart1.ChartAreas[0].AxisY.Maximum = x * y;
            // Легенда
            //chart1.Legends[0].Docking = Docking.Bottom;
            // Заголовок для графика живых клеток
            chart1.Titles.Clear();
            Title liveTitle = new Title("Динамика численности популяции");
            chart1.Titles.Add(liveTitle);
            // Заголовок для графика родившихся клеток
            chart2.Titles.Clear();
            Title birthsTitle = new Title("Динамика рождаемости популяции");
            chart2.Titles.Add(birthsTitle);
            // Заголовок для графика вымерших клеток
            chart3.Titles.Clear();
            Title deathsTitle = new Title("Динамика смертности популяции");
            chart3.Titles.Add(deathsTitle);
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
            x = e.ColumnIndex;
            y = e.RowIndex;
            //Если клетка не зелёная, то она красится в зелёный и её значение в массиве становится живым(True), а также прибавляется кол-во живых клеток
            if (dataGridView1[x, y].Style.BackColor != Color.Green)
            {
                countIndivid = int.Parse(textBox4.Text);
                countIndivid++;
                textBox4.Text = countIndivid.ToString();
                dataGridView1[x, y].Style.BackColor = Color.Green;
                today[x, y] = true;
            }
            //Иначе клетка красится в белый и её значение в массиве становится мёртвым(False), а также обовляется кол-во живых клеток
            else
            {
                countIndivid = int.Parse(textBox4.Text);
                countIndivid--;
                textBox4.Text = countIndivid.ToString();
                dataGridView1[x, y].Style.BackColor = Color.White;
                today[x, y] = false;
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
            numBirths = 0; //Обнуляется количество родившихся особей
            numDeaths = 0; //Обнуляется количество вымерших особей
            // Очищаем серию и добавляем все точки заново
            Series liveSeries = chart1.Series["Живые клетки"];
            liveSeries.Points.Clear();
            // Очищаем серию и добавляем все точки заново
            Series birthsSeries = chart2.Series["Родившиеся клетки"];
            birthsSeries.Points.Clear();
            // Очищаем серию и добавляем все точки заново
            Series deathsSeries = chart3.Series["Вымершие клетки"];
            deathsSeries.Points.Clear();
            //Очищаем историю
            liveCellsHistory.Clear();
            birthsCellsHistory.Clear();
            deathsCellsHistory.Clear();
            generationHistory.Clear();
        }

        //Кнопка "Расставить автоматически"
        private void button1_Click(object sender, EventArgs e)
        {
            int x = int.Parse(textBox1.Text);
            int y = int.Parse(textBox2.Text);
            int i, j;
            countIndivid = int.Parse(textBox4.Text); //Берёт кол-во особей из пользовательского поля
            int maxCell = x * y;
            //Предупреждение о том, что мир ещё не создан
            if (dataGridView1.ColumnCount == 0 || dataGridView1.RowCount == 0) MessageBox.Show("Сначала создайте мир!");
            else if (countIndivid > maxCell)
            {
                MessageBox.Show("Ограничение числа живых клеток " + x * y + "!");
                textBox4.Text = ""+maxCell;
                countIndivid = maxCell;
            }
            else if (countIndivid == 0) MessageBox.Show("Невозможно расставить 0 клеток!");
            // Алгоритм очистки поля и расстановки клеток
            else
            {
                Random rnd = new Random();
                int placed = 0; // Счётчик кол-ва заполненых клеток
                int attempts = 0; // Кол-во попыток на заполнение без повторений
                int maxAttempts = maxCell * 8; // Защита от бесконечного цикла
                //Цикл, убивающий живые клетки и перекрашивающий их в белый
                for (i = 0; i < x; i++)
                {
                    for (j = 0; j < y; j++)
                    {
                        if (today[i, j])
                        {
                            today[i, j] = false;
                            dataGridView1[i, j].Style.BackColor = Color.White;
                        }
                    }
                }
                //Цикл, заполняющий поле живыми клетками в случайных местах
                while (placed < countIndivid && attempts < maxAttempts)
                {
                    int Rx = rnd.Next(0, x);
                    int Ry = rnd.Next(0, y);

                    if (!today[Rx, Ry])
                    {
                        today[Rx, Ry] = true;
                        dataGridView1[Rx, Ry].Style.BackColor = Color.Green;
                        placed++;
                    }
                    attempts++;
                }
            }
        }

        //Кнопка "Старт"
        private void button3_Click(object sender, EventArgs e)
        {
            int countGen = int.Parse(textBox3.Text); //Считывается кол-во поколений из поля "Количество поколений"
            int x = int.Parse(textBox1.Text);//Считывает значение ширины с пользовательского поля
            int y = int.Parse(textBox2.Text); //Считывает значение высоты с пользовательского поля
            int countInd = int.Parse(textBox4.Text); //Считывается кол-во живых клеток из поля "Количество живых клеток"
            //Предупреждение о том, что мир ещё не создан
            if (dataGridView1.ColumnCount==0 || dataGridView1.RowCount==0) MessageBox.Show("Сначала создайте мир!");
            //Предупреждение о том, что не все поля заполнены и программа не может начать работу
            else if (countGen == 0 || x == 0 || y == 0 || countInd == 0) MessageBox.Show("Заполните все пользовательские поля!");
            //Предупреждение о том, что на поле нет ни одной живой клетки
            else if (countIndivid==0) MessageBox.Show("Установите хотябы одну живую клетку!");
            //Предупреждение о превышении максимального числа поколений и установка числа поколений на максимально допустимое значение
            else if (countGen > 100)
                {
                    MessageBox.Show("Ограничение числа поколений 100!");
                    countGen = 100;
                    textBox3.Text = "100";
                }
             else timer1.Start(); //Запускает таймер
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
                        if (makeNewLive) numBirths++; //Подсчёт кол-ва родившихся особей
                        if (today[i, j] && !nextDay[i, j]) numDeaths++; //Подсчёт кол-ва вымерших особей
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
                // Добавляем в историю
                liveCellsHistory.Add(countIndivid);
                birthsCellsHistory.Add(numBirths);
                deathsCellsHistory.Add(numDeaths);
                generationHistory.Add(numGen);
                // Ограничиваем количество точек для производительности
                if (liveCellsHistory.Count > maxHistoryPoints)
                {
                    liveCellsHistory.RemoveAt(0);
                    generationHistory.RemoveAt(0);
                }
                // Ограничиваем количество точек для производительности
                if (birthsCellsHistory.Count > maxHistoryPoints)
                {
                    birthsCellsHistory.RemoveAt(0);
                    generationHistory.RemoveAt(0);
                }
                // Ограничиваем количество точек для производительности
                if (deathsCellsHistory.Count > maxHistoryPoints)
                {
                    deathsCellsHistory.RemoveAt(0);
                    generationHistory.RemoveAt(0);
                }
                // Очищаем серию и добавляем все точки заново
                Series liveSeries = chart1.Series["Живые клетки"];
                liveSeries.Points.Clear();

                for (int i = 0; i < generationHistory.Count; i++)
                {
                    liveSeries.Points.AddXY(generationHistory[i], liveCellsHistory[i]);
                }
                // Очищаем серию и добавляем все точки заново
                Series birthsSeries = chart2.Series["Родившиеся клетки"];
                birthsSeries.Points.Clear();

                for (int i = 0; i < generationHistory.Count; i++)
                {
                    birthsSeries.Points.AddXY(generationHistory[i], birthsCellsHistory[i]);
                }
                // Очищаем серию и добавляем все точки заново
                Series deathsSeries = chart3.Series["Вымершие клетки"];
                deathsSeries.Points.Clear();

                for (int i = 0; i < generationHistory.Count; i++)
                {
                    deathsSeries.Points.AddXY(generationHistory[i], deathsCellsHistory[i]);
                }
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
            if (textBox1.Text.Length > 5) e.KeyChar = (char)0;
        }

        //Поле "Высота"
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || (int)e.KeyChar == 8))
                e.KeyChar = (char)0;//Зпрет на ввод любых символов кроме чисел
            if (textBox2.Text.Length > 5) e.KeyChar = (char)0;
        }

        //Поле "Количество живых клеток"
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || (int)e.KeyChar == 8))
                e.KeyChar = (char)0;//Зпрет на ввод любых символов кроме чисел
            if (textBox4.Text.Length > 5) e.KeyChar = (char)0;
        }

        //Поле "Количество поколений"
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || (int)e.KeyChar == 8))
                e.KeyChar = (char)0;//Зпрет на ввод любых символов кроме чисел
            if (textBox3.Text.Length > 5) e.KeyChar = (char)0;
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

        private void правилаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Последовательность действий:\n\n1) Для начала необходимо заполнить поля \"Ширина\" и \"Высота\", после чего нажать кнопку \"Создать мир\"." +
                "\n\n2) Следующим шагом стоит либо расставить живые клетки при помощи мыши, либо заполнить поле \"Количество живых клеток\" и нажать кнопку \"Расставить автоматически\"." +
                "\n\n3) Далее заполняется поле \"Количество поколений\" и если данные соответствуют условиям задачи, то программа готова к запуску и можно нажимать кнопку \"Старт\" " +
                "и при необходимости остановить программу в процессе симуляции нажав кнопку \"Стоп\", " +
                "а если во введённых данных ошибка или вы готовы перейти к новой симуляции, то нажать кнопку \"Очистить мир\" и ввести все данные заново." +
                "\n\nВнимание! Не стоит сразу заполнять все пользовательские поля так как после нажатия некоторых кнопок поля могут обнулятся во избежание ошибок!");
        }
    }
}
