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
using System.Xml.Linq;

namespace RadialBasis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        class RadialBasisNetwork
        {
        
            public double[] radial_basis_function(double x, double[] centers, double widths)
            {
                double[] result = new double[centers.Length];
                for (int i = 0; i < centers.Length; i++)
                {
                    result[i] = Math.Exp(-Math.Pow(x - centers[i], 2) / (2 * Math.Pow(widths, 2)));
                }
                return result;
            }

            public double[] train_rbf_network(double[] x_train, double[] y_train, int n_centers, double learning_rate, int training_epochs)
            {
                double[] centers = CreateLinspace(1, 5, n_centers);
                double widths =1;

                double[][] phi = new double[x_train.Length][];
                for (int i = 0; i < x_train.Length; i++)
                {
                    phi[i] = radial_basis_function(x_train[i], centers, widths);
                }
                /*Ошибка где то здесь*/
                double[] weights = new double[n_centers];
                Random rand = new Random();
                for (int i = 0; i < n_centers; i++)
                {
                    weights[i] = rand.NextDouble();
                }

                for (int epoch = 0; epoch < training_epochs; epoch++)
                {
                    double[] y_pred = new double[x_train.Length];
                    for (int i = 0; i < x_train.Length; i++)
                    {
                        y_pred[i] = phi[i].Zip(weights, (a, b) => a * b).Sum();
                    }

                    double[] error = y_train.Zip(y_pred, (a, b) => a - b).ToArray();

                    double[] delta_weights = new double[n_centers];
                    for (int i = 0; i < n_centers; i++)
                    {
                        for (int j = 0; j < x_train.Length; j++)
                        {
                            delta_weights[i] += learning_rate * phi[j][i] * error[j];
                        }
                    }

                    for (int i = 0; i < n_centers; i++)
                    {
                        weights[i] += delta_weights[i];
                    }
                }

                return weights;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*double[] x_train = Enumerable.Range(1, 10).Select(i => i / 1.0).ToArray();*/
            double[] x_train = CreateLinspace(1, 5, 10);

            double[] y_train = x_train.Select(x => Math.Sqrt(Math.Abs(Math.Pow(x, 2)))).ToArray();
            

            int n_centers = 10;
            double learning_rate = 0.01;
            int training_epochs = 100;

            var RadialBasis = new RadialBasisNetwork();

            double[] trainedWeights = RadialBasis.train_rbf_network(x_train, y_train, n_centers, learning_rate, training_epochs);
            /*double[] trainedWeights = { 0.51035746, 0.40751717, 0.85443882, 0.65111941, 0.97626601, 1.22263574, 1.41520545, 0.87561876, 2.41514814, 2.39758235 };*/
          

        
            double[] x_test = CreateLinspace(1, 5, 100);

            double[] y_test = x_test.Select(x => Math.Sqrt(Math.Abs(Math.Pow(x, 2)))).ToArray();

            double[][] phi_test = new double[x_test.Length][];

            double[] centers = CreateLinspace(1, 5, n_centers);

            for (int i = 0; i < x_test.Length; i++)
            {
                phi_test[i] = RadialBasis.radial_basis_function(x_test[i], centers, 1);
            }

            double[] y_pred = new double[x_test.Length];
            for (int i = 0; i < x_test.Length; i++)
            {
                y_pred[i] = phi_test[i].Zip(trainedWeights, (a, b) => a * b).Sum();
            
            }




            Chart chart = new Chart();
            chart.Dock = DockStyle.Fill;
            Controls.Add(chart);


            // Создание серий для графика
            Series predictedSeries = new Series("Предсказанные значения");
            Series trueSeries = new Series("Истинные значения");
            predictedSeries.ChartType = SeriesChartType.Line;
            trueSeries.ChartType = SeriesChartType.Line;

            // Добавление данных в серии
            for (int i = 0; i < x_test.Length; i++)
            {
                predictedSeries.Points.AddXY(x_test[i], y_pred[i]);
                trueSeries.Points.AddXY(x_test[i], y_test[i]);
            }

            // Добавление серий на график
            chart.Series.Add(predictedSeries);
            chart.Series.Add(trueSeries);

            // Настройка осей и легенды
            chart.ChartAreas.Add(new ChartArea());
            chart.Legends.Add(new Legend());

            // Определение подписей осей
            chart.ChartAreas[0].AxisX.Title = "x";
            chart.ChartAreas[0].AxisY.Title = "y";

            // Отображение легенды
            chart.Legends[0].IsDockedInsideChartArea = false;
            chart.Legends[0].Docking = Docking.Bottom;

            // Отображение формы
            Size = new System.Drawing.Size(800, 600);
        }

        static double[] CreateLinspace(double start, double stop, int num)
        {
            double[] linspace = new double[num];
            double step = (stop - start) / (num - 1);

            for (int i = 0; i < num; i++)
            {
                linspace[i] = start + step * i;
            }

            return linspace;
        }
    }
}
