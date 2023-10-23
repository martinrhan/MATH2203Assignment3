using Avalonia.Controls;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Palettes;
using ScottPlot.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OzoneLinearRegression.Views;

public partial class MainWindow : Window {
    static string Directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
    public MainWindow() {
        InitializeComponent();

        string[] lines = ReadLines("ozone.csv").Skip(1).ToArray();
        double[] Ys = new double[lines.Length]; //FEVs
        double[] Xs = new double[lines.Length]; //FVCs
        for (int i = 0; i < lines.Length; i++) {
            string[] strings = lines[i].Split(',');
            Ys[i] = double.Parse(strings[0]);
            Xs[i] = double.Parse(strings[1]);
        }
        double X_Min = Xs.Min();
        double X_Max = Xs.Max();

        LinearRegressionLine model = new LinearRegressionLine(Xs, Ys);
        AvaPlot1.Plot.Title("OzoneLinearRegression");
        AvaPlot1.Plot.AddScatter(Xs, Ys, lineWidth: 0);
        AvaPlot1.Plot.AddLine(model.slope, model.offset, (X_Min, X_Max));
        AvaPlot1.Plot.SaveFig("OzoneLinearRegression.png");

        AvaPlot avaPlot2 = new AvaPlot();
        Window window2 = new Window() { Content = avaPlot2 };
        avaPlot2.Plot.Title("OzoneLinearRegression_Residuals");
        double[] Residuals = new double[Ys.Length];
        for (int i = 0; i < Residuals.Length; i++) {
            Residuals[i] = Ys[i] - (model.offset + model.slope * Xs[i]);
        }
        avaPlot2.Plot.AddScatter(Xs, Residuals, lineWidth: 0);
        avaPlot2.Plot.AddLine(0, 0, (X_Min, X_Max));
        avaPlot2.Plot.SaveFig("OzoneLinearRegression_Residuals.png");
        window2.Show();

        AvaPlot avaPlot3 = new AvaPlot();
        Window window3 = new Window() { Content= avaPlot3 };
        avaPlot3.Plot.Title("OzoneLinearRegression_Residuals_NormalQuantilePlot");
        double[] Ys_Sorted = Ys.Order().ToArray();
        MathNet.Numerics.Distributions.Normal normal = new();
        double[] quantiles = Enumerable.Range(1, Ys.Length).Select(q => normal.InverseCumulativeDistribution((double)q / (double)(Ys.Length + 1))).ToArray();
        avaPlot3.Plot.AddScatter(quantiles, Ys_Sorted, lineWidth:0);
        LinearRegressionLine model_q = new LinearRegressionLine(quantiles, Ys_Sorted);
        avaPlot3.Plot.AddLine(model_q.slope, model_q.offset, (quantiles[0], quantiles[^1]));
        avaPlot3.Plot.SaveFig("OzoneLinearRegression_Residuals_NormalQuantilePlot.png");
        window3.Show();

        Window window4 = new Window() {};
        double meanSquredError = 0;
        for (int i = 0; i < Xs.Length; i++) {
            meanSquredError += Residuals[i] * Residuals[i];
        }
        meanSquredError /= Xs.Length - 2;
        double sum_den = 0;
        double mean_x = MathNet.Numerics.Statistics.Statistics.Mean(Xs);
        for (int i = 0; i < Xs.Length; i++) {
            double d = Xs[i] - mean_x;
            sum_den += d * d;
        }
        window4.Content = new TextBlock() { Text =
            "Offset: " + model.offset + Environment.NewLine +
            "Slope: " + model.slope + Environment.NewLine +
            "MeanX: " + mean_x + Environment.NewLine +
            "MeanSquredError: " + meanSquredError + Environment.NewLine + 
            sum_den };
        window4.Show();
    }

    internal static IEnumerable<string> ReadLines(string fileName) {
        StreamReader reader;
        string? line;
        reader = new(Directory + @"\" + fileName);
        reader.ReadLine();
        while ((line = reader.ReadLine()) is not null) {
            yield return line;
        }
    }
}
